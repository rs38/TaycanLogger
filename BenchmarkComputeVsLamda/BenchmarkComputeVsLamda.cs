using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using SimpleExpressionEvaluator;
using System.Data;
using System.Linq.Expressions;


[SimpleJob(RunStrategy.Throughput, launchCount: 2, warmupCount: 1, targetCount: 2, id: "FastAndDirtyJob")]
public class BenchmarkComputeVsLamda
{
    private byte[] m_Data;
    private DataTable m_DataTable;
    private Func<double, double, double> m_CompiledExpression;
    org.matheval.Expression expOrg;
    ExpressionEvaluator engine;
    string Conversion;

    public BenchmarkComputeVsLamda()
    {
        m_DataTable = new DataTable();
        m_Data = new byte[2];
        for (int i = 0; i < m_Data.Length; i++)
            m_Data[i] = (byte)(i + 1);

        Conversion = "((((B0*256)+B1)/8)-100)";
        
        expOrg = new org.matheval.Expression(Conversion);


        ParameterExpression num1 = Expression.Parameter(typeof(double), "num1");
        ConstantExpression con3 = Expression.Constant((double)2);
        var be1 = Expression.Add(num1, con3);

        ParameterExpression num2 = Expression.Parameter(typeof(double), "num2");
        ConstantExpression con4 = Expression.Constant((double)100);
        var be2 = Expression.Subtract(num2, con4);

        ParameterExpression[] parameters = new ParameterExpression[] { num1, num2 };
        var body = Expression.Multiply(be1, be2);

        engine = new ExpressionEvaluator();

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
    //as currently used
    public void DataTable()
    {
        double result = 0;
        string conversion = Conversion;

        for (int i = m_Data.Length - 1; i >= 0; i--)
        {
            if (!conversion.Contains("B")) break;

            if (conversion.Contains($"B{i}"))
            {
                conversion = conversion.Replace($"B{i}", m_Data[i].ToString());
            }
        }
        result += Convert.ToDouble(m_DataTable.Compute(conversion, null));
    }

    [Benchmark]
    public void ExpressionCompiled()
    {
        double result = 0;
        for (int i = 0; i < m_Data.Length; i++)
            result += m_CompiledExpression(m_Data[i], m_Data[i]);
    }

    [Benchmark]
    //slow, unhandy
    public void MathExpressionEvaluator()
    {
        double result = 0;
        string conversion = Conversion;

        for (int i = m_Data.Length - 1; i >= 0; i--)
        {
            if (!conversion.Contains("B")) break;

            if (conversion.Contains($"B{i}"))
            {
                conversion = conversion.Replace($"B{i}", m_Data[i].ToString());
            }
        }

        result += Convert.ToDouble(engine.Evaluate(conversion));
    }


    [Benchmark]
    //my new favorite nice assigning of variables and fast!

    public void MathEvalOrg()

    {
        double result = 0;
        for (int i = 0; i < m_Data.Length; i++)
            expOrg.Bind($"B{i}", m_Data[i]);

        result += Convert.ToDouble(expOrg.Eval());
    }

}
