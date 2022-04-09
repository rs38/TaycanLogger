
using InTheHand.Net.Sockets;
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
    public IOBDDevice myDevice;
    public Func<string> RawFilename = null;

    string[] initSequence;

    string configFileContent;

    public OBDSession(string configContent, string devicename)
    {
      configFileContent = configContent;
      Devicename = devicename;
    }

    public async Task<bool> InitDevice()
    {
      if (Devicename == "RawDevice")
      {
        OBDRawDevice v_OBDRawDevice = new OBDRawDevice();
        v_OBDRawDevice.Filename = RawFilename;
        myDevice = v_OBDRawDevice;
      }
      else
        myDevice = new OBDDevice();
      if (!hasValidConfig())
      {
        //Trace.WriteLine($"config data not valid or found");

        return false;
      }

      if (!myDevice.init(Devicename))
      {
        //Trace.WriteLine($"Adapter {Devicename} not found");
        return false;
      }
      await myDevice.writeAllAsync(initSequence);
      return true;
    }

    public List<string> GetPairedDevices() => new BluetoothClient().PairedDevices.Select<BluetoothDeviceInfo, string>(bdi => bdi.DeviceName).Append("RawDevice").ToList();

    public bool hasValidConfig() => readConfig();

    bool readConfig()
    {
      cmds = new List<OBDCommand>();
      try
      {
        var config = XDocument.Parse(configFileContent);
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
              header = cmd.Attribute("header")?.Value?.ToUpper()
            };
            c.Values = new List<OBDValue>();
            foreach (var element in (cmd.Descendants("value")))
            {

              var s = new OBDValue(c)
              {
                Name = element.Attribute("name")?.Value ?? "",
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
              Name = cmd.Attribute("name")?.Value ?? "",
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
        //Trace.WriteLine(ex.Message);
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
      using (var FileWriter = new StreamWriter(@$"{System.Environment.CurrentDirectory}\TayCANLogger {DateTime.Now:yyMMddHHmmssf}.csv"))
      {
        var OBDvalueNames = from cmd in cmds
                            from Values in cmd.Values
                            select Values.Name;

        //LINQ expression syntax alternative:
        var OBDValueValues = cmds.SelectMany(values => values.Values).Select(v => v.Value.ToString());

        String.Join(";", OBDvalueNames).Dump();
        //Trace.WriteLine("now loggong: "+ String.Join(",", OBDvalueNames));
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
                errorCounter++;
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

