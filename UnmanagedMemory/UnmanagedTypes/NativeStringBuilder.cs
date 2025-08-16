using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace UnmanagedMemory.UnmanagedTypes;

// NOTE: NativeStringBuilder doesn't need to call ThrowIfDisposed() since
// the internal _buffer handles disposal.

/// <summary>
///     Represents a mutable string of characters. This class cannot be inherited.
///     <para>
///         Note: Uses an <see cref="UnsafeMemory{Char}" /> for the internal buffer.
///     </para>
/// </summary>
[PublicAPI]
public sealed class NativeStringBuilder : UnmanagedObject
{
    private readonly UnsafeMemory<char> _buffer;

    /// <summary>
    ///     Initialize a new instance of the <see cref="NativeStringBuilder" /> class.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    /// <param name="growthRate">The rate at which the buffer grows.</param>
    public NativeStringBuilder(
        int capacity = 1024,
        int growthRate = 512)
    {
        if (capacity <= 0) {
            throw new ArgumentOutOfRangeException(nameof(capacity),
                "Capacity must be greater than or equal to zero.");
        }

        if (growthRate <= 0) {
            throw new ArgumentOutOfRangeException(nameof(growthRate),
                "GrowthRate must be greater than or equal to zero.");
        }

        GrowthRate = growthRate;

        _buffer = new UnsafeMemory<char>(capacity);
    }

    /// <summary>
    ///     Gets or Sets the rate at which the buffer grows.
    /// </summary>
    public int GrowthRate { get; set; }

    /// <summary>
    ///     Gets the current capacity.
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    ///     Gets the total number of characters.
    /// </summary>
    public int Length { get; private set; }

    /// <summary>
    ///     Appends the string representation of a specified Char object to this instance.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(char c)
    {
        GrowBufferIfNeeded(1);

        _buffer[Length++] = c;

        return this;
    }

    /// <summary>
    ///     Appends the string representation of a specified <see cref="int" /> object to this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(int value)
    {
        Span<char> buffer = stackalloc char[12];

        value.TryFormat(buffer, out var charsWritten);

        return Append(buffer[..charsWritten]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public NativeStringBuilder Append(ReadOnlySpan<char> span)
    {
        GrowBufferIfNeeded(span.Length);

        var destination = _buffer.AsSpan(Length, span.Length);

        span.CopyTo(destination);

        Length += span.Length;

        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void GrowBufferIfNeeded(int consumed)
    {
        if (Length + consumed >= Capacity) {
            _buffer.Resize(Capacity + GrowthRate, true);
        }
    }

    /// <summary>
    ///     Converts the value of this instance to a managed <see cref="String" />.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return string.Create(Length, _buffer, (span, buffer) => {
            var chars = (ReadOnlySpan<char>)buffer.AsSpan(0, Length);

            chars.CopyTo(span);
        });
    }

    /// <inheritdoc />
    protected override void Free()
    {
        _buffer.Dispose();
    }
}
