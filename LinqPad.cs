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
}

