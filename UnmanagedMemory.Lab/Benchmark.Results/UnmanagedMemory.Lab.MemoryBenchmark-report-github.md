| Method                     | Length | Mean     | Error     | StdDev    | Allocated |
|--------------------------- |------- |---------:|----------:|----------:|----------:|
| ManagedWithSpan            | 10000  | 4.827 μs | 0.1558 μs | 0.4594 μs |   10024 B |
| UnmanagedWithSpan          | 10000  | 3.241 μs | 0.0238 μs | 0.0198 μs |      32 B |
| PooledWithSpan             | 10000  | 2.924 μs | 0.0209 μs | 0.0185 μs |         - |
| PooledWithSpan_ClearsArray | 10000  | 3.021 μs | 0.0202 μs | 0.0179 μs |         - |
