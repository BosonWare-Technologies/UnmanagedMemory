using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace UnmanagedMemory.UnmanagedTypes;

/// <summary>
///     Represents a mutable string of characters stored in unmanaged memory. This class cannot be inherited.
///     Internally uses <see cref="UnsafeMemory{Char}" /> to manage memory.
/// </summary>
[PublicAPI]
public sealed class NativeStringBuilder : IDisposable
{
    private readonly UnsafeMemory<char> _buffer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NativeStringBuilder" /> class with the specified capacity and growth
    ///     rate.
    /// </summary>
    /// <param name="capacity">Initial character capacity of the buffer. Must be greater than zero.</param>
    /// <param name="growthRate">Amount of memory to grow when the buffer is full. Must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when <paramref name="capacity" /> or
    ///     <paramref name="growthRate" /> is less than or equal to zero.
    /// </exception>
    public NativeStringBuilder(int capacity = 1024, int growthRate = 512)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than zero.");

        if (growthRate <= 0)
            throw new ArgumentOutOfRangeException(nameof(growthRate), "Growth rate must be greater than zero.");

        GrowthRate = growthRate;
        _buffer = new UnsafeMemory<char>(capacity);
    }

    /// <summary>
    ///     Gets or sets the rate at which the internal buffer grows when additional space is required.
    /// </summary>
    public int GrowthRate { get; set; }

    /// <summary>
    ///     Gets the current capacity of the internal buffer.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    ///     Gets the number of characters currently stored.
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    ///     Appends a single character to the current instance.
    /// </summary>
    /// <param name="c">The character to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(char c)
    {
        GrowBufferIfNeeded(1);
        _buffer[Length++] = c;
        return this;
    }

    /// <summary>
    ///     Appends the string representation of a 32-bit integer to the current instance.
    /// </summary>
    /// <param name="value">The integer value to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(int value)
    {
        Span<char> buffer = stackalloc char[12]; // Max length for Int32.MinValue

        value.TryFormat(buffer, out var charsWritten);

        return Append(buffer[..charsWritten]);
    }

    /// <summary>
    ///     Appends the string representation of a <see cref="float" /> to the current instance.
    /// </summary>
    /// <param name="value">The float value to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(float value)
    {
        Span<char> buffer = stackalloc char[32]; // enough for float formatting
        
        return value.TryFormat(buffer, out var charsWritten) 
            ? Append(buffer[..charsWritten]) 
            : this;
    }

    /// <summary>
    ///     Appends the string representation of a <see cref="double" /> to the current instance.
    /// </summary>
    /// <param name="value">The double value to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(double value)
    {
        Span<char> buffer = stackalloc char[32]; // enough for double formatting
        
        return value.TryFormat(buffer, out var charsWritten) 
            ? Append(buffer[..charsWritten]) 
            : this;
    }

    /// <summary>
    ///     Appends the string representation of a <see cref="long" /> to the current instance.
    /// </summary>
    /// <param name="value">The long value to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(long value)
    {
        Span<char> buffer = stackalloc char[20]; // long.MinValue is -9223372036854775808
        
        return value.TryFormat(buffer, out var charsWritten) 
            ? Append(buffer[..charsWritten]) 
            : this;
    }

    /// <summary>
    ///     Appends the string representation of a <see cref="short" /> to the current instance.
    /// </summary>
    /// <param name="value">The short value to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(short value)
    {
        Span<char> buffer = stackalloc char[6]; // short.MinValue is -32768
        
        return value.TryFormat(buffer, out var charsWritten) 
            ? Append(buffer[..charsWritten]) 
            : this;
    }

    /// <summary>
    ///     Appends the string representation of a <see cref="byte" /> to the current instance.
    /// </summary>
    /// <param name="value">The byte value to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(byte value)
    {
        Span<char> buffer = stackalloc char[3]; // byte.MaxValue is 255
        
        return value.TryFormat(buffer, out var charsWritten) 
            ? Append(buffer[..charsWritten]) 
            : this;
    }

    /// <summary>
    ///     Appends a span of characters to the current instance.
    /// </summary>
    /// <param name="span">The span of characters to append.</param>
    /// <returns>The current <see cref="NativeStringBuilder" /> instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(ReadOnlySpan<char> span)
    {
        var spanLength = span.Length;
        GrowBufferIfNeeded(spanLength);

        span.CopyTo(_buffer.AsSpan(Length, spanLength));
        Length += spanLength;

        return this;
    }

    /// <summary>
    ///     Ensures that the buffer has enough capacity for the specified number of additional characters.
    /// </summary>
    /// <param name="additional">The number of characters to accommodate.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBufferIfNeeded(int additional)
    {
        if (Length + additional <= Capacity) return;

        var newCapacity = Capacity + Math.Max(GrowthRate, additional);

        _buffer.Resize(newCapacity);
    }

    /// <summary>
    ///     Converts the contents of this instance to a managed <see cref="string" />.
    /// </summary>
    /// <returns>A managed string containing the characters appended so far.</returns>
    public override string ToString()
    {
        return string.Create(Length, _buffer, static (span, buffer) => {
            buffer.AsSpan(0, span.Length).CopyTo(span);
        });
    }

    /// <summary>
    /// Converts the contents of this instance to a UTF-8 encoded <see cref="UnsafeMemory{T}"/> of bytes.
    /// </summary>
    /// <returns></returns>
    public UnsafeMemory<byte> ToUtf8Bytes()
    {
        var span = _buffer.AsSpan(0, Length);

        var memory = new UnsafeMemory<byte>(span.Length * 4);
        
        var bytesWritten = Encoding.UTF8.GetBytes(span, memory.AsSpan());
        
        memory.Resize(bytesWritten, keepOriginal: true);
        
        return memory;
    }

    /// <summary>
    ///     Releases the unmanaged resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        _buffer.Dispose();
    }
}
