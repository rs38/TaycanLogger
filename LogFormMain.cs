using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TaycanLogger
{
    public partial class LogFormMain : Form
    {
        Logger myOBD;

       
        Series series1;
        Series series2;
        Series series3;
        Series series4;


        string COMport { 
            get;
            set; }


        public LogFormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();

            InitChart();
            InitCOMDropbox();
            
         
            myOBD = new Logger();
            myOBD.Delay = 120;
            myOBD.LogLineReady += ProcessLogline;

        }

         void ProcessLogline(object sender, LogLineReadyEventArgs e)
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
        }

        void HandleSomethingHappened(string foo)
        {
            //Do some stuff
        }
       private void InitCOMDropbox()
        {
            for (int i = 1; i < 12; i++)
            {
                comboBoxCOMPort.Items.Add($"COM{i}");
            }
           // comboBoxCOMPort.SelectedIndex = 9-1;
        }

        async void button1_Click(object sender, EventArgs e)
        {
            textBoxDebug.Text = "starting....";


            myOBD.stop = false;
            await myOBD.LogfromCOM(COMport);
        }

      
        private void comboBoxCOMPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            COMport = $"COM{((ComboBox)sender).SelectedIndex+1}";
            Debug.WriteLine($"COM{((ComboBox)sender).SelectedIndex+1} seleceted");
            textBoxDebug.Text += $"COM{((ComboBox)sender).SelectedIndex+1} seleceted\r\n";

        }

         void buttonStop_Click(object sender, EventArgs e)
        {
            myOBD.stop = true;
            textBoxDebug.Text = "stopped....\r\n";
        }

        private void LogFormMain_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            myOBD.test();
        }

        private void checkBoxIsDebug_CheckedChanged(object sender, EventArgs e)
        {
            myOBD.debug = checkBoxIsDebug.Checked;
        }

        private void numericUpDownWaitMs_ValueChanged(object sender, EventArgs e)
        {
            myOBD.Delay = (int)numericUpDownWaitMs.Value;
        }
    }
}
