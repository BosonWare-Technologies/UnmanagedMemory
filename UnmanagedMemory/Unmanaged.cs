using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

[UnsafeApi]
public static unsafe class Unmanaged
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Malloc(int size) => (void*)Marshal.AllocHGlobal(size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* Malloc<T>(int size) where T : unmanaged => (T*)Marshal.AllocHGlobal(size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free(ref void* ptr)
    {
        if (ptr is not null) {
            Marshal.FreeHGlobal((nint)ptr);

            ptr = null; // Disallow the reuse of that pointer.
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free<T>(ref T* ptr) where T : unmanaged
    {
        if (ptr is not null) {
            Marshal.FreeHGlobal((nint)ptr);

            ptr = null; // Disallow the reuse of that pointer.
        }
    }

    [UnsafeApi]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Copy<T>(T* source, T* destination, int length) where T : unmanaged
    {
        new Span<T>(source, length).CopyTo(new Span<T>(destination, length));
    }
}
