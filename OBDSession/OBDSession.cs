
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
        public string Devicename;
        OBDDevice myDevice;

        string[] initSequence;

        string configFilename;

        public OBDSession(string configfile, string devicename)
        {
            myDevice = new OBDDevice();
            configFilename = configfile;
            Devicename = devicename;

        }

        public async Task<bool> InitDevice()
        {
            if (!hasValidConfig())
            {
                Trace.WriteLine($"config data {configFilename} not valid or found");

                return false;
            }

            if (!myDevice.init(Devicename))
            {
                Trace.WriteLine($"Adapter {Devicename} not found");
                return false;
            }
            await myDevice.writeAllAsync(initSequence);
            return true;
        }


        public List<string> GetPairedDevices() => myDevice.GetPairedDevices();


        public bool hasValidConfig() => readConfig();

        bool readConfig()
        {
            cmds = new List<OBDCommand>();
            try
            {
                var config = XDocument.Load(configFilename);
                var init = config.Elements().Elements("init");
                initSequence = init.Elements().Attributes("send").Select(s => s.Value).ToArray();

                foreach (var cmd in config.Elements().Elements("rotation").Elements("command"))
                {
                    OBDCommand c;
                    if ((cmd.Attribute("units") == null) && (cmd.Descendants("value").Count() > 0))
                    {
                        c = new OBDCommand(myDevice)
                        {
                            send = cmd.Attribute("send").Value,
                            skipCount = int.Parse(cmd.Attribute("skipCount").Value) + 1,
                            header = cmd.Attribute("header")?.Value
                            // headerResp = cmd.Attribute("headerresp")?.Value,
                            
                        };
                        c.Values = new List<OBDValue>();
                        foreach (var element in (cmd.Descendants("value")))
                        {
                            
                            var s = new OBDValue(c)
                            {
                                name = element.Attribute("name")?.Value ?? "",
                                ConversionFormula = element.Attribute("conversion")?.Value?.ToUpper() ?? "",
                                units = element.Attribute("units")?.Value ?? ""
                            };
                            c.Values.Add(s);
                        }
                    }
                    else
                    {
                        c = new OBDCommand(myDevice)
                        {
                            send = cmd.Attribute("send").Value,
                            skipCount = int.Parse(cmd.Attribute("skipCount").Value) + 1,
                            header = cmd.Attribute("header")?.Value,
                        };
                        c.Values = new List<OBDValue>();
                        var s = new OBDValue(c)
                        {
                            name = cmd.Attribute("name")?.Value ?? "",
                            ConversionFormula = cmd.Attribute("conversion")?.Value?.ToUpper() ?? "",
                            units = cmd.Attribute("units")?.Value ?? ""
                        };
                        c.Values.Add(s);
                    }
                    cmds.Add(c);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        void IDisposable.Dispose()
        {
            ((IDisposable)myDevice).Dispose();
        }

        public async Task DoLogAsync(string devicename, IProgress<OBDCommandViewModel> progress, CancellationToken token)
        {
            var progressData = new OBDCommandViewModel();

            var sw = new Stopwatch();
            sw.Start();
            Console.WriteLine("go!");
            UInt32 errorCounter = 0;
            uint lineNr = 0;

            //using var FileWriterRaw = new StreamWriter(@$"c:\temp\OBD Taycan {DateTime.Now:yyMMddHHmmssf} Raw.csv");
            using (var FileWriter = new StreamWriter(@$"c:\temp\OBD Taycan {DateTime.Now:yyMMddHHmmssf}.csv"))
            {
                var OBDvalueNames = from cmd in cmds
                                from Values in cmd.Values
                                select Values.name;

                //LINQ expression syntax alternative:
               var OBDValueValues = cmds.SelectMany(values => values.Values).Select(v => v.Value.ToString());

                String.Join(";",OBDvalueNames).Dump();
                Trace.WriteLine("now loggong: "+ String.Join(",", OBDvalueNames));
                FileWriter.WriteLine("time," + String.Join(",", OBDvalueNames));
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
                                
                            }
                            //Console.Write($"{cmd.name}:{cmd.ResponseValue} {cmd.units}, ");
                        }
                    }
                    var LineString = $"{DateTime.Now:HH:mm:ss.ff},{ String.Join(",", OBDValueValues)}";
                    progressData.logline = LineString + "," + errorCounter;
                    progressData.DataList = cmds.SelectMany(values => values.Values).ToList();
                    progress.Report(progressData);
                    FileWriter.WriteLine(LineString);

                    if (lineNr % 100 == 0)
                    {
                        Console.WriteLine(sw.ElapsedMilliseconds.ToString("0000ms|") + (errorCounter));
                        FileWriter.Flush();
                    }
                    if (token.IsCancellationRequested) break;
                } while (true);
                progressData.logline = $"ms/line: {sw.ElapsedMilliseconds / lineNr} ErrQ: {errorCounter * 1.0F / lineNr} ErrSum{errorCounter}";
                // progress.Report(progressData);

            }
        }
    }
}

