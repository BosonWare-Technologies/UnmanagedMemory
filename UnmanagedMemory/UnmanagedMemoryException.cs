namespace UnmanagedMemory;

/// <summary>
///     Base class for all exception in the UnmanagedMemory library.
/// </summary>
[Serializable]
public class UnmanagedMemoryException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UnmanagedMemoryException" /> class.
    /// </summary>
    public UnmanagedMemoryException() { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnmanagedMemoryException" /> class with a specified error message.
    /// </summary>
    /// <param name="message">
    ///     The message that describes the error.
    /// </param>
    public UnmanagedMemoryException(string? message) : base(message) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="UnmanagedMemoryException" /> class with a specified
    ///     error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">
    ///     The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    ///     innerException — The exception that is the cause of the current exception,
    ///     or a null reference (Nothing in Visual Basic) if no inner exception is specified.
    /// </param>
    public UnmanagedMemoryException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
