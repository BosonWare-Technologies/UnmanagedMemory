using UnmanagedMemory.UnmanagedTypes;

namespace UnmanagedMemory.Diagnostics;

internal class UnsafeListDebuggerProxy<T>(UnsafeList<T> list) where T : unmanaged
{
    public unsafe IntPtr StartAddress => (IntPtr)list._items.AsUnsafePointer();

    public int Count => list.Count;

    public int Size => list.Size;

    public T[] Items {
        get {
            if (list.Count > 1000) {
                return [.. list._items.AsSpan(0, 1000)];
            }
            
            return [.. list._items];
        }
    }
}
