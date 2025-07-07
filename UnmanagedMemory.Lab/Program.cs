using UnmanagedMemory;

Console.WriteLine("Hello, World!");

using var memory = new UnsafeMemory<int>(10);

memory.AsSpan().Fill(25);

memory[9] = 20;

var @enum = memory.Where(x => x > 20);

Console.WriteLine($"[{string.Join(", ", @enum)}]");

unsafe {
    var ptr = MemoryUtils.Malloc(512);

    var bytes = new Span<byte>(ptr, 512);

    MemoryUtils.Free(ref ptr);
}

Console.WriteLine($"My Array: [{string.Join(", ", memory.AsSpan().ToArray())}]");

Console.Read();
