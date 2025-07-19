namespace UnmanagedMemory;

[Serializable]
public sealed class MemoryLeakException : UnmanagedMemoryException
{
    public MemoryLeakException()
    {
    }

    public MemoryLeakException(string? message) : base(message)
    {
    }

    public MemoryLeakException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
