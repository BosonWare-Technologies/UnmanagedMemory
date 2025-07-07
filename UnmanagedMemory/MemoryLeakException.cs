namespace UnmanagedMemory;

[Serializable]
public class MemoryLeakException : Exception
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
