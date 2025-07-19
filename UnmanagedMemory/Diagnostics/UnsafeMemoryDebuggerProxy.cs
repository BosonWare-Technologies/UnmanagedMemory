namespace UnmanagedMemory.Diagnostics;

internal class UnsafeMemoryDebuggerProxy<T>(UnsafeMemory<T> memory) where T : unmanaged
{
    private readonly UnsafeMemory<T> _memory = memory;

    public unsafe IntPtr StartAddress => (IntPtr)_memory.AsUnsafePointer();

    public IntPtr EndAddress => StartAddress + _memory.Size;

    public int Length => _memory.Length;

    public int Size => _memory.Size;

    public T[] Items {
        get {
            if (_memory.Length > 1000) {
                return [.. _memory.AsSpan(0, 1000)];
            }
            
            return [.. _memory];
        }
    }
}
