using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnmanagedMemory.Annotations;
using UnmanagedMemory.Diagnostics;

namespace UnmanagedMemory;

/// <summary>
///     Provides a managed wrapper around a block of unmanaged memory
///     for elements of type <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The unmanaged type of elements to store in memory.</typeparam>
/// <remarks>
///     <para>
///         <see cref="UnsafeMemory{T}" />
///         allocates and manages a block of unmanaged memory for
///         storing elements of type <typeparamref name="T" />.
///         It implements <see cref="IEnumerable{T}" /> for enumeration and
///         <see cref="IDisposable" /> for explicit resource management.
///     </para>
///     <para>
///         The memory is allocated using <see cref="Unmanaged.Malloc{T}(int)" />
///         and must be explicitly freed by calling <see cref="Dispose" /> or <see cref="Free" />.
///         Accessing the memory after disposal will throw an
///         <see cref="ObjectDisposedException" /> or
///         <see cref="NullReferenceException" />.
///     </para>
/// </remarks>
[PublicAPI]
[DebuggerTypeProxy(typeof(UnsafeMemoryDebuggerProxy<>))]
public sealed unsafe partial class UnsafeMemory<T> : IEnumerable<T> where T : unmanaged
{
    /// <summary>
    ///     Gets an empty instance of <see cref="UnsafeMemory{T}" />.
    /// </summary>
    public static readonly UnsafeMemory<T> Empty = new(0);

    private T* _ptr;
    
    private int _length;

    /// <summary>
    ///     Gets the number of elements in the memory block.
    /// </summary>
    public int Length => _length;

    /// <summary>
    ///     Gets the total size in bytes of the allocated memory block.
    /// </summary>
    public int Size => _length * sizeof(T);

