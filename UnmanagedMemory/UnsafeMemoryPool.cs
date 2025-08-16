namespace UnmanagedMemory;

/// <summary>
///     Represents a pool of <see cref="UnsafeMemory{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
[Obsolete("Unsafe memory pools are not supported", true)]
public static class UnsafeMemoryPool<T> where T : unmanaged
{
    public static UnsafeMemory<T> Rent(int length)
    {
        throw new NotSupportedException("Unsafe memory pools are not supported");
    }

    public static void Return(UnsafeMemory<T> buffer)
    {
        throw new NotSupportedException("Unsafe memory pools are not supported");
    }
}
