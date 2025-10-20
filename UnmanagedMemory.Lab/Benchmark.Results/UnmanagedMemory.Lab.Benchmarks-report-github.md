```

BenchmarkDotNet v0.15.2, Windows 10 (10.0.19045.6456/22H2/2022Update)
Intel Core i5-4590 CPU 3.30GHz (Haswell), 1 CPU, 4 logical and 4 physical cores
.NET SDK 9.0.305
  [Host]     : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.9 (9.0.925.41916), X64 RyuJIT AVX2


```
| Method               | Mean     | Error     | StdDev    | Median   | Allocated |
|--------------------- |---------:|----------:|----------:|---------:|----------:|
| NativeStringBuilder  | 3.360 μs | 0.0105 μs | 0.0098 μs | 3.359 μs |      96 B |
| ManagedStringBuilder | 2.100 μs | 0.0418 μs | 0.1173 μs | 2.067 μs |    4536 B |
