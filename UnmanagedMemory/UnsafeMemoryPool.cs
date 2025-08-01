using System.Collections.Concurrent;
using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

/// <summary>
/// Represents a pool of <see cref="UnsafeMemory{T}"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public static class UnsafeMemoryPool<T> where T : unmanaged
{
    private static readonly ConcurrentStack<UnsafeMemory<T>> Free = [];

    public static UnsafeMemory<T> Rent(int length)
    {
        if (Free.TryPop(out var buff)) {
            buff.Realloc(length);
        }
        else {
            buff = new UnsafeMemory<T>(length);
        }
        
        return buff;
    }
    
    public static void Return(UnsafeMemory<T> buffer)
    {
        buffer.FreeUnmanaged();
        
        Free.Push(buffer);
    }
}
