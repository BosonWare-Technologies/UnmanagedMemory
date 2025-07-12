using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

public static class UnsafeMemoryExtensions
{
    extension<T>(UnsafeMemory<T> array) where T : unmanaged
    {
        [UnsafeApi]
        public unsafe PointerWrapper<T> GetPointerWrapper()
            => new(array.AsUnsafePointer(), array.Length);
    }
}
