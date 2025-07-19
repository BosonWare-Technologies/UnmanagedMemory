using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

public static class UnsafeMemoryExtensions
{
    [UnsafeApi]
    public static unsafe PointerWrapper<T> GetPointerWrapper<T>(this UnsafeMemory<T> array) where T : unmanaged
    {
        return new PointerWrapper<T>(array.AsUnsafePointer(), array.Length);
    }
}
