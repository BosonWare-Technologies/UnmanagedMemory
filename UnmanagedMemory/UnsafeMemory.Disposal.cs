using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace UnmanagedMemory;

public sealed unsafe partial class UnsafeMemory<T>
    : CriticalFinalizerObject, IDisposable where T : unmanaged
{
    /// <summary>
    ///     Finalizer that frees unmanaged resources if not already disposed and throws a
    ///     <see cref="MemoryLeakException" /> if
    ///     not properly disposed.
    /// </summary>
    ~UnsafeMemory()
    {
        if (_ptr is null)
            return;

        Free(); // Free the unmanaged resources.

        throw new MemoryLeakException();
    }

    /// <summary>
    ///     Frees the unmanaged memory block.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    ///     Thrown if the memory has already been disposed.
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Free()
    {
        Unmanaged.Free(ref _ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfDisposed()
    {
        if (_ptr is null) {
            throw new ObjectDisposedException(
                nameof(UnsafeMemory<>),
                "The unmanaged memory has already been disposed."
            );
        }
    }

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (_length == 0) {
            throw new InvalidOperationException("You may not dispose a zero length memory block.");
        }

        Free();

        GC.SuppressFinalize(this);
    }
}
