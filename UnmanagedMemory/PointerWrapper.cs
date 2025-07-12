using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;


[UnsafeApi]
public readonly unsafe struct PointerWrapper<T>(T* ptr, int length)
    where T : unmanaged
{
    public readonly T* Ptr = ptr;

    public readonly int Length = length;

    public ref T this[int index] {
        get {
            ref T elem = ref Ptr[index];

            return ref elem;
        }
    }
}
