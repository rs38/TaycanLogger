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


        string COMport { 
            get;
            set; }


        public LogFormMain()
        {
            InitializeComponent();

            InitChart();
            InitCOMDropbox();
            
         
           // debugwriter = s => textBoxDebug.AppendText(s + "\n\r");

            myOBD = new Logger();
            myOBD.LogLineReady += ProcessLogline;

        }

         void ProcessLogline(object sender, LogLineReadyEventArgs e)
        {
            textBoxDebug.AppendText(e.Time + e.LogLine);
            series1.Points.AddY(e.Power);

        }

        private void InitChart()
        {
            
            var ca = new ChartArea();
            var ca2 = new ChartArea("A2");
            var ca3 = new ChartArea("A3");
            chart1.ChartAreas.Add(ca);
            chart1.ChartAreas.Add(ca2);
            chart1.ChartAreas.Add(ca3);
            series1 = new Series("Eins") { ChartType = SeriesChartType.Line };
            chart1.Series.Add(series1);
            series2 = new Series("Zwei") { ChartType = SeriesChartType.Line };
            series2.ChartArea = "A2";
            series3 = new Series("Drei") { ChartType = SeriesChartType.Line };
            series3.ChartArea = "A3";
            chart1.Series.Add(series2);
            chart1.Series.Add(series3);
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
            comboBoxCOMPort.SelectedIndex = 9-1;
            COMport = "COM9";
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
    }
}
