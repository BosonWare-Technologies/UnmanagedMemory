using System.Collections;
using System.Runtime.CompilerServices;

namespace UnmanagedMemory;
/// <summary>
/// Provides a managed wrapper around a block of unmanaged memory for elements of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The unmanaged type of elements to store in memory.</typeparam>
/// <remarks>
/// <para>
/// <see cref="UnsafeMemory{T}"/> allocates and manages a block of unmanaged memory for storing elements of type <typeparamref name="T"/>.
/// It implements <see cref="IEnumerable{T}"/> for enumeration and <see cref="IDisposable"/> for explicit resource management.
/// </para>
/// <para>
/// The memory is allocated using <see cref="MemoryUtils.Malloc{T}(int)"/> and must be explicitly freed by calling <see cref="Dispose"/> or <see cref="Free"/>.
/// Accessing the memory after disposal will throw an <see cref="ObjectDisposedException"/>.
/// </para>
/// </remarks>
public unsafe class UnsafeMemory<T> : IEnumerable<T>, IDisposable where T : unmanaged
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

    private bool _isDisposed;

    private T* _ptr;

    /// <summary>
    /// Gets the number of elements in the memory block.
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// Gets the total size in bytes of the allocated memory block.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets a reference to the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get.</param>
    /// <returns>A reference to the element at the specified index.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index"/> is out of range.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.</exception>
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
    /// Finalizer that frees unmanaged resources if not already disposed and throws a <see cref="MemoryLeakException"/> if not properly disposed.
    /// </summary>
    ~UnsafeMemory()
    {
        if (!_isDisposed) {
            Free();
        }

        throw new MemoryLeakException();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsafeMemory{T}"/> class with the specified length.
    /// </summary>
    /// <param name="length">The number of elements to allocate.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="length"/> is negative.</exception>
    public UnsafeMemory(int length)
    {
        if (length < 0) {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");
        }

        Length = length;
        Size = sizeof(T) * length;

        if (length > 0) {
            _ptr = MemoryUtils.Malloc<T>(Size);
        }
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> representing the entire memory block.
    /// </summary>
    /// <returns>A <see cref="Span{T}"/> over the memory block.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.</exception>
    public Span<T> AsSpan()
    {
        ThrowIfDisposed();

        return new Span<T>(_ptr, Length);
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> representing a range within the memory block.
    /// </summary>
    /// <param name="start">The starting index of the span.</param>
    /// <param name="length">The number of elements in the span.</param>
    /// <returns>A <see cref="Span{T}"/> over the specified range.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="start"/> or <paramref name="length"/> are out of range.</exception>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.</exception>
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
        // Create a span from the specified range.
        if (length == 0) {
            return Span<T>.Empty;
        }

        return new Span<T>(&_ptr[start], length);
    }

    public T* AsUnsafePointer() => _ptr;

    /// <summary>
    /// Copies the contents of the unmanaged memory block to a new managed array.
    /// </summary>
    /// <returns>A managed array containing the elements of the memory block.</returns>
    public T[] ToArray() => AsSpan().ToArray();

    /// <summary>
    /// Frees the unmanaged memory block.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has already been disposed.</exception>
    public void Free()
    {
        ThrowIfDisposed();

        MemoryUtils.Free(ref _ptr);

        _isDisposed = true;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the memory block.
    /// </summary>
    /// <returns>An enumerator for the memory block.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the memory has been disposed.</exception>

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
    /// Releases the unmanaged memory and suppresses finalization.
    /// </summary>
    public void Dispose()
    {
        Free();

        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (_isDisposed) {
            throw new ObjectDisposedException(nameof(UnsafeMemory<T>), "The memory has already been disposed.");
        }
    }
}
