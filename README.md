# UnmanagedMemory

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/UnmanagedMemory.svg)](https://www.nuget.org/packages/UnmanagedMemory)

A .NET library for safe and efficient allocation and management of unmanaged memory, with a modern, idiomatic API. Designed for high-performance scenarios where direct memory control is required.

---

## Features

- **Safe Unmanaged Memory Allocation**: Allocate, free, and work with unmanaged memory using familiar .NET patterns.
- **Span<T> Support**: Expose unmanaged memory as `Span<T>` for fast, safe access.
- **LINQ Integration**: Use LINQ queries directly on unmanaged memory buffers.
- **Disposal Pattern**: Implements `IDisposable` for deterministic cleanup.
- **Unsafe Operations**: Allows advanced scenarios with pointers when needed.
- **.NET Standard 2.1+**: Broad compatibility across .NET platforms.

---

## Getting Started

### Installation

Install via NuGet:

```sh
dotnet add package UnmanagedMemory
```

---

### Basic Usage

```csharp
using UnmanagedMemory;

// Allocate unmanaged memory for 10 integers
using var memory = new UnsafeMemory<int>(10);

// Fill the memory with a value
memory.AsSpan().Fill(25);

// Set a specific element
memory[9] = 20;

// LINQ query on the buffer
var filtered = memory.Where(x => x > 20);
Console.WriteLine($"[{string.Join(", ", filtered)}]");

// Access raw pointer (unsafe context)
unsafe
{
    var ptr = memory.AsUnsafePointer();
}
```

---

## API Overview

### `UnsafeMemory<T>`

A disposable wrapper for a block of unmanaged memory.

- `new UnsafeMemory<T>(int length)`: Allocates memory for `length` elements.
- `Span<T> AsSpan()`: Access the memory as a span.
- `T this[int index]`: Indexer for element access.
- Implements `IEnumerable<T>` for LINQ support.

### `MemoryUtils`

Static helpers for raw memory allocation.

- `IntPtr Malloc(int bytes)`: Allocates a block of unmanaged memory.
- `void Free(ref IntPtr ptr)`: Frees unmanaged memory and nulls the pointer.

---

## Example

```csharp
using var memory = new UnsafeMemory<float>(100);
memory.AsSpan().Fill(3.14f);

foreach (var value in memory.Where(x => x > 3.0f))
    Console.WriteLine(value);
```

---

## Requirements

- .NET Standard 2.1 or later (.NET Core 3.0+, .NET 5+, .NET 6+, .NET 7+, .NET 8+, .NET 9+)

---

## License

MIT © [BosonWare, Technologies](https://github.com/BosonWare-Technologies/UnmanagedMemory)

---

## Links

- [GitHub Repository](https://github.com/BosonWare-Technologies/UnmanagedMemory)
- [NuGet Package](https://www.nuget.org/packages/UnmanagedMemory)

---

## Release Notes

Initial Release.
