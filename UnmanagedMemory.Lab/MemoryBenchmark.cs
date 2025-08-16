using BenchmarkDotNet.Attributes;
using JetBrains.Annotations;

namespace UnmanagedMemory.Lab;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
[MemoryDiagnoser(false)]
public class MemoryBenchmark
{
    [Params(10000)]
    public int Length { get; set; } = 1000000;

    [Benchmark]
    public void Managed()
    {
        var array = new byte[Length];
        
        var span = new Span<byte>(array);

        for (var i = 0; i < span.Length; i++)
            span[i] = 5;
    }

    [Benchmark]
    public void Unmanaged()
    {
        using var memory = new UnsafeMemory<byte>(Length);

        for (var i = 0; i < memory.Length; i++)
            memory[i] = 5;
    }

    [Benchmark]
    public void UnmanagedWithSpan()
    {
        using var array = new UnsafeMemory<byte>(Length);

        foreach (ref var value in memory.AsSpan()) {
            value = 5;
        }
    }

    [Benchmark]
    public unsafe void Raw()
    {
        var ptr = UnmanagedMemory.Unmanaged.Malloc<byte>(Length);

        try {
            for (var i = 0; i < Length; i++)
                ptr[i] = 5;
        }
        finally {
            UnmanagedMemory.Unmanaged.Free(ref ptr);
        }
    }
}
