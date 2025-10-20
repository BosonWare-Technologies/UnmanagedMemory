using System.Text;
using BenchmarkDotNet.Attributes;
using UnmanagedMemory.UnmanagedTypes;

namespace UnmanagedMemory.Lab;

[MemoryDiagnoser(false)]
public class Benchmarks
{
    [Benchmark]
    public void NativeStringBuilder()
    {
        using var builder = new NativeStringBuilder(capacity: 2048);

        builder.Append('[');
        for (var i = 0; i < 250; i++) {
            builder.Append(i);
        }
        builder.Append(']');

        using var ut8Bytes = builder.ToUtf8Bytes();
    }

    [Benchmark]
    public void ManagedStringBuilder()
    {
        var builder = new StringBuilder();

        builder.Append('[');
        for (var i = 0; i < 250; i++) {
            builder.Append(i);
        }
        builder.Append(']');

        // StringBuilder can be very expensive if you need a UTF-8 encoded byte array.
        var bytes = Encoding.UTF8.GetBytes(builder.ToString());
    }
}
