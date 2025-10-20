namespace UnmanagedMemory;

/// <summary>
/// Represents an unmanaged memory leak error.
/// </summary>
[Serializable]
public sealed class MemoryLeakException : UnmanagedMemoryException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryLeakException" /> class.
    /// </summary>
    public MemoryLeakException(): base("Unhanded memory leak exception") { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MemoryLeakException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the error.
    /// </param>
    public MemoryLeakException(string? message) : base(message) { }
}
