using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Windows.Forms.DataVisualization.Charting;
using System.Configuration;

namespace TaycanLogger
{
    public partial class LogFormMain : Form
    {
        OBDSession myOBDSession;
        
        Series series1;
        Series series2;
        Series series3;
        Series series4;

        string UIDeviceName { 
            get; 
            set; }

        public LogFormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            UIDeviceName = Properties.Settings.Default.DeviceName;
            Debug.WriteLine($"start log at {DateTime.Now}!");

            InitChart();

            myOBDSession = new OBDSession();
            InitCOMDropbox();
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

       private void InitCOMDropbox()
        {
          
           var list = myOBDSession.GetPairedDevices();
            comboBoxCOMPort.ValueMember = UIDeviceName;
            comboBoxCOMPort.DataSource = list;
            Debug.WriteLine("init device drop box, default: "+UIDeviceName);
            if (list.Contains(UIDeviceName)) 
                comboBoxCOMPort.SelectedItem = UIDeviceName;
        }

        async void ButtonDoLog_Click(object sender, EventArgs e)
        {
            await myOBDSession.DoLogAsync(UIDeviceName);
        }
              
        private void comboBoxCOMPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            UIDeviceName = ((ComboBox)sender).SelectedItem.ToString();
            Debug.WriteLine($"{UIDeviceName} seleceted");
            textBoxDebug.Text += $"{UIDeviceName} seleceted\r\n";
            
            Properties.Settings.Default.DeviceName = ((ComboBox)sender).SelectedItem.ToString();
            Properties.Settings.Default.Save();
        }

         void buttonStop_Click(object sender, EventArgs e)
        {
             textBoxDebug.Text = "stopped....\r\n";
        }

        private void LogFormMain_Load(object sender, EventArgs e)
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
