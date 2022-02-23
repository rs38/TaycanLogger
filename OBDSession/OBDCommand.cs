using System;
using System.Data;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace TaycanLogger
{

    public class OBDCommand : IDisposable
    {
        DataTable dt;
        OBDDevice runner;

        public string send;
        public int skipCount;
        public string name;
        public string header;
        public string headerResp;
        public string ConversionFormula;
        public string units;
        public bool IsSkipped(uint i) => (i % skipCount != 0);
        string ResponseString;
        public decimal ResponseValue
        {
            get;
            set;
        }
        public OBDCommand(OBDDevice run)
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

        decimal calcConversion(byte[] bytes)
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
                Debug.WriteLine($"{name} did not init conversion '{conversion}' error with '{ResponseString}'" );
            }
            decimal temp;
            try
            {
                temp = decimal.Parse(dt.Compute(conversion, null).ToString());
            }
            catch 
            {
                Debug.WriteLine($"{name} conversion '{conversion}' error with {Convert.ToHexString(bytes)} ");
                return ResponseValue;
            }
            return temp;
        }

        public bool IsValidResponse()
        {
            bool valid = (!ResponseString.Contains("NO DATA") && !ResponseString.StartsWith("7F") && !ResponseString.Contains("STOPPED"));
            Debug.WriteLine($":Resp valid:{valid},");
            return valid;
        }

        internal async Task DoExecAsync()
        {
            if (!String.IsNullOrEmpty(header))
                await runner.WriteReadAsync(header);
            if (!String.IsNullOrEmpty(headerResp))
                await  runner.WriteReadAsync(headerResp);
            var x = await runner.WriteReadAsync(send);
            if (x.Contains(":"))
                x = x.Split(':')[1].Trim();
            Response = x;
        }

        void IDisposable.Dispose() => runner.Dispose();
    }
}

