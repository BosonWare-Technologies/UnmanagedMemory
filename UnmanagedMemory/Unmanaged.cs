#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

/// <summary>
///     Provides utility methods for allocating and freeing unmanaged memory.
///     <para>
///         Contains unsafe methods. Use <see cref="UnsafeMemory{T}" /> instead.
///     </para>
/// </summary>
[UnsafeApi(Comment = "This API is meant for internal use.")]
public static unsafe class Unmanaged
{
    // ReSharper disable once UnusedMember.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Malloc(int size)
    {
        return (void*)Marshal.AllocHGlobal(size);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* Malloc<T>(int size) where T : unmanaged
    {
        return (T*)Marshal.AllocHGlobal(size);
    }

    // ReSharper disable once UnusedMember.Global
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free(ref void* ptr)
    {
        if (ptr is null) return;

        Marshal.FreeHGlobal((nint)ptr);

        ptr = null; // Disallow the reuse of that pointer.
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free<T>(ref T* ptr) where T : unmanaged
    {
        if (ptr is null) return;

        Marshal.FreeHGlobal((nint)ptr);

        ptr = null; // Disallow the reuse of that pointer.
    }

    [UnsafeApi]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void Copy<T>(T* source, T* destination, int length) where T : unmanaged
    {
        new Span<T>(source, length).CopyTo(new Span<T>(destination, length));
    }
}
