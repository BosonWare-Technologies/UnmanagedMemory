using JetBrains.Annotations;
using UnmanagedMemory.UnmanagedTypes;

namespace UnmanagedMemory.Diagnostics;

internal class UnsafeListDebuggerProxy<T>(UnsafeList<T> list) where T : unmanaged
{
    [UsedImplicitly]
    public unsafe IntPtr StartAddress => (IntPtr)list._items.AsUnsafePointer();

    [UsedImplicitly]
    public int Count => list.Count;

    [UsedImplicitly]
    public int Size => list.Size;

    [UsedImplicitly]
    public T[] Items {
        get {
            if (list.Count > 1000) {
                return [.. list._items.AsSpan(0, 1000)];
            }

            return [.. list._items];
        }
    }
}
