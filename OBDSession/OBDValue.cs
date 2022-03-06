using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TaycanLogger;

public class OBDValue
{
    DataTable dt;

    public string name
    {
        get;
        set;
    }
    public string ConversionFormula;
   
    public string units;
    OBDCommand cmd;
    public OBDValue(OBDCommand _cmd)
    {
        cmd = _cmd;
        dt = new DataTable();
    }
    public string Response
    {
        get
        {
            if (ConversionFormula == "")
                return cmd.CommonResponseString;
            else 
                return Value.ToString();
            
        }
    }


    public OBDValue()
    {

    }

    public double Value
    {
        get;
        set;
    }

    internal void calcConversion()
    {
        string conversion = ConversionFormula;

        if (String.IsNullOrEmpty(ConversionFormula)) return;

        try
        {
            var b = cmd.CommonResponseBytes;
            for (int i = b.Length-1; i >= 0; i--)
            {
                if (!conversion.Contains("B")) break;
              
                if (conversion.Contains($"B{i}"))
                {
                    conversion = conversion.Replace($"B{i}", b[i].ToString());
                }
            }
        }
        catch
        {
            Trace.WriteLine($"{name} did not init conversion '{conversion}' error with '{cmd.CommonResponseString}'");
        }

        try
        {
            Value = double.Parse(dt.Compute(conversion, null).ToString());
        }
        catch (Exception ex)
        {

            Trace.WriteLine($"{name} conversion '{conversion}' error {ex.Message} with {Convert.ToHexString(cmd.CommonResponseBytes)} ");
        }
    }
}
