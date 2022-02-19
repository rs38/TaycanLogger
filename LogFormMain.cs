using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;
using Microsoft.Extensions.Logging;

namespace TaycanLogger
{
    public partial class LogFormMain : Form
    {
      
        OBDSession myOBDSession;
        OBDWorker myWorker;
        
        Series series1;
        Series series2;
        Series series3;
        Series series4;

        ILogger logger;

        string ConnectionName { 
            get;  set; }

        public LogFormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            ReadConfig();
            CreateLogger();

            InitChart();

         
            myOBDSession = new OBDSession();
            myWorker = new OBDWorker();

            InitCOMDropbox();

        }

        private  void CreateLogger()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddDebug();
               

            });

            logger = loggerFactory.CreateLogger<LogFormMain>();
            logger.LogInformation($"start log at {DateTime.Now}!");
        }

        private void ReadConfig()
        {
          ConnectionName=  Properties.Settings.Default.DeviceName;
        }

        void ProcessLogline( )
        {
           
                series1.Points.AddY();
                series2.Points.AddY();
                series3.Points.AddY();
                series4.Points.AddY();
            
        }

        private void InitChart()
        {
            chart1 = new Chart();
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
        }

        void HandleSomethingHappened(string foo)
        {
            //Do some stuff
        }
       private void InitCOMDropbox()
        {
            comboBoxCOMPort.DataSource = myOBDSession.GetPairedDevices();
            comboBoxCOMPort.SelectedText = Properties.Settings.Default.DeviceName;
           
        }

        async void ButtonDoLog_Click(object sender, EventArgs e)
        {
            textBoxDebug.Text = "starting....";
            myOBDSession.InitDevice(ConnectionName);
            await myWorker.Work(myOBDSession);
        }

      
        private void comboBoxCOMPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConnectionName = ((ComboBox)sender).SelectedItem.ToString();
            Debug.WriteLine($"{ConnectionName} seleceted");
            textBoxDebug.Text += $"{ConnectionName} seleceted\r\n";
            
            Properties.Settings.Default.DeviceName = ((ComboBox)sender).SelectedValue.ToString();
            Properties.Settings.Default.Save();
        }

         void buttonStop_Click(object sender, EventArgs e)
        {
         
            textBoxDebug.Text = "stopped....\r\n";
        }

        private void LogFormMain_Load(object sender, EventArgs e)
        {
        }

        private void checkBoxIsDebug_CheckedChanged(object sender, EventArgs e)
        {
          
        }

        private void numericUpDownWaitMs_ValueChanged(object sender, EventArgs e)
        {
         
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
