using InTheHand.Bluetooth;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TaycanLogger
{

	public class OBDCommand : IDisposable
	{
		DataTable dt;
		OBDDevice runner;

		public OBDCommand(OBDDevice run)
		{
			ResponseString = "";
			runner = run;
			dt = new DataTable();
		}

		//TODO	public CommandValue[] values;
		public string send;
		public int skipCount;
		public string name;
		public string header;
		public string headerResp;
		public string ConversionFormula;
		public string units;
		public bool IsSkipped(uint i) => (i % skipCount != 0);
		string ResponseString;
		decimal ResponseValue
		{
			get;
			set;
		}
		public string Response
		{
			get => ConversionFormula == "" ? ResponseString : ResponseValue.ToString();
			private set
			{
				//if (value.Contains("27D6"))		Console.WriteLine(value);
				if (BigInteger.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var myInt) &&
					!(ConversionFormula == "") &&
					(value.Length % 2 == 0)) //valid HEX String?
				{
					ResponseValue = calcConversion(Convert.FromHexString(value));
					ResponseString = value;
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
			catch (Exception ex)
			{
				throw new ArgumentException(ResponseString);
			}
			decimal temp;
			try
			{
				temp = decimal.Parse(dt.Compute(conversion, null).ToString());
			}
			catch (Exception ex)
			{
				throw new IndexOutOfRangeException(Convert.ToHexString(bytes) + name);

			}
			return temp;
		}

		public bool IsValidResponse => !ResponseString.Contains("DATA");
		internal async Task DoExec()
		{
			if (!String.IsNullOrEmpty(header))
				Task.WaitAll(runner.WriteReadAsync(header));
			if (!String.IsNullOrEmpty(headerResp))
				Task.WaitAll(runner.WriteReadAsync(headerResp));
			var x = await runner.WriteReadAsync(send);
			if (x.Contains(":"))
				x = x.Split(':')[1].Trim();
			Response = x;

		}

		void IDisposable.Dispose() => runner.Dispose();
	}
}

