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

	public class OBDSession : IDisposable
	{
		public List<OBDCommand> cmds;
		string Devicename;
		OBDDevice myDevice;

		string[] initSequence;

		public OBDSession()
        {
			initCMDsWithConfig();
			myDevice = new OBDDevice();
		}

        public void InitDevice(string devicename)
        {
            Devicename = devicename;
       
            if (!myDevice.init(devicename)) throw new NotSupportedException("Adapter not found");
           
            myDevice.writeAll(initSequence);
        }

        public List<string> GetPairedDevices() => myDevice.GetPairedDevices();
		void initCMDsWithConfig()
		{
			cmds = new List<OBDCommand>();
			var config = XDocument.Load(@"C:\Users\Falco\OneDrive\Ablage\Auto\realdash\RealDash-extras\OBD2\obd2_Taycan.xml");
			//var config = XDocument.Load(@"C:\Users\Falco\OneDrive\Ablage\Auto\realdash\RealDash-extras\OBD2\realdash_obd.xml");
			var init = config.Elements().Elements("init");
			initSequence = init.Elements().Attributes("send").Select(s => s.Value).ToArray();

			foreach (var cmd in config.Elements().Elements("rotation").Elements("command"))
			{
				var c = new OBDCommand(myDevice)
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
		}
		void IDisposable.Dispose()
		{
			((IDisposable)myDevice).Dispose();
		}
	}
}

