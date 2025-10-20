using JetBrains.Annotations;

namespace UnmanagedMemory.Safety;

/// <summary>
/// Provides methods for handling unmanaged memory leaks.
/// </summary>
[PublicAPI]
public static class MemoryLeakManager
{
    /// <summary>
    /// Delegate for handling memory leaks.
    /// </summary>
    public delegate void MemoryLeakHandler(Context ctx);

    /// <summary>
    /// Provides information about a memory leak.
    /// </summary>
    public unsafe struct Context
    {
        /// <summary>
        /// The pointer to the unmanaged memory.
        /// </summary>
        public void* Pointer;

        /// <summary>
        /// The size in bytes of the unmanaged memory block.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Context"/> struct.
        /// </summary>
        /// <param name="pointer">Pointer to unmanaged memory.</param>
        /// <param name="size">Size of the unmanaged memory block.</param>
        public Context(void* pointer, int size)
        {
            Pointer = pointer;
            Size = size;
        }
    }

    private static MemoryLeakHandler? _handler;

    /// <summary>
    /// Sets a custom memory leak handler.
    /// </summary>
    /// <param name="handler">The handler to be called when a memory leak is detected.</param>
    public static void SetHandler(MemoryLeakHandler handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    /// <summary>
    /// Handles a detected memory leak.
    /// </summary>
    /// <param name="ctx">The memory leak context.</param>
    internal static unsafe void HandleMemoryLeak(Context ctx)
    {
        if (_handler is null)
        {
            // Attempt to free unmanaged memory to avoid leaking
            Unmanaged.Free(ref ctx.Pointer);

            throw new MemoryLeakException(
                "A memory leak was detected and no custom handler was provided. " +
                "Use 'MemoryLeakManager.SetHandler' to define how memory leaks should be handled.");
        }

        _handler.Invoke(ctx);
    }
}
