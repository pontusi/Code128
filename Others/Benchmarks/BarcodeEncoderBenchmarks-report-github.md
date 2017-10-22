``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i5-2500 CPU 3.30GHz (Sandy Bridge), ProcessorCount=4
Frequency=3215352 Hz, Resolution=311.0079 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2115.0
  DefaultJob : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2115.0


```
 |         Method |       Mean |      Error |     StdDev |
 |--------------- |-----------:|-----------:|-----------:|
 | NumericBarcode |   2.665 us |  0.1008 us |  0.2971 us |
 |    TextBarcode |   2.615 us |  0.0881 us |  0.2584 us |
 | CodesetChanges |   4.374 us |  0.1332 us |  0.3887 us |
 | VeryLongString | 560.238 us | 17.7349 us | 52.2917 us |
