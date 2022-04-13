using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TaycanLogger;

namespace TaycanLogger
{
  public class OBDValue
  {
    private OBDCommand m_OBDCommand;
    private Func<byte[], double> m_Convert;

    public string Name { get; set; }
    public string Units { get; set; }
    public double Value { get; set; }


    public OBDValue(OBDCommand p_OBDCommand)
    {
      m_OBDCommand = p_OBDCommand;
    }

    public void AddConversionFormula(string p_Formula)
    {
      if (!string.IsNullOrEmpty(p_Formula))
      {
        var v_ConversionParser = new ConversionParser(p_Formula);
        var v_Expression = v_ConversionParser.ParseFormula();
        m_Convert = Expression.Lambda<Func<byte[], double>>(v_Expression, v_ConversionParser.ParameterArray).Compile();
      }
    }

    public string Response
    {
      get
      {
        if (m_Convert is null)
          return m_OBDCommand.CommonResponseString;
        else
          return Value.ToString();
      }
    }

    internal void calcConversion()
    {
      try
      {
        if (m_Convert is not null)
          Value = Math.Round(m_Convert(m_OBDCommand.CommonResponseBytes),6);
      }
      catch (Exception ex)
      {
        Trace.WriteLine($"{Name} conversion error {ex.Message} with {Convert.ToHexString(m_OBDCommand.CommonResponseBytes)} ");
      }
    }
  }



  //public class OBDValue
  //{
  //    DataTable dt;  //todo: in die OBDCommand class?

  //    public string Name
  //    {
  //        get;
  //        set;
  //    }
  //    public string ConversionFormula;

  //    public string units;
  //    OBDCommand cmd;
  //    public OBDValue(OBDCommand _cmd)
  //    {

  //        cmd = _cmd;
  //        dt = new DataTable();
  //    }
  //    public string Response
  //    {
  //        get
  //        {
  //            if (ConversionFormula == "")
  //                return cmd.CommonResponseString;
  //            else
  //                return Value.ToString();
  //        }
  //    }
  //    public double Value
  //    {
  //        get;
  //        set;
  //    }

  //    internal void calcConversion()
  //    {
  //        string conversion = ConversionFormula;

  //        if (String.IsNullOrEmpty(ConversionFormula)) return;

  //        try
  //        {
  //            var b = cmd.CommonResponseBytes;
  //            for (int i = b.Length - 1; i >= 0; i--)
  //            {
  //                if (!conversion.Contains("B")) break;

  //                if (conversion.Contains($"B{i}"))
  //                {
  //                    conversion = conversion.Replace($"B{i}", b[i].ToString());
  //                }
  //            }
  //        }
  //        catch
  //        {
  //            Trace.WriteLine($"{Name} did not init conversion '{conversion}' error with '{cmd.CommonResponseString}'");
  //        }

  //        try
  //        {
  //            Value = double.Parse(dt.Compute(conversion, null).ToString());
  //        }
  //        catch (Exception ex)
  //        {

  //            Trace.WriteLine($"{Name} conversion '{conversion}' error {ex.Message} with {Convert.ToHexString(cmd.CommonResponseBytes)} ");
  //        }
  //    }
  //}
}