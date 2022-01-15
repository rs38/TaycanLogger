using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace TaycanLogger
{
    public partial class LogFormMain : Form
    {
        Logger myOBD;
        string COMport;

        public LogFormMain()
        {
            InitializeComponent();

            InitCOMDropbox();

            myOBD = new Logger();

          

        }

        private void InitCOMDropbox()
        {
            for (int i = 1; i < 12; i++)
            {
                comboBoxCOMPort.Items.Add($"COM{i}");
            }
            comboBoxCOMPort.SelectedIndex = 1;
            COMport = "COM2";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myOBD.LogfromCOM(COMport);
        }

      
        private void comboBoxCOMPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            COMport = $"COM{((ComboBox)sender).SelectedIndex}";
            Debug.WriteLine($"COM{((ComboBox)sender).SelectedIndex} seleceted");

        }
    }
}
