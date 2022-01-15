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

       
        string COMport { 
            get;
            set; }


        public LogFormMain()
        {
            InitializeComponent();

            InitCOMDropbox();

            myOBD = new Logger();

          

        }

        void readdata(object sender, OBD.NET.Common.Communication.EventArgs.DataReceivedEventArgs e)
        { }

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
            textBoxDebug.Text += $"COM{((ComboBox)sender).SelectedIndex+1} seleceted\n";

        }

         void buttonStop_Click(object sender, EventArgs e)
        {
            myOBD.stop = true;
            textBoxDebug.Text = "stopped....\n";
        }

        private void LogFormMain_Load(object sender, EventArgs e)
        {

        }
    }
}
