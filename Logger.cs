using OBD.NET.Desktop.Communication;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaycanLogger
{

    class Logger
    {
        //todo
        //define conversion+ ID Data in parameter format
        //add SoH
        // "broadcast" ??
        // parse multi-frame data stolpert dabei
        // FileLogger auslagern


        LogLineReadyEventArgs Logline;

        TextWriter FileWriter;
        TextWriter FileWriterRaw;
        SerialConnection conn;
        Dictionary<string, string> mapping;
        Dictionary<string, string> newline;
        Dictionary<string, string> lastline;



        public bool stop = false;

        public bool debug { get;  set; }

        public event EventHandler<LogLineReadyEventArgs> LogLineReady;

        protected virtual void OnLogLineReady(LogLineReadyEventArgs e)
        {
            EventHandler<LogLineReadyEventArgs> handler = LogLineReady;
            handler?.Invoke(this, e);
        }


        public Logger()
        {
            Init();

            Logline = new LogLineReadyEventArgs(DateTime.Now)
            {
                textonly = true,
                LogLine = "nur ein Test\r\n"
            };
        }

        private void InitFiles()
        {
            FileWriter = new StreamWriter($@".\OBDTaycan{DateTime.Now:yyMMddHHmmssf}.csv");
            FileWriterRaw = new StreamWriter($@".\OBDTaycan{DateTime.Now:yyMMddHHmmssf} Raw.csv");
            FileWriterRaw.WriteLine("time;value;unit;cmd;comment");

            var sb = new StringBuilder();

            foreach (var element in mapping)
            {
                lastline[element.Key] = "";
                sb.Append(element.Value + ",");
            }


            FileWriter.WriteLine(sb.ToString());
        }

        public async Task LogfromCOM(string COM)
        {
            using (conn = new OBD.NET.Desktop.Communication.SerialConnection(COM))
            {
                try
                {
                    conn.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (conn.IsOpen)
                {
                    InitFiles();
                    CANInit();

                    conn.DataReceived += readdata;

                    while (!stop)
                    {
                        foreach (var element in mapping)
                        {
                            if (element.Value == "SoC Anzeige" | element.Value == "kwh/100km" | element.Value == "PTC current"
                            | element.Value == "Outside Temp" | element.Value == "Inside Temp" | element.Value == "Kompressor kW" | element.Value == "Dashboard") continue; //hack

                            if (Convert.ToUInt32(element.Key, 16) > 2) //sonst ist es custom
                                await writeCAN("22" + element.Key);
                        }
                        //hack();
                        FileWriter.Flush();
                        FileWriterRaw.Flush();
                    }

                    Thread.Sleep(1000);
                    FileWriter.Close();
                    FileWriterRaw.Close();
                }
            }


        }


        void hack2a()
        {
            return;
            writeCAN("atz");
            writeCAN("atsh17fc00b7");
            writeCAN("atsh17fe00b7");
            writeCAN("atfcsh17fc00b7");
            writeCAN("atfcsd300001");
            writeCAN("atfcsm1");
            writeCAN("22F19E"); //VariantIdentificationAndSelection

        }

        void hack()
        {

            writeCAN("atsh7E0", 40); writeCAN("atcra7E8", 30); //0x01 Motor
            writeCAN("2208D2"); //Soc Anzeige


            writeCAN("atsh742", 40); writeCAN("atcra7AC", 40);//Thermo 0xC5
            writeCAN("22475F"); //heater current
                                //	writeCAN("2227D6",200); //Compress kw Probleme.


            writeCAN("ATSH746", 60); writeCAN("ATCRA7B0", 40); //climate control
                                                               //writeCAN("222609"); // Outside Temp Sensor 62 26 09 28 x/2-50 °C 40 -40?
            writeCAN("222608"); // Outside Temp Calc 62 26 09 23 x/2-50 °C  35
            writeCAN("222613");  // Inside temperature, °C

            /*	writeCAN("ATSH714",40); writeCAN("ATCRA77E",50);  //Kombiinstr,
					writeCAN("220530");
				*/
            writeCAN("ATSH7E5", 40); writeCAN("ATCRA7ED", 50);

        }

        void experiementell()
        {
            //write("22 F446"); //"Außentemperatur"; ID: F446 x-40
            //write("22 0432"); //"Historiendaten 35 Charge_Power_Limit_Time_TEMP ID: 0432 32bit werte x/10 in Sec.

            //write("22 1E32"); // Total accumulated charged and discharge

            //writeCAN("atcra7ac");
            //	writeCAN("atfcsh742");
            //writeCAN("atfcsh742");
            //	writeCAN("atfcsd300001");
            //		writeCAN("atfcsm1");

            writeCAN("220801");  // PTC air heater inside, ampere

            writeCAN("ATSH 17FC007B"); //BMS
            writeCAN("ATCRA17FE007B");

            writeCAN("ATSH17FC00C7"); // HVChargBoost
            writeCAN("ATCRA17FE00C7");



            writeCAN("ATCRA17 FE 00 B7");  // DCDC Basic

            //	charger_coolant_temperature "15E2" Value: 45 
            //17 FE 00 8B Gateway?

        }

        async Task writeCAN(string s, int i = 1)
        {
            char term = '\r';
            var data = Encoding.ASCII.GetBytes(s + term);
            conn.Write(data);

            if (i == 1)
                //Thread.Sleep(90);//
                await Task.Delay(90);
            else
                await Task.Delay(i);
            //Thread.Sleep(i);
        }

        void readdata(object sender, OBD.NET.Common.Communication.EventArgs.DataReceivedEventArgs e)
        {
            string resp = "";
            var s = System.Text.Encoding.Default.GetString(e.Data);


            for (int i = 0; i < e.Count; i++)
            {
                char c = (char)e.Data[i];
                switch (c)
                {
                    case '\r':
                        break;
                    case '>':  //Console.Write("||");
                        break;
                    case '\n':
                    case (char)0x00:
                        break; // ignore
                    default:
                        resp += c;
                        break;
                }
            }

            if (debug)
            {
                FileWriterRaw.Write(resp + "|");
                Logline.LogLine = resp + "|";
                Logline.textonly = true;
                OnLogLineReady(Logline) ;
            }
            if (resp.Contains("0:")) //abgehacktes Multiframe
                resp = resp.Substring(7, resp.Length - 7);
            if (resp.Length > 1 && resp.Substring(0, 2) == "62")
            {
                resp = (resp.Substring(2, resp.Length - 2));
                //Console.WriteLine(result);

                string unit = "";
                int i = 0;
                double f = 0;
                var text = resp.Replace(" ", "");
                var cmd = text.Substring(0, 4);
                if (cmd == "1802" | cmd == "1804") //A
                {
                    var res = text.Substring(4, text.Length - 4); //3 Bytes
                    f = (Convert.ToInt32(res, 16) / 100.0) - 1500;
                    unit = "A Strom";
                }

                if (cmd == "1801" | cmd == "1E3B")
                {
                    var res = text.Substring(4, text.Length - 4); //2 Bytes
                    f = Convert.ToInt32(res, 16) / 10.0;
                    unit = "V HV Spannung";
                }

                if (cmd == "F40D")
                {
                    var res = text.Substring(4, text.Length - 4); //2 Bytes
                    f = Convert.ToInt32(res, 16) * 1.0;
                    unit = "km/h Fzg.geschw.";
                }

                /*
                                if (cmd == "181C" | cmd == "181D")
                                {
                                    var res = text.Substring(4, text.Length - 4); //2 Bytes
                                    f = Convert.ToInt32(res, 16) - 40;
                                    unit = "C Batt Cooling";
                                }

                                if (cmd == "1E10")
                                {
                                    var res = text.Substring(4, text.Length - 4); //1 Byte
                                    f = Convert.ToInt32(res, 16) - 100;
                                    unit = "C Batt Temp";
                                }
                                if (cmd == "2608" | cmd == "2613")
                                {
                                    var res = text.Substring(4, text.Length - 4); //1 Byte
                                    f = Convert.ToInt32(res, 16) / 8.0;
                                    unit = "°C ";
                                }

                                if (cmd == "1E33" | cmd == "1E34")
                                {
                                    var res = text.Substring(4, text.Length - 4); //2 Bytes
                                    f = Convert.ToInt32(res.Substring(0, 4), 16) / 10000.0;
                                    unit = "C Cell Volt";
                                }

                              

                                if (cmd == "1E0E" | cmd == "1E0F") //es kommen zwei werte zurück
                                {
                                    var res = text.Substring(4, text.Length - 4);
                                    f = Convert.ToInt32(res.Substring(2, 2), 16);
                                    //writeLogRaw(f,"#",cmd+"2","Number");

                                    f = Convert.ToUInt16(res.Substring(0, 2), 16) - 100;
                                    unit = "°C";
                                }

                                if (cmd == "1E2D" | cmd == "1E2C") //es kommen zwei werte zurück
                                {
                                    var res = text.Substring(4, text.Length - 4);
                                    f = Convert.ToUInt16(res.Substring(0, 2), 16);
                                    //	writeLogRaw(f, "Soc Batt Cell", cmd, res);

                                    //	f = Convert.ToInt32(res.Substring(2, 2), 16);
                                    unit = "°C Batt Cell";
                                }

                                if (cmd == "028C")
                                {
                                    var res = text.Substring(4, text.Length - 4); //2 Bytes
                                    f = Convert.ToInt32(res, 16) * 1.0;
                                    unit = "% Soc ";
                                }


                                if (cmd == "08D2") //0x01 Motor
                                {
                                    var res = text.Substring(4, text.Length - 8); //2 Bytes
                                    f = Convert.ToInt32(res, 16) * 0.01;
                                    unit = "% Soc Anz. ";
                                }


                                if (cmd == "475F") // PTC current
                                {
                                    var res = text.Substring(4, text.Length - 8); //2 Bytes
                                    f = Convert.ToInt32(res, 16) * 0.25;
                                    unit = "A PTC current";
                                }

                                if (cmd == "27D6") // Compressor kw
                                {
                                    var res = text.Substring(4, text.Length - 8); //2 Bytes
                                    f = Convert.ToInt32(res, 16) * 0.25;
                                    unit = "kW Compressor";
                                }

                                if (cmd == "1E1B" | cmd == "1E1C" | cmd == "")
                                {
                                    var res = text.Substring(4, text.Length - 4);
                                    f = Convert.ToInt32(res, 16) * 1.0;
                                    unit = "Amp";
                                }
                                */
                writeLogRaw(f, unit, cmd, resp);
            }
            else
            {
                //	 Console.Write($"{resp},");
                if (resp != "") return;
                if (resp.StartsWith("OK")) return;
                if (resp.StartsWith("22")) return;

                //if( !resp.ToUpper().StartsWith("AT")) Console.Write($"{resp},");
            }
        }

        void writeLogRaw(double val, string unit, string cmd, string comment = "")
        {
            //Console.WriteLine($"{DateTime.Now:yyMMdd HH:mm:ss.ff};{val:00.00};{unit};{cmd};{comment}");
            FileWriterRaw.WriteLine($"{DateTime.Now:yyMMdd HH:mm:ss.ff};{val:00.00};{unit}; {cmd} ; {comment}");
            addDataToFullRow(cmd, val);
        }

        void addDataToFullRow(string cmd, double val)
        {
            Logline = new LogLineReadyEventArgs(DateTime.Now);
            if (newline[cmd] == "")
            {
                newline[cmd] = $"{val:00.000}";
                if (cmd == "F40D") //Speed
                    Logline.Speed = val;
                  /*  if (cmd == "1E10") //Batt Temp
                        series2.Points.AddY(val);*/
            }
            else //wenn schon beschrieben, wird von einer vollen Zeile ausgegangen
            {
                if (Double.TryParse(newline["1801"], out double voltage) & Double.TryParse(newline["1802"], out double current))
                {
                    newline["0001"] = ($"{voltage * current / 1000.0:00.000}");
                    Logline.Power = (voltage * current / 1000.0);
                    Logline.Voltage = voltage;
                    Logline.Current = current;
                }
                else
                    newline["0001"] = "0";

                cleanAndWrite();
                newline[cmd] = ($"{val:0.000}"); //und schon die neue Zeile befüllen
            }
        }

       
        void cleanAndWrite()
        {
            //initnewline();

            try
            {
                foreach (var element in newline) //BUG: element modifies error
                {
                    if (element.Value == "")
                        newline[element.Key] = lastline[element.Key];
                }
            }
            catch (Exception)
            {
            }
            FileWriter.WriteLine(newline.ToCSV());
            //Console.WriteLine(newline.ToCSV());
            Logline.LogLine = newline.ToCSV();
            Logline.textonly = false;
            OnLogLineReady(Logline); //raise
            initnewline();
        }

        void initnewline()
        {

            lastline = new Dictionary<string, string>(newline);
            //newline.Clear();
            foreach (var element in mapping)
            {
                newline[element.Key] = "";
                //	lastline[element.Key] = "";
            }
            newline["0000"] = $"{DateTime.Now:HH:mm:ss.ff}";
        }

        void Init()
        {
            mapping = new Dictionary<string, string>()
           {
                { "0000","time" },
                { "0001","Power" },
                { "1801","HV Volt1" },
                { "1802","Current 1"},
                   { "F40D","Veh.Speed"},
               /*     { "1E1B","BattlimitChar"},
                    { "1E1C","BattlimDisc"},
                    { "028C","Soc gesamt"},
                 
	            //	{"1E33","CellVoltMax"},
	            //	{"1E34","CellVoltMin"},
	              //  { "1804","Current 2"},
	             //	{ "1E0E1E0F","TempBat max"},
                 //   { "1E0F","TempBat min"},
                   { "1E0F2","TempBat min #"}
                    { "1E0E2","TempBat max #"}, 
                    { "181C","Cool Inlet"},
                    { "181D","Cool Outlet"},
                    { "1E10","T Batt Ave"},
                 { "1E3B","CellVoltSum"},  
                  { "1E2D","SocCell min"},
		            { "1E2C","SocCell max"},
		            { "08D2","SoC Anzeige"},
                    {"475F", "PTC current"},
                    {"2608","Outside Temp"},
                    {"2613","Inside Temp"}
	            /*	{"27D6","Kompressor kW"},
		            {"0530","Dashboard"}
		
	              { "1E2D2","SocCell min #"},
	 	            { "1E2C2","SocCell max #"},
  	            { "192A","Reqrmt_BMS_CoolLvl"},
		            { "192C","StateBattCoolPump"}, 
		            { "192B","CoolPumpPwrSetPoint"}
		
		Strecke Read Data By Identifier ;22 05 20;10547
                RSP0002;19:37:08.822;LL_DashBoardUDS BV_DashBoardUDS EV_DashBoardLGEPO513_004;Strecke Read Data By Identifier ;62 05 20 00 37 b1 00 a6 4f 00;10547

                714 03 22 05 30 
                77E 
                RSP0002;19:35:18.655;LL_DashBoardUDS BV_DashBoardUDS EV_DashBoardLGEPO513_004;Berechnete, angezeigte Werte Read Data By Identifier ;22 05 30;9982
                62 05 30 00 22 c1 24 ce 25 a1 80 00 01 dd d0 01 80 00 00 00 05 32 78 1b d8 80 00 46 00 18 00 00 00 00 01 60 23 82 7f 84 c9 a6 00 dc 00 00 00 00 3d b8 01 5d 4c 45 44 38 59 52 4c 45 00 00 00;9981
               */

	 };

            newline = new Dictionary<string, string>();
            lastline = new Dictionary<string, string>();

            initnewline();

            //		string[] logline = new string[mapping.Count];



        }

        private void CANInit()
        {
            writeCAN("ATZ", 2000);   //write("ATH1");
            writeCAN("ATE0", 500); //bringt das was?

            /*	writeCAN("AT SP0", 4);  // Set protokoll to 7 - ISO 15765-4 CAN (29 bit ID, 500 kbaud)
				writeCAN("AT ST16", 4); */
            //	writeCAN("atcp17", 4); //5 msb can header 
            //	writeCAN("atfcsm1",200);
            //	writeCAN("ATSHFC007B", 4); //BMS
            writeCAN("ATSH7E8", 500); //BMS
            writeCAN("ATCRA7ED", 500);

            /*
					{0, "ATE0"},	// Echo off
					 "ATAT0"}, //	Adaptive timing off
					{ 0, "ATSTFF"}, //	Set timeout to ff x 4ms
					{ 0, "ATAL"}, //	Allow long messages ( > 7 Bytes)
					{ 0, "ATH1"}, //	Additional headers on
					{ 0, "ATS0"}, //	Printing of spaces off
					{ 0, "ATL0"}, //	Linefeeds off
					{ 0, "ATCSM0"}, //	Silent monitoring off*/
        }
        public async void test()
        {
            var rnd = new Random();
            await Task.Delay(1000);

            if (debug)
            {
                OnLogLineReady(new LogLineReadyEventArgs(DateTime.Now)
                {
                   textonly = true,
                    LogLine = "00 11 22|"
                });
            }
            else
            {
                OnLogLineReady(new LogLineReadyEventArgs(DateTime.Now)
                {
                    Current = rnd.NextDouble() * 3.22,
                    Voltage = 822.1,
                    textonly = false,
                    Power = rnd.Next(2, 10) * 2.1,
                    Speed = rnd.NextDouble() * 12.22,
                    LogLine = "nur ein Test\r\n"
                });
            }
        }
    }

    public class LogLineReadyEventArgs : EventArgs
    {
        public LogLineReadyEventArgs(DateTime time)
        {
            Time = time;
        }
        public string LogLine { get; set; }
        public double Power { get; set; }

        public double Voltage { get; set; }

        public double Current { get; set; }

        public DateTime Time { get; set; }
        public double Speed { get;  set; }

        public bool textonly { get; set; }
    }
}

