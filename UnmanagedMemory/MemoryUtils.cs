using System.Runtime.InteropServices;

namespace UnmanagedMemory;

public static unsafe class MemoryUtils
{
    public static void* Malloc(int size) => (void*)Marshal.AllocHGlobal(size);

    public static void Free(ref void* ptr)
    {
        Marshal.FreeHGlobal((nint)ptr);

        ptr = null; // Disallow the reuse of that pointer.
    }

    public static T* Malloc<T>(int size) where T : unmanaged => (T*)Marshal.AllocHGlobal(size);

    public static void Free<T>(ref T* ptr) where T : unmanaged
    {
        Marshal.FreeHGlobal((nint)ptr);

        ptr = null; // Disallow the reuse of that pointer.
    }
}
