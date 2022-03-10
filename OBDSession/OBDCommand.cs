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
        public List<OBDValue> Values
        {
            get; set;
        }
        public bool HasSingleValue => Values.Count == 1;
        public string send;
        public int skipCount;
        public string header;
        char[] charsToTrim = { '\r', ' ', '>', '\0' };
       
        public bool IsSkipped(uint i) => (i % skipCount != 0);
        internal string CommonResponseString
        {
            get;   set;
        }
        internal byte[] CommonResponseBytes
        {
            get;  set;
        }
        public OBDCommand(IOBDDevice run)
        {
            CommonResponseString = "";
            runner = run;
        }
        public bool IsValidResponse()
        {
            bool valid = !CommonResponseString.Contains("DATA") && 
               // !CommonResponseString.EndsWith("V") && 
                !CommonResponseString.StartsWith("7F") &&
                !CommonResponseString.Contains("STOPPED");
            Trace.WriteLine($":Resp valid:{valid},");
            return valid;
        }
        bool IsvalidHex()
        {
            if (BigInteger.TryParse(CommonResponseString, System.Globalization.NumberStyles.HexNumber, null, out _) &&
                 (CommonResponseString.Length % 2 == 0)) //valid HEX String?
            {
                Trace.Write("corrent Response: " + CommonResponseString);
                return true;
            }

            Trace.Write("malformed Response: " + CommonResponseString);
            return false;
        }

        internal async Task DoExecAsync()
        {
            if (!String.IsNullOrEmpty(header))
                await runner.WriteReadAsync(header);

            CommonResponseString = encodeRawAnswer(await runner.WriteReadAsync(send));
            if (IsValidResponse() && IsvalidHex())
            {
                try
                {
                    CommonResponseBytes = Convert.FromHexString(CommonResponseString);
                    foreach (var value in Values)
                    {
                        value.calcConversion();
                    }
                }
                catch (Exception ex)
                {
                    Trace.Write("Hex Convert Error: " + ex.Message);
                }
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
                    int cr = lines[i].IndexOf("\r");
                    sb.Append(lines[i].Substring(0, cr > 0 ? cr : lines[i].Length).Trim(charsToTrim));
                }
                a = sb.ToString();
            }
            return a.Replace(" ", "").Trim(charsToTrim);
        }
      
        void IDisposable.Dispose() => runner.Dispose();
    }
}
