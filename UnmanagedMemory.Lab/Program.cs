using BenchmarkDotNet.Running;
using UnmanagedMemory;
using UnmanagedMemory.Lab;

BenchmarkRunner.Run<MemoryBenchmark>();

// using var memory = Enumerable
//     .Range(0, 25)
//     .ToUnsafeMemory();

// Console.WriteLine($"[{string.Join(", ", memory)}]");

// Console.Read();
