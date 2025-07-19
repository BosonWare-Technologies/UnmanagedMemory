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
        // FIX:
        //  Remove the `if (!IsDisposed)` statement which is not required since the finalizer
        //  can only run if the unmanaged object has not been disposed.
        
        Free(); // Free the unmanaged resources.

        throw new MemoryLeakException();
    }

    protected abstract void Free();

    /// <summary>
    /// Releases the unmanaged object and suppresses finalization.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void Dispose()
    {
        Free();
        IsDisposed = true;

        GC.SuppressFinalize(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ThrowIfDisposed()
    {
        if (IsDisposed) {
            throw new ObjectDisposedException(GetType().Name,
                "The unmanaged object has already been disposed.");
        }
    }
}
