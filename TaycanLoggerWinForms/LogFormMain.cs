using System;
using System.Diagnostics;
using System.Linq;
using System.Configuration;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;


namespace TaycanLogger
{
    public partial class LogFormMain : Form
    {
        OBDSession myOBDSession;

        Progress<OBDCommandViewModel> progressData;
        CancellationTokenSource cancel;
        string UIDeviceName;
        string ConfigFilename;

        public LogFormMain()
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
            UIDeviceName = Properties.Settings.Default.DeviceName;
           // ConfigFilename 
            Debug.WriteLine($"start log at {DateTime.Now}!");

            InitChart();

            myOBDSession = new OBDSession(ConfigFilename,UIDeviceName);

            InitCOMDropbox();
        }

        private void InitChart()
        {

        }

        private void InitCOMDropbox()
        {

            var list = myOBDSession.GetPairedDevices();
            comboBoxCOMPort.ValueMember = UIDeviceName;
            comboBoxCOMPort.DataSource = list;
            Debug.WriteLine("init device drop box, default: " + UIDeviceName);
            if (list.Contains(UIDeviceName))
                comboBoxCOMPort.SelectedItem = UIDeviceName;
        }

        async void ButtonDoLog_Click(object sender, EventArgs e)
        {
            progressData = new Progress<OBDCommandViewModel>();

            progressData.ProgressChanged += OnDataChanged;

            cancel = new CancellationTokenSource();
            await myOBDSession.DoLogAsync(UIDeviceName, progressData, cancel.Token);
        }

        private void OnDataChanged(object sender, OBDCommandViewModel e)
        {
            textBoxDebug.AppendText(e.logline + Environment.NewLine);

            //   series1.Points.AddY(e.DataList[0].ResponseValue);
            // series2.Points.AddY(e.DataList[1].ResponseValue);
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
            textBoxDebug.AppendText("stopped....\r\n");
            // progressData.ProgressChanged
            cancel.Cancel();
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
