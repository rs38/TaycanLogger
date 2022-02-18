using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
//using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;

namespace TaycanLogger
{
    public partial class LogFormMain : Form
    {
        Logger myLogger;
        OBD myOBD;
        
       /* Series series1;
        Series series2;
        Series series3;
        Series series4; */

        Configuration configSettings;
        KeyValueConfigurationCollection config;

        string ConnectionName { 
            get;  set; }

        public LogFormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            
            InitializeComponent();
            configSettings = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            configSettings.AppSettings.Settings.Add("DeviceName", "OBDKlein");
            config = configSettings.AppSettings.Settings;
            configSettings.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");
            //InitChart();
          
            ConnectionName = "ink";
            myOBD = new OBD(ConnectionName);
            myLogger = new Logger(myOBD);
         
            InitCOMDropbox();
            myLogger.Delay = 120;

        }

      /*   void ProcessLogline(object sender, LogLineReadyEventArgs e)
        {
            if (e.textonly)
                textBoxDebug.AppendText(e.LogLine);
            else
            {
                textBoxDebug.AppendText(e.Time + " " + e.LogLine);
                series1.Points.AddY(e.Power);
                series2.Points.AddY(e.Current);
                series3.Points.AddY(e.Voltage);
                series4.Points.AddY(e.Speed);
            }
        }

        private void InitChart()
        {
            var ca = new ChartArea("A1");
            var ca2 = new ChartArea("A2");
            var ca3 = new ChartArea("A3");
            chart1.ChartAreas.Add(ca);
            chart1.ChartAreas.Add(ca2);
            chart1.ChartAreas.Add(ca3);
            series1 = new Series("Power") { ChartType = SeriesChartType.Line };
            series1.ChartArea = "A1";
            chart1.Series.Add(series1);
            series2 = new Series("Voltage") { ChartType = SeriesChartType.Line };
            series2.ChartArea = "A2";
            series3 = new Series("Current") { ChartType = SeriesChartType.Line };
            series3.ChartArea = "A3";
            series4 = new Series("Speed") { ChartType = SeriesChartType.Line };
            series4.ChartArea = "ChartArea1"; //default
            chart1.Series.Add(series2);
            chart1.Series.Add(series3);
            chart1.Series.Add(series4);
        }*/

        void HandleSomethingHappened(string foo)
        {
            //Do some stuff
        }
       private void InitCOMDropbox()
        {
           var s = myOBD.DiscoverDevices();
            comboBoxCOMPort.DataSource = s;

            var x = config["DeviceName"].Value;
          
            comboBoxCOMPort.SelectedText = x;
            ConnectionName = x;
        }

        async void button1_Click(object sender, EventArgs e)
        {
            textBoxDebug.Text = "starting....";


            myLogger.stop = false;
            await myLogger.LogfromCOM(ConnectionName);
        }

      
        private void comboBoxCOMPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionName = ((ComboBox)sender).SelectedItem.ToString();
            Debug.WriteLine($"{ConnectionName} seleceted");
            textBoxDebug.Text += $"{ConnectionName} seleceted\r\n";

            config["DeviceName"].Value = ((ComboBox)sender).SelectedValue.ToString();
            configSettings.Save(ConfigurationSaveMode.Full, true);
            ConfigurationManager.RefreshSection("appSettings");

        }

         void buttonStop_Click(object sender, EventArgs e)
        {
            myLogger.stop = true;
            textBoxDebug.Text = "stopped....\r\n";
        }

        private void LogFormMain_Load(object sender, EventArgs e)
        {

        }

      

        private void checkBoxIsDebug_CheckedChanged(object sender, EventArgs e)
        {
            myLogger.debug = checkBoxIsDebug.Checked;
        }

        private void numericUpDownWaitMs_ValueChanged(object sender, EventArgs e)
        {
            myLogger.Delay = (int)numericUpDownWaitMs.Value;
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
