using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using UnmanagedMemory.Safety;

namespace UnmanagedMemory;

public sealed unsafe partial class UnsafeMemory<T>
    : CriticalFinalizerObject, IDisposable where T : unmanaged
{
    /// <summary>
    /// Gets a value that indicates whether this memory block has not been disposed.
    /// <returns>
    /// true if the memory block is not disposed, otherwise false.
    /// </returns>
    /// </summary>
    public bool IsAlive => _ptr is not null;
    
    /// <summary>
    ///     Finalizer that frees unmanaged resources if not already disposed and throws a
    ///     <see cref="MemoryLeakException" /> if
    ///     not properly disposed.
    /// </summary>
    ~UnsafeMemory()
    {
        if (_ptr is null) 
        {
            return; // N/A
        }

        MemoryLeakManager.HandleMemoryLeak(new MemoryLeakManager.Context(_ptr, Size));
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
        if (_ptr is null && _length > 0) {
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
