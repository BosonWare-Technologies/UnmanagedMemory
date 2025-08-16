using JetBrains.Annotations;
using UnmanagedMemory.Annotations;

namespace UnmanagedMemory;

/// <summary>
///     Provides useful extensions for working with <see cref="UnsafeMemory{T}" /> objects.
/// </summary>
[PublicAPI]
public static class UnsafeMemoryExtensions
{
    /// <summary>
    ///     Creates a <see cref="PointerWrapper{T}" /> around the
    ///     native pointer for the provided <paramref name="array" />
    /// </summary>
    /// <param name="array"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [UnsafeApi]
    public static unsafe PointerWrapper<T> GetPointerWrapper<T>(
        this UnsafeMemory<T> array) where T : unmanaged
    {
        return new PointerWrapper<T>(array.AsUnsafePointer(), array.Length);
    }

    /// <summary>
    ///     Creates an <see cref="UnsafeMemory{T}" /> from the <see cref="IEnumerable{T}" />
    /// </summary>
    /// <param name="source">
    ///     The <see cref="IEnumerable{T}" /> to create an <see cref="UnsafeMemory{T}" /> from.
    /// </param>
    /// <param name="bufferSize">
    ///     The initial size of the buffer.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the elements of source.
    /// </typeparam>
    /// <returns></returns>
    public static UnsafeMemory<T> ToUnsafeMemory<T>(
        this IEnumerable<T> source,
        int bufferSize = 512)
        where T : unmanaged
    {
        var memory = new UnsafeMemory<T>(bufferSize);

        var length = 0;
        try {
            foreach (var element in source) {
                if (length + 1 >= memory.Length)
                    memory.Resize(memory.Length + bufferSize);

                memory[length++] = element;
            }
        }
        catch {
            memory.Dispose();

            throw;
        }
        finally {
            memory.Resize(length); // Trim unused memory.
        }

        return memory;
    }

    /// <summary>
    ///     Asynchronously Creates an <see cref="UnsafeMemory{T}" />
    ///     from the <see cref="IAsyncEnumerable{T}" />
    /// </summary>
    /// <param name="source">
    ///     The <see cref="IAsyncEnumerable{T}" /> to create an <see cref="UnsafeMemory{T}" /> from.
    /// </param>
    /// <param name="bufferSize">
    ///     The initial size of the buffer.
    /// </param>
    /// <typeparam name="T">
    ///     The type of the elements of source.
    /// </typeparam>
    /// <returns></returns>
    public static async Task<UnsafeMemory<T>> ToUnsafeMemoryAsync<T>(
        this IAsyncEnumerable<T> source,
        int bufferSize = 512)
        where T : unmanaged
    {
        var memory = new UnsafeMemory<T>(bufferSize);

        var length = 0;
        try {
            await foreach (var element in source) {
                if (length + 1 >= memory.Length)
                    memory.Resize(memory.Length + bufferSize);

                memory[length++] = element;
            }
        }
        catch {
            memory.Dispose();

            throw;
        }
        finally {
            memory.Resize(length); // Trim unused memory.
        }

        return memory;
    }
}
