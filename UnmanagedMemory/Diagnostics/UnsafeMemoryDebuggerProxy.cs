using JetBrains.Annotations;

namespace UnmanagedMemory.Diagnostics;

internal class UnsafeMemoryDebuggerProxy<T>(UnsafeMemory<T> memory) where T : unmanaged
{
    [UsedImplicitly]
    public unsafe IntPtr StartAddress => (IntPtr)memory.AsUnsafePointer();

    [UsedImplicitly]
    public IntPtr EndAddress => StartAddress + memory.Size;

    [UsedImplicitly]
    public int Length => memory.Length;

    [UsedImplicitly]
    public int Size => memory.Size;

    [UsedImplicitly]
    public T[] Items {
        get {
            if (memory.Length > 1000) {
                return [.. memory.AsSpan(0, 1000)];
            }

            return [.. memory];
        }
    }
}