    /// <summary>
    ///     Gets a reference to the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="index" /> is out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has been disposed.
    /// </exception>
    [IndexerName("Items")]
    public ref T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            if ((uint)index >= (uint)_length) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return ref _ptr[index];
        }
    }
    
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnsafeMemory{T}" />
    ///     class with the specified length.
    /// </summary>
    /// <param name="length">The number of elements to allocate.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="length" /> is negative.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public UnsafeMemory(int length)
    {
        if (length < 0) {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
        }

        _length = length;

        if (length > 0) {
            _ptr = Unmanaged.Malloc<T>(Size);
        }
    }

    /// <summary>
    ///     Initializes every element of the memory block.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize()
    {
        AsSpan().Fill(default);
    }

    /// <summary>
    ///     Fills the elements of this memory with a specified value.
    /// </summary>
    /// <param name="value">
    ///     The value to assign to each element of the memory.
    /// </param>
    public void Fill(T value)
    {
        AsSpan().Fill(value);
    }

    /// <summary>
    ///     Writes the contents of the specified <see cref="ReadOnlySpan{T}" /> to the unmanaged memory,
    ///     starting at the specified index.
    /// </summary>
    /// <param name="span">The source span containing the data to write.</param>
    /// <param name="start">
    ///     The zero-based index in the unmanaged memory at which to begin writing.
    ///     Defaults to 0.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="start" /> is less than zero or if the range specified by
    ///     <paramref name="start" /> and <c>span.Length</c> exceeds the bounds of the unmanaged memory.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<T> span, int start = 0)
    {
        var destination = AsSpan(start, span.Length);

        span.CopyTo(destination);
    }

    /// <summary>
    ///     Resizes the unmanaged memory block to the specified length.
    /// </summary>
    /// <param name="length">The new length of the memory block.</param>
    /// <param name="keepOriginal">
    ///     If <c>true</c>, the contents of the original memory
    ///     block are copied to the new block up to the minimum of the old and new lengths.
    ///     If <c>false</c>, the contents are not preserved.
    /// </param>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory block has already been disposed.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///     Thrown if attempting to resize a zero-length memory block.
    /// </exception>
    [UnsafeApi(Comment = "Silently breaks any existing spans.")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Resize(int length, bool keepOriginal = true)
    {
        ThrowIfDisposed();

        if (Length == 0) {
            throw new InvalidOperationException("You may not resize a zero length memory block.");
        }

        if (length == 0) {
            Unmanaged.Free(ref _ptr);

            _length = 0;

            return;
        }

        var source = _ptr;
        var destination = Unmanaged.Malloc<T>(sizeof(T) * length);

        if (keepOriginal) {
            var elementsToCopy = Math.Min(_length, length);

            Unmanaged.Copy(source, destination, elementsToCopy);
        }

        Unmanaged.Free(ref source);

        _ptr = destination;
        _length = length;
    }

    /// <summary>
    ///     Clears the contents of this <see cref="UnsafeMemory{T}" /> object.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        ThrowIfDisposed();
        
        AsSpan().Clear();
    }

    /// <summary>
    ///     Returns a <see cref="Span{T}" /> representing the entire memory block.
    /// </summary>
    /// <returns>A <see cref="Span{T}" /> over the memory block.</returns>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        ThrowIfDisposed();

        return new Span<T>(_ptr, _length);
    }

    /// <summary>
    ///     Returns a <see cref="Span{T}" /> representing a range within the memory block.
    /// </summary>
    /// <param name="start">The starting index of the span.</param>
    /// <returns>A <see cref="Span{T}" /> over the specified range.</returns>
    /// <exception
    ///     cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="start" /> is out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int start)
    {
        ThrowIfDisposed();

        // Sanitize user input.
        if ((uint)start >= _length)
            throw new ArgumentOutOfRangeException(nameof(start), "Start index must be within the bounds of the memory block.");

        return _length == 0
            ? Span<T>.Empty
            :
            // Create a span from the specified range.
            new Span<T>(&_ptr[start], _length - start);
    }

    /// <summary>
    ///     Returns a <see cref="Span{T}" /> representing a range within the memory block.
    /// </summary>
    /// <param name="start">The starting index of the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <returns>A <see cref="Span{T}" /> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown if <paramref name="start" /> or <paramref name="length" /> are out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int start, int length)
    {
        ThrowIfDisposed();

        // Sanitize user input.
        if (length < 0) {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
        }
        if ((uint)start >= (uint)_length) {
            throw new ArgumentOutOfRangeException(nameof(start), "Start index must be within the bounds of the memory block.");
        }
        if (start + length > _length)
            throw new ArgumentOutOfRangeException(nameof(length), "The specified range exceeds the bounds of the memory block.");

        return length == 0
            ? Span<T>.Empty
            :
            // Create a span from the specified range.
            new Span<T>(&_ptr[start], length);
    }

    /// <summary>
    ///     Copies the contents of the unmanaged memory block to a new managed array.
    /// </summary>
    /// <returns>A managed array containing the elements of the memory block.</returns>
    public T[] ToArray()
    {
        return AsSpan().ToArray();
    }

    /// <summary>
    ///     Returns a raw pointer to the allocated memory.
    /// </summary>
    /// <returns></returns>
    [UnsafeApi(Comment = "This API is for internal use.")]
    public T* AsUnsafePointer()
    {
        ThrowIfDisposed();

        return _ptr;
    }
    
    /// <summary>
    ///     Returns an enumerator that iterates through the memory block.
    /// </summary>
    /// <returns>An enumerator for the memory block.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();

        return new Enumerator(this);
    }

    /// <summary>
    ///     Returns an enumerator that iterates through the memory block.
    /// </summary>
    /// <returns>An enumerator for the memory block.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Returns a <see cref="Span{T}" /> representing the entire memory block.
    /// </summary>
    /// <returns>A <see cref="Span{T}" /> over the memory block.</returns>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has been disposed.
    /// </exception>
    public static explicit operator Span<T>(UnsafeMemory<T> memory)
    {
        return memory.AsSpan();
    }

    /// <summary>
    ///     Returns a <see cref="ReadOnlySpan{T}" /> representing the entire memory block.
    /// </summary>
    /// <returns>A <see cref="ReadOnlySpan{T}" /> over the memory block.</returns>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has been disposed.
    /// </exception>
    public static explicit operator ReadOnlySpan<T>(UnsafeMemory<T> memory)
    {
        return memory.AsSpan();
    }

    /// <summary>
    ///     Enumerator for <see cref="UnsafeMemory{T}" />, enabling iteration over the elements.
    /// </summary>
    public struct Enumerator(UnsafeMemory<T> memory) : IEnumerator<T>
    {
        private int _index = -1;

        /// <inheritdoc />
        public readonly T Current => memory[_index];

        /// <inheritdoc />
        readonly object IEnumerator.Current => Current;

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            if (_index + 1 >= memory.Length)
                return false;

            _index++;

            return true;
        }

        /// <inheritdoc />
        public void Reset()
        {
            _index = -1;
        }

        /// <inheritdoc />
        public readonly void Dispose() { }
    }
}
