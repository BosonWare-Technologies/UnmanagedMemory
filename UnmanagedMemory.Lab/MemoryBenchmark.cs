using System.Buffers;
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
    public void ManagedWithSpan()
    {
        var array = new byte[Length];
        
        foreach (ref var value in array.AsSpan()) {
            value = 5;
        }
    }

    [Benchmark]
    public void UnmanagedWithSpan()
    {
        using var array = new UnsafeMemory<byte>(Length);

        foreach (ref var value in array.AsSpan()) {
            value = 5;
        }
    }
    
    [Benchmark]
    public void PooledWithSpan()
    {
        var array = ArrayPool<byte>.Shared.Rent(Length);

        try {
            var span = array.AsSpan(0, Length);

            foreach (ref var value in span) {
                value = 5;
            }
        }
        finally {
            ArrayPool<byte>.Shared.Return(array);
        }
    }

    [Benchmark]
    public void PooledWithSpan_ClearsArray()
    {
        var array = ArrayPool<byte>.Shared.Rent(Length);

        try {
            var span = array.AsSpan(0, Length);

            foreach (ref var value in span) {
                value = 5;
            }
            
            // clear the array's contents.
            span.Clear();
        }
        finally {
            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
