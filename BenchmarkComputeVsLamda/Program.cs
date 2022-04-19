//|             Method |       Mean |     Error |    StdDev |  Ratio | RatioSD |
//|------------------- |-----------:|----------:|----------:|-------:|--------:|
//|             Direct |   1.130 us | 0.0226 us | 0.0309 us |   1.00 |    0.00 |
//|          DataTable | 309.685 us | 6.1421 us | 9.5625 us | 274.71 |   12.32 |
//| ExpressionCompiled |   5.823 us | 0.1139 us | 0.1399 us |   5.15 |    0.18 |

//using decimal in all of the benchmark code, bad performance but not a total desaster

//|             Method |         Mean |       Error |       StdDev |    Ratio | RatioSD |
//|------------------- |-------------:|------------:|-------------:|---------:|--------:|
//|             Direct |     251.7 ns |     5.06 ns |     11.53 ns |     1.00 |    0.00 |
//|          DataTable | 355,006.5 ns | 7,093.46 ns | 13,323.25 ns | 1,426.98 |   77.20 |
//| ExpressionCompiled |     398.6 ns |     7.84 ns |     14.73 ns |     1.60 |    0.10 |

//changing all the code over to double, and using the DataTable version in the original code
//result += double.Parse(m_DataTable.Compute($"({m_Data[i]} + 2) * ({m_Data[i]} - 100)", null).ToString());
//result is a performance problem. This might explain, why we have problems with the cancel button.

//changing the parsing over to using convert does not improve performance a bit
//result += Convert.ToDouble(m_DataTable.Compute($"({m_Data[i]} + 2) * ({m_Data[i]} - 100)", null));

//|             Method |         Mean |       Error |       StdDev |    Ratio | RatioSD |
//|------------------- |-------------:|------------:|-------------:|---------:|--------:|
//|             Direct |     273.4 ns |     5.49 ns |     10.04 ns |     1.00 |    0.00 |
//|          DataTable | 390,762.5 ns | 7,812.96 ns | 13,267.00 ns | 1,435.79 |   94.12 |
//| ExpressionCompiled |     495.8 ns |     9.95 ns |     20.78 ns |     1.83 |    0.12 |

//second run, same result...

//|             Method |         Mean |       Error |      StdDev |    Ratio | RatioSD |
//|------------------- |-------------:|------------:|------------:|---------:|--------:|
//|             Direct |     266.9 ns |     6.68 ns |    18.95 ns |     1.00 |    0.00 |
//|          DataTable | 379,133.3 ns | 5,724.43 ns | 5,074.55 ns | 1,640.46 |   44.43 |
//| ExpressionCompiled |     495.7 ns |     5.67 ns |     5.30 ns |     2.13 |    0.08 |

//we need a better solution for the conversion calculation. We need a parser to generate
//expression functions, compile them as lamda function and store them to be used every time.



//|                  Method |           Mean |          Error |        StdDev |      Ratio |  RatioSD |
//|------------------------ |---------------:|---------------:|--------------:|-----------:|---------:|
//|                  Direct |       1.798 ns |      0.1777 ns |     0.0275 ns |       1.00 |     0.00 |
//|               DataTable |   3,059.478 ns |  1,318.6724 ns |   204.0660 ns |   1,703.49 |   139.94 |
//|      ExpressionCompiled |       4.721 ns |      0.2451 ns |     0.0379 ns |       2.63 |     0.03 |
//| MathExpressionEvaluator | 481,571.021 ns | 44,661.1051 ns | 6,911.3554 ns | 267,983.89 | 7,906.72 |
//|             MathEvalOrg |   1,075.770 ns |     17.2845 ns |     2.6748 ns |     598.55 |     9.50 |


BenchmarkDotNet.Running.BenchmarkRunner.Run<BenchmarkComputeVsLamda>();
//var b = new BenchmarkComputeVsLamda(); b.MathEvalOrg(); // for debug

Console.ReadKey();