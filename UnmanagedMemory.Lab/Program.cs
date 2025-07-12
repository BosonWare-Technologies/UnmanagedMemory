using UnmanagedMemory;
using UnmanagedMemory.IO;

using var array = new UnsafeMemory<int>(10);

array.AsSpan().Fill(25);

array[9] = 17;

Console.WriteLine($"({array.Length}) [{string.Join(", ", array)}]");

array.Expand(20);

array.AsSpan(10).Fill(42);

array[19] = 100;

Console.WriteLine($"({array.Length}) [{string.Join(", ", array)}]");

array.GetPointerWrapper()[3] = Random.Shared.Next();

Console.Read();
