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

	public class OBDDevice : IDisposable
	{
		BluetoothClient BTclient;
		BluetoothDeviceInfo device;


        NetworkStream stream;
		byte[] buffer;
		char[] charsToTrim = { '\r', ' ', '>', '\0' };
		string dongleName;

		enum DeviceType { BT, IP, USB }
		DeviceType devicetype;

		public OBDDevice()
		{
			BTclient = new BluetoothClient();
		}

		public bool init(string _dongleName)
		{
			this.dongleName = _dongleName;
			buffer = new byte[80];
				devicetype = DeviceType.BT;
				device = getPairedAndroidDongle(dongleName);
				return initBT();
		}

		private void BluetoothClientConnectCallback(IAsyncResult ar)
		{
			Console.WriteLine(ar.ToString());
		}

		bool initBT()
		{
			try
			{
				//device.Refresh();
				Debug.WriteLine($"device auth:{device.Authenticated}");
				if (!BTclient.Connected)
					BTclient.Connect(device.DeviceAddress, BluetoothService.SerialPort);


				stream = BTclient.GetStream();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
			return true;
		}



		public async Task<string> WriteReadAsync(string str)
		{
			await writeAsync(str);
			var temp = await readAsync();
			Debug.WriteLine($"send {str}:received{temp}");
			
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
				Debug.WriteLine($"paired device:{d.Id} {d.Name}");
			}
		}

		public  List<string> GetPairedDevices() => BTclient.PairedDevices.Select(p => p.DeviceName).ToList();

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
}

