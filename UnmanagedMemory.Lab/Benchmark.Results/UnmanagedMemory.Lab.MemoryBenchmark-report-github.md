```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i5-4590 CPU 3.30GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.305
  [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2


```
| Method            | Length | Mean     | Error     | StdDev    | Allocated |
|------------------ |------- |---------:|----------:|----------:|----------:|
| ManagedWithSpan   | 10000  | 3.902 μs | 0.0687 μs | 0.1186 μs |   10024 B |
| UnmanagedWithSpan | 10000  | 3.220 μs | 0.0111 μs | 0.0103 μs |      32 B |
