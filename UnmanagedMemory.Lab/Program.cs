using System.Diagnostics;
using BenchmarkDotNet.Running;
using UnmanagedMemory;
using UnmanagedMemory.Lab;
using UnmanagedMemory.Safety;

MemoryLeakManager.SetHandler(ctx => {
    Console.WriteLine($"[Development] Memory Leak of {ctx.Size} bytes.");
    
    Environment.Exit(1);
});

unsafe {
    MemoryLeakManager.SetHandler(ctx => {
        Unmanaged.Free(ref ctx.Pointer);
        
        Console.WriteLine($"[Production] Memory Leak of {ctx.Size} bytes.");
    });
}

BenchmarkRunner.Run<MemoryBenchmark>();

// using var memory = Enumerable
//     .Range(0, 25)
//     .ToUnsafeMemory();

// Console.WriteLine($"[{string.Join(", ", memory)}]");

// Console.Read();
