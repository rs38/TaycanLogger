using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TaycanLogger;

namespace TaycanLogger
{

    public class OBDCommand : IDisposable
    {
        IOBDDevice runner;
        public List<OBDValue> Values;
        public bool HasSingleValue => Values.Count == 1;
        public string send;
        public int skipCount;
        public string header;
        char[] charsToTrim = { '\r', ' ', '>', '\0' };

        public bool IsSkipped(uint i) => (i % skipCount != 0);
        internal string CommonResponseString;
        internal byte[] CommonResponseBytes
        {
            get;
            set;
        }
        public OBDCommand(IOBDDevice run)
        {
            CommonResponseString = "";
            runner = run;
        }
        public bool IsValidResponse()
        {
            bool valid = (!CommonResponseString.Contains("NO DATA") && !CommonResponseString.StartsWith("7F") && !CommonResponseString.Contains("STOPPED"));
            Trace.WriteLine($":Resp valid:{valid},");
            return valid;
        }

        internal async Task DoExecAsync()
        {
            if (!String.IsNullOrEmpty(header))
                await runner.WriteReadAsync(header);

            CommonResponseString = encodeRawAnswer(await runner.WriteReadAsync(send));
            CommonResponseBytes = Convert.FromHexString(CommonResponseString);

            foreach (var value in Values)
            {
                value.calcConversion();
            }
        }

        public string encodeRawAnswer(string a)
        {
            if (a.Contains(":"))
            {
                var sb = new StringBuilder();

                var lines = a.Split(':');
                for (int i = 1; i < lines.Length; i++)
                {
                    int cr = lines[i].IndexOf("\r\n");
                    sb.Append(lines[i].Substring(0, cr > 0 ? cr : lines[i].Length).Trim(charsToTrim));
                }
                a = sb.ToString();
            }
            return a.Replace(" ", "");
        }

        bool isvalid(string h)
        {
            if (BigInteger.TryParse(h, System.Globalization.NumberStyles.HexNumber, null, out var myInt) &&
                 (h.Length % 2 == 0)) //valid HEX String?
            {
                CommonResponseString = h;
            }
            else
            {
                CommonResponseString = h;
            }
            return true;
        }
        void IDisposable.Dispose() => runner.Dispose();
    }
}

public class OBDValue
{
    DataTable dt;
    public string name;
    public string ConversionFormula;
    public string units;
    OBDCommand cmd;
    public OBDValue(OBDCommand _cmd)
    {
        cmd = _cmd;
        dt = new DataTable();
    }
    public double Value
    {
        get;
        set;
    }
    public string Response
    {
        get => ConversionFormula == "" ? cmd.CommonResponseString : Value.ToString();
    }
    internal void calcConversion()
    {
        string conversion = ConversionFormula;

        if (String.IsNullOrEmpty(ConversionFormula)) return;

        try
        {
            int i = 0;
            foreach (var b in cmd.CommonResponseBytes)
            {
                if (!conversion.Contains("B")) break;

                if (conversion.Contains($"B{i}"))
                {
                    conversion = conversion.Replace($"B{i}", b.ToString());
                }
                i++;
            }
        }
        catch   {
            Trace.WriteLine($"{name} did not init conversion '{conversion}' error with '{cmd.CommonResponseString}'");
        }

        try
        {
            Value = double.Parse(dt.Compute(conversion, null).ToString());
        }
        catch  {
            Trace.WriteLine($"{name} conversion '{conversion}' error with {Convert.ToHexString(cmd.CommonResponseBytes)} ");
        }
    }
}
