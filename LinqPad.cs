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
	class LinqPad
	{

		async Task Main()
		{
			using (var executor = new OBDSession("ink"))
			{
				var sw = new Stopwatch();
				sw.Start();
				Console.WriteLine("go!");
				UInt32 max = 10_100;
				UInt32 errorCounter = 0;
				uint lineNr = 0;
				//using var FileWriterRaw = new StreamWriter(@$"c:\temp\OBD Taycan {DateTime.Now:yyMMddHHmmssf} Raw.csv");
				using (var FileWriter = new StreamWriter(@$"c:\temp\OBD Taycan {DateTime.Now:yyMMddHHmmssf}.csv"))
				{

					String.Join(";", executor.cmds.Select(c => c.name)).Dump();
					FileWriter.WriteLine("time," + String.Join(",", executor.cmds.Select(c => c.name)));
					do
					{
						lineNr++;
						foreach (var cmd in executor.cmds)
						{
							if (!cmd.IsSkipped(lineNr))
							{

								await cmd.DoExec();
								if (!cmd.IsValidResponse)
								{
									errorCounter++;
									//	cmd.Response.Dump("Err:");
								}
								//Console.Write($"{cmd.name}:{cmd.ResponseValue} {cmd.units}, ");
							}
						}
						var LineString = $"{DateTime.Now:HH:mm:ss.ff},{ String.Join(",", executor.cmds.Select(c => c.Response))}";
						Console.WriteLine(LineString + "," + errorCounter);
						FileWriter.WriteLine(LineString);

						if (lineNr % 100 == 0)
						{
							Console.WriteLine(sw.ElapsedMilliseconds.ToString("0000ms|") + (errorCounter));
							FileWriter.Flush();
						}
					} while (lineNr < max);
					"okay".Dump();
					(sw.ElapsedMilliseconds / max).Dump();
					(errorCounter * 1.0F / max).Dump();
					errorCounter.Dump();
				}
			}
		}
	}

	public class OBDSession : IDisposable
	{
		public List<OBDCommand> cmds;
		string Devicename;
		OBDDevice runner;

		string[] initSequence;

		public OBDSession(string devicename)
		{
			cmds = new List<OBDCommand>();
			Devicename = devicename;
			runner = new OBDDevice(devicename);
			if (!runner.init()) throw new NotSupportedException();
			cmds = initConfig();
			runner.writeAll(initSequence);
		}

		void IDisposable.Dispose()
		{
			((IDisposable)runner).Dispose();
		}

		List<OBDCommand> initConfig()
		{
			var config = XDocument.Load(@"C:\Users\Falco\OneDrive\Ablage\Auto\realdash\RealDash-extras\OBD2\obd2_Taycan.xml");
			//var config = XDocument.Load(@"C:\Users\Falco\OneDrive\Ablage\Auto\realdash\RealDash-extras\OBD2\realdash_obd.xml");
			var init = config.Elements().Elements("init");
			initSequence = init.Elements().Attributes("send").Select(s => s.Value).ToArray();

			foreach (var cmd in config.Elements().Elements("rotation").Elements("command"))
			{
				var c = new OBDCommand(runner)
				{
					send = cmd.Attribute("send").Value,
					skipCount = int.Parse(cmd.Attribute("skipCount").Value) + 1,
					header = cmd.Attribute("header")?.Value,
					headerResp = cmd.Attribute("headerresp")?.Value,
					name = cmd.Attribute("name")?.Value ?? "",
					ConversionFormula = cmd.Attribute("conversion")?.Value?.ToUpper() ?? "",
					units = cmd.Attribute("units")?.Value ?? ""
				};
				cmds.Add(c);
			}
			return cmds;
		}
	}

	public class OBDDevice : IDisposable
	{
		BluetoothClient BTclient;
		BluetoothDeviceInfo device;

		TcpClient NETclient;
		NetworkStream stream;
		byte[] buffer;
		char[] charsToTrim = { '\r', ' ', '>', '\0' };
		string dongleName;

		enum DeviceType { BT, IP, USB }
		DeviceType devicetype;

		public OBDDevice(string _dongleName)
		{
			this.dongleName = _dongleName;
			buffer = new byte[80];
		}

		public bool init()
		{
			if (dongleName.Contains("192"))
			{
				devicetype = DeviceType.IP;
				return initIP(dongleName);
			}
			else
			{
				devicetype = DeviceType.BT;
				BTclient = new BluetoothClient();
				device = getPairedAndroidDongle(dongleName);
				return initBT();
			}
		}



		private void BluetoothClientConnectCallback(IAsyncResult ar)
		{
			Console.WriteLine(ar.ToString());
		}

		bool initIP(string ip)
		{
			try
			{
				TcpClient client = new TcpClient(ip, 35000);
				NetworkStream stream = client.GetStream();
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		bool initBT()
		{
			try
			{
				//device.Refresh();
				System.Diagnostics.Debug.WriteLine(device.Authenticated);
				if (!BTclient.Connected)
					BTclient.Connect(device.DeviceAddress, BluetoothService.SerialPort);


				stream = BTclient.GetStream();
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}



		public async Task<string> WriteReadAsync(string str)
		{
			await writeAsync(str);
			//	Console.Write(str);
			var temp = await readAsync();
			//	Console.Write(temp);
			return temp;
		}

		public async Task writeAsync(string str)
		{
			await stream.WriteAsync(Encoding.ASCII.GetBytes(str + '\r'), 0, str.Length + 1);
			// stream.FlushAsync();
		}

		public void writeAll(string[] array)
		{
			foreach (var str in array)
			{
				Console.Write(WriteReadAsync(str).Result + "|");
			}
		}

		public async Task<string> readAsync()
		{
			string answer = "";
			do
			{
				var x = stream.ReadAsync(buffer, 0, 40);
				answer += System.Text.Encoding.UTF8.GetString(buffer, 0, x.Result);
			} while (!answer.Contains('>'));

			return answer.Trim(charsToTrim); //hack
		}

		static void getpairedLE()
		{
			foreach (var d in Bluetooth.GetPairedDevicesAsync().Result)
			{
				Debug.WriteLine($"{d.Id} {d.Name}");
			}
		}

		BluetoothDeviceInfo getPairedAndroidDongle(string name)
		{
			var bts = BTclient.PairedDevices;
			bts.Select(b => b.DeviceName).Dump();
			var devs = bts.Where(b => b.DeviceName.Contains(name));//.Where(b => b.DeviceAddress.ToString()=="asd");
			var dev = devs.FirstOrDefault();
			//dev.Dump();
			if (dev == null)
				//dev = discover(name);


				if (!dev.Authenticated)
				{
					BluetoothSecurity.PairRequest(dev.DeviceAddress, "1234");
				}
			return dev;
		}

		BluetoothDeviceInfo discover(string name)
		{
			foreach (var dev in BTclient.DiscoverDevices())
			{
				if (dev.DeviceName.Contains(name))
				{
					device = dev;
					break;
				}
			}
			return device;
		}

		public void Dispose()
		{
			stream.Close();
			if (devicetype == DeviceType.BT)
				BTclient.Close();
		}
	}

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

