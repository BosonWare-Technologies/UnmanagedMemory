using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;

namespace UnmanagedMemory;

public abstract class UnmanagedObject : CriticalFinalizerObject, IDisposable
{
    protected bool IsDisposed;

    /// <summary>
    /// Finalizer that frees unmanaged resources if not already disposed and throws a <see cref="MemoryLeakException"/> if not properly disposed.
    /// </summary>
    ~UnmanagedObject()
    {
        if (!IsDisposed) {
            Free();
        }

        throw new MemoryLeakException();
    }

    protected abstract void Free();

    /// <summary>
    /// Releases the unmanaged memory and suppresses finalization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Free();

        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
        if (IsDisposed) {
            throw new ObjectDisposedException(GetType().Name,
                "The memory has already been disposed.");
        }
    }
}
