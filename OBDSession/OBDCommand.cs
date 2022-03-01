using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace TaycanLogger
{

    public class OBDCommand : OBDBase, IDisposable
    {
        DataTable dt;
        IOBDDevice runner;

        public List<OBDBase> Subcommands;
        public bool HasSubCommands;
        public string send;
        public int skipCount;
        public string header;
        char[] charsToTrim = { '\r', ' ', '>', '\0' };
        //public string headerResp;

        public bool IsSkipped(uint i) => (i % skipCount != 0);
        string ResponseString;
        public double ResponseValue
        {
            get;
            set;
        }
        public OBDCommand(IOBDDevice run)
        {
            ResponseString = "";
            runner = run;
            dt = new DataTable();
        }

        public string Response
        {
            get => ConversionFormula == "" ? ResponseString : ResponseValue.ToString();
            private set
            {
                if (BigInteger.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var myInt) &&
                    !(ConversionFormula == "") &&
                    (value.Length % 2 == 0)) //valid HEX String?
                {
                    ResponseString = value;
                    ResponseValue = calcConversion(Convert.FromHexString(value));
                }
                else
                {
                    ResponseString = value;
                }
            }
        }

        double calcConversion(byte[] bytes)
        {
            string conversion = ConversionFormula;
            try
            {
                int i = 0;
                foreach (var b in bytes)
                {
                    if (conversion.Contains($"B{i}"))
                    {
                        conversion = conversion.Replace($"B{i}", b.ToString());
                    }
                    i++;
                }
            }
            catch
            {
                Trace.WriteLine($"{name} did not init conversion '{conversion}' error with '{ResponseString}'");
            }
            double temp;
            try
            {
                temp = double.Parse(dt.Compute(conversion, null).ToString());
            }
            catch
            {
                Trace.WriteLine($"{name} conversion '{conversion}' error with {Convert.ToHexString(bytes)} ");
                return ResponseValue;
            }
            return temp;
        }

        public bool IsValidResponse()
        {
            bool valid = (!ResponseString.Contains("NO DATA") && !ResponseString.StartsWith("7F") && !ResponseString.Contains("STOPPED"));
            Trace.WriteLine($":Resp valid:{valid},");
            return valid;
        }

        internal async Task DoExecAsync()
        {
            if (!String.IsNullOrEmpty(header))
                await runner.WriteReadAsync(header);

            Response = processRawAnswer(await runner.WriteReadAsync(send));
            ProcessMultipleSubValues();

        }

        private void ProcessMultipleSubValues()
        {
            throw new NotImplementedException();
        }

        public string processRawAnswer(string a)
        {
            if (a.Contains(":"))
            {
                var sb = new StringBuilder();

                var lines = a.Split(':');
                for (int i = 1; i < lines.Length; i++)
                {
                    int cr = lines[i].IndexOf("\r\n");
                    sb.Append(lines[i].Substring(0, cr > 0 ? cr : lines.Length).Trim(charsToTrim));
                }
                a = sb.ToString();
            }

            return a.Replace(" ","");

        }

        void IDisposable.Dispose() => runner.Dispose();
    }
}

public class OBDBase
{
    public string name;
    public string ConversionFormula;
    public string units;
}
