using JetBrains.Annotations;
using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

/// <summary>
///     Represents a wrapper around an unmanaged pointer to a contiguous block of memory of type <typeparamref name="T" />.
/// </summary>
/// <typeparam name="T">The unmanaged type of the elements in the memory block.</typeparam>
/// <param name="ptr">A pointer to the start of the unmanaged memory block.</param>
/// <param name="length">The number of elements in the memory block.</param>
/// <remarks>
///     This struct provides safe access to unmanaged memory by encapsulating a pointer and its length.
/// </remarks>
[UnsafeApi, PublicAPI]
public unsafe readonly struct PointerWrapper<T>(T* ptr, int length)
    where T : unmanaged
{
    public readonly T* Ptr = ptr;

    public readonly int Length = length;

    public ref T this[int index] {
        get {
            ref var elem = ref Ptr[index];

            return ref elem;
        }
    }
}
