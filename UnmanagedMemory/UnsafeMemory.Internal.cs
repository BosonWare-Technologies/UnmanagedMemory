namespace UnmanagedMemory;

internal unsafe interface IUnsafeMemory
{
    void* NativePointer { get; }
}

public unsafe partial class UnsafeMemory<T>
    : IUnsafeMemory where T : unmanaged
{
    void* IUnsafeMemory.NativePointer => _ptr;
}
