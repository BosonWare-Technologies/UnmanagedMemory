using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnmanagedMemory.Annotations;
using UnmanagedMemory.Diagnostics;

namespace UnmanagedMemory;

/// <summary>
/// Provides a managed wrapper around a block of unmanaged memory 
/// for elements of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The unmanaged type of elements to store in memory.</typeparam>
/// <remarks>
/// <para>
/// <see cref="UnsafeMemory{T}"/> 
/// allocates and manages a block of unmanaged memory for 
/// storing elements of type <typeparamref name="T"/>.
/// It implements <see cref="IEnumerable{T}"/> for enumeration and 
/// <see cref="IDisposable"/> for explicit resource management.
/// </para>
/// <para>
/// The memory is allocated using <see cref="Unmanaged.Malloc{T}(int)"/> 
/// and must be explicitly freed by calling <see cref="Dispose"/> or <see cref="Free"/>.
/// Accessing the memory after disposal will throw an <see cref="ObjectDisposedException"/>.
/// </para>
/// </remarks>
[DebuggerTypeProxy(typeof(UnsafeMemoryDebuggerProxy<>))]
public unsafe class UnsafeMemory<T> : UnmanagedObject, IEnumerable<T> where T : unmanaged
{
    /// <summary>
    /// Enumerator for <see cref="UnsafeMemory{T}"/>, enabling iteration over the elements.
    /// </summary>
    public struct Enumerator(UnsafeMemory<T> memory) : IEnumerator<T>
    {
        private readonly UnsafeMemory<T> _memory = memory;

        private int _index = -1;

        public readonly T Current => _memory[_index];

        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (_index + 1 >= _memory.Length)
                return false;

            _index++;

            return true;
        }

        public void Reset() => _index = -1;

        public readonly void Dispose() { }
    }

    /// <summary>
    /// Gets an empty instance of <see cref="UnsafeMemory{T}"/>.
    /// </summary>
    public static readonly UnsafeMemory<T> Empty = new(0);

    private T* _ptr;

    /// <summary>
    /// Gets the number of elements in the memory block.
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    /// Gets the total size in bytes of the allocated memory block.
    /// </summary>
    public int Size { get; private set; }

    /// <summary>
    /// Gets a reference to the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index"/> is out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.
    /// </exception>
    public ref T this[int index] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get {
            ThrowIfDisposed();

            if (index < 0 || index >= Length) {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return ref _ptr[index];
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeMemory{T}"/> 
    /// class with the specified length.
    /// </summary>
    /// <param name="length">The number of elements to allocate.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="length"/> is negative.
    /// </exception>
    public UnsafeMemory(int length)
    {
        if (length < 0) {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
        }

        Length = length;
        Size = sizeof(T) * length;

        if (length > 0) {
            _ptr = Unmanaged.Malloc<T>(Size);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ReadOnlySpan<T> span, int start = 0)
    {
        var destination = AsSpan(start, span.Length);

        span.CopyTo(destination);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Expand(int length)
    {
        if (length <= Length) {
            throw new ArgumentOutOfRangeException("Length must be greater than the current Length.",
                nameof(length));
        }

        if (Length == 0) {
            throw new InvalidOperationException("You may not expand a zero length memory block.");
        }

        T* source = _ptr;
        T* destination = Unmanaged.Malloc<T>(sizeof(T) * length);

        Unmanaged.Copy(source, destination, Length);

        Unmanaged.Free(ref source);

        _ptr = destination;
        Length = length;
        Size = sizeof(T) * length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear() => AsSpan().Fill(default);

    /// <summary>
    /// Returns a <see cref="Span{T}"/> representing the entire memory block.
    /// </summary>
    /// <returns>A <see cref="Span{T}"/> over the memory block.</returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the memory has been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        ThrowIfDisposed();

        return new Span<T>(_ptr, Length);
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> representing a range within the memory block.
    /// </summary>
    /// <param name="start">The starting index of the span.</param>
    /// <returns>A <see cref="Span{T}"/> over the specified range.</returns>
    /// <exception 
    /// cref="ArgumentOutOfRangeException">Thrown if <paramref name="start"/> is out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the memory has been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int start)
    {
        ThrowIfDisposed();

        // Sanitize user input.
        if (start < 0 || start >= Length) {
            throw new ArgumentOutOfRangeException(nameof(start), "Start index must be within the bounds of the memory block.");
        }

        if (Length == 0) {
            return Span<T>.Empty;
        }

        // Create a span from the specified range.
        return new Span<T>(&_ptr[start], Length - start);
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> representing a range within the memory block.
    /// </summary>
    /// <param name="start">The starting index of the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <returns>A <see cref="Span{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="start"/> or <paramref name="length"/> are out of range.
    /// </exception>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int start, int length)
    {
        ThrowIfDisposed();

        // Sanitize user input.
        if (length < 0) {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
        }
        if (start < 0 || start >= Length) {
            throw new ArgumentOutOfRangeException(nameof(start), "Start index must be within the bounds of the memory block.");
        }
        if (start + length > Length) {
            throw new ArgumentOutOfRangeException(nameof(length), "The specified range exceeds the bounds of the memory block.");
        }

        if (length == 0) {
            return Span<T>.Empty;
        }

        // Create a span from the specified range.
        return new Span<T>(&_ptr[start], length);
    }

    /// <summary>
    /// Copies the contents of the unmanaged memory block to a new managed array.
    /// </summary>
    /// <returns>A managed array containing the elements of the memory block.</returns>
    public T[] ToArray() => AsSpan().ToArray();

    [UnsafeApi]
    public T* AsUnsafePointer()
    {
        ThrowIfDisposed();

        return _ptr;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the memory block.
    /// </summary>
    /// <returns>An enumerator for the memory block.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        ThrowIfDisposed();

        return new Enumerator(this);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the memory block.
    /// </summary>
    /// <returns>An enumerator for the memory block.</returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Frees the unmanaged memory block.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the memory has already been disposed.
    /// </exception>
    protected override void Free() => Unmanaged.Free(ref _ptr);

    /// <summary>
    /// Returns an enumerator that iterates through the memory block.
    /// </summary>
    /// <returns>An enumerator for the memory block.</returns>
    /// <exception cref="ObjectDisposedException">
    /// Thrown if the memory has been disposed.
    /// </exception>
    public static explicit operator Span<T>(UnsafeMemory<T> memory) => memory.AsSpan();

    public static explicit operator ReadOnlySpan<T>(UnsafeMemory<T> memory) => memory.AsSpan();
}
