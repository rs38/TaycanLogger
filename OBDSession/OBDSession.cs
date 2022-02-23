﻿
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
           
            myDevice = new OBDDevice();

        }

        async Task InitDevice(string devicename)
        {
            Devicename = devicename;
            if (!myDevice.init(devicename)) throw new NotSupportedException("Adapter not found");
            await myDevice.writeAll(initSequence);
        }

        public List<string> GetPairedDevices() => myDevice.GetPairedDevices();
        void initCMDsWithConfig()
        {
            cmds = new List<OBDCommand>();
            var config = XDocument.Load(@"C:\Users\Falco\OneDrive\Ablage\Auto\realdash\RealDash-extras\OBD2\obd2_GW.xml");
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

        public async Task DoLogAsync(string devicename, IProgress<OBDCommandViewModel> progress, CancellationToken token)
        {
           
            initCMDsWithConfig();
            await InitDevice(devicename);
            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("go!");
            UInt32 max = 10_100;
            UInt32 errorCounter = 0;
            uint lineNr = 0;
            var progressData = new OBDCommandViewModel();
            //using var FileWriterRaw = new StreamWriter(@$"c:\temp\OBD Taycan {DateTime.Now:yyMMddHHmmssf} Raw.csv");
            using (var FileWriter = new StreamWriter(@$"c:\temp\OBD Taycan {DateTime.Now:yyMMddHHmmssf}.csv"))
            {
                String.Join(";", cmds.Select(c => c.name)).Dump();
                FileWriter.WriteLine("time," + String.Join(",", cmds.Select(c => c.name)));
                do
                {
                    lineNr++;
                    foreach (var cmd in cmds)
                    {
                        if (!cmd.IsSkipped(lineNr))
                        {
                            await cmd.DoExecAsync();
                            if (!cmd.IsValidResponse())
                            {
                                errorCounter++;
                                //	cmd.Response.Dump("Err:");
                            }
                            //Console.Write($"{cmd.name}:{cmd.ResponseValue} {cmd.units}, ");
                        }
                    }
                    var LineString = $"{DateTime.Now:HH:mm:ss.ff},{ String.Join(",", cmds.Select(c => c.Response))}";
                    progressData.logline = LineString + "," + errorCounter;
                    progressData.DataList = cmds;
                    progress.Report(progressData);
                    FileWriter.WriteLine(LineString);

                    if (lineNr % 100 == 0)
                    {
                        Console.WriteLine(sw.ElapsedMilliseconds.ToString("0000ms|") + (errorCounter));
                        FileWriter.Flush();
                    }
                    if (token.IsCancellationRequested) break;
                } while (lineNr < max);
                progressData.logline = $"ms/line: {sw.ElapsedMilliseconds / lineNr} ErrQ: {errorCounter * 1.0F / lineNr} ErrSum{errorCounter}";
               // progress.Report(progressData);
                
            }
        }
    }
}
