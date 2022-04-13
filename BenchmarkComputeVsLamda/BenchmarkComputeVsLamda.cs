using BenchmarkDotNet.Attributes;
using System.Data;
using System.Linq.Expressions;

public class BenchmarkComputeVsLamda
{
  private byte[] m_Data;
  private DataTable m_DataTable;
  private Func<double, double, double> m_CompiledExpression;

  public BenchmarkComputeVsLamda()
  {
    m_DataTable = new DataTable();
    m_Data = new byte[255];
    for (int i = 0; i < m_Data.Length; i++)
      m_Data[i] = (byte)(i + 1);


    ParameterExpression num1 = Expression.Parameter(typeof(double), "num1");
    ConstantExpression con3 = Expression.Constant((double)2);
    var be1 = Expression.Add(num1, con3);

    ParameterExpression num2 = Expression.Parameter(typeof(double), "num2");
    ConstantExpression con4 = Expression.Constant((double)100);
    var be2 = Expression.Subtract(num2, con4);

    ParameterExpression[] parameters = new ParameterExpression[] { num1, num2 };
    var body = Expression.Multiply(be1, be2);

    Expression<Func<double, double, double>> expression = Expression.Lambda<Func<double, double, double>>(body, parameters);
    m_CompiledExpression = expression.Compile();
  }

  [Benchmark(Baseline = true)]
  public void Direct()
  {
    double result = 0;
    for (int i = 0; i < m_Data.Length; i++)
      result += (m_Data[i] + 2) * (m_Data[i] - 100);
  }

  [Benchmark]
  public void DataTable()
  {
    double result = 0;
    for (int i = 0; i < m_Data.Length; i++)
      result += Convert.ToDouble(m_DataTable.Compute($"({m_Data[i]} + 2) * ({m_Data[i]} - 100)", null));
  }

  [Benchmark]
  public void ExpressionCompiled()
  {
    double result = 0;
    for (int i = 0; i < m_Data.Length; i++)
      result += m_CompiledExpression(m_Data[i], m_Data[i]);
  }

}