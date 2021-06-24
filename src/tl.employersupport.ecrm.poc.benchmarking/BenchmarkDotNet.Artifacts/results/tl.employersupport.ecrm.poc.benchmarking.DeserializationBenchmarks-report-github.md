``` ini

BenchmarkDotNet=v0.13.0, OS=Windows 10.0.19042.1055 (20H2/October2020Update)
AMD Ryzen 9 3900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.100-preview.5.21302.13
  [Host]     : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT
  DefaultJob : .NET 5.0.7 (5.0.721.25508), X64 RyuJIT


```
|                                       Method |          Mean |      Error |     StdDev |           Min |           Max |
|--------------------------------------------- |--------------:|-----------:|-----------:|--------------:|--------------:|
|                    DeserializeTicketFromJson |  4,877.060 μs | 34.0809 μs | 30.2119 μs |  4,838.992 μs |  4,932.478 μs |
|       DeserializeTicketWithSideloadsFromJson | 11,354.685 μs | 56.1123 μs | 49.7421 μs | 11,260.478 μs | 11,420.259 μs |
|                    BuildTicketFromJsonStream |     86.470 μs |  0.3730 μs |  0.3307 μs |     86.039 μs |     87.042 μs |
| BuildTicketFromTicketWithSideloadsJsonStream |     96.241 μs |  0.4215 μs |  0.3943 μs |     95.556 μs |     96.940 μs |
|              BuildTicketFieldsFromJsonStream |      4.358 μs |  0.0187 μs |  0.0175 μs |      4.325 μs |      4.387 μs |
