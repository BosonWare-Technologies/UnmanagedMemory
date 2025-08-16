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

        builder.Append("[");
        for (var i = 0; i < 250; i++) {
            builder.Append(i);
        }
        builder.Append("]");

        var str = builder.ToString();
    }

    [Benchmark]
    public void ManagedStringBuilder()
    {
        var builder = new StringBuilder();

        builder.Append("[");
        for (var i = 0; i < 250; i++) {
            builder.Append(i);
        }
        builder.Append("]");

        var str = builder.ToString();
    }
}
