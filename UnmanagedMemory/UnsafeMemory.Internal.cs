using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

public unsafe partial class UnsafeMemory<T> where T : unmanaged
{
    [UnsafeApi]
    internal void Realloc(int length)
    {
        if (_ptr is not null) {
            throw new InvalidOperationException("You cannot reallocate a non-freed memory block.");
        }
        
        _ptr = Unmanaged.Malloc<T>(length);
    }

    internal void FreeUnmanaged() => Free();
}
