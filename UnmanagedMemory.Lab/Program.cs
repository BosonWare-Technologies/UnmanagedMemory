using UnmanagedMemory;
using UnmanagedMemory.UnmanagedTypes;

{
    using var nativeBuilder = new NativeStringBuilder();

    nativeBuilder
        .Append("Hello")
        .Append(',')
        .Append(" ")
        .Append("World")
        .Append('!');
    
    var txt = nativeBuilder.ToString();

    Console.Read();
}

using var array = new UnsafeMemory<int>(10);

array.AsSpan().Fill(25);

array[9] = 17;

Console.WriteLine($"({array.Length}) [{string.Join(", ", array)}]");

array.Resize(20);

array.AsSpan(10).Fill(42);

array[19] = 100;

Console.WriteLine($"({array.Length}) [{string.Join(", ", array)}]");

Console.Read();
