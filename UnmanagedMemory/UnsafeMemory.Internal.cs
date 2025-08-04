using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

internal unsafe interface IUnsafeMemory
{
    void* NativePointer { get; }
}

public unsafe partial class UnsafeMemory<T> 
    : IUnsafeMemory where T : unmanaged
{
    void* IUnsafeMemory.NativePointer => _ptr;
    
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
