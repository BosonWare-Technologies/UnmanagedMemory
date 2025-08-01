namespace UnmanagedMemory.Diagnostics;

internal class UnsafeMemoryDebuggerProxy<T>(UnsafeMemory<T> memory) where T : unmanaged
{
    public unsafe IntPtr StartAddress => (IntPtr)memory.AsUnsafePointer();

    public IntPtr EndAddress => StartAddress + memory.Size;

    public int Length => memory.Length;

    public int Size => memory.Size;

    public T[] Items {
        get {
            if (memory.Length > 1000) {
                return [.. memory.AsSpan(0, 1000)];
            }
            
            return [.. memory];
        }
    }
}
