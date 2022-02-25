using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace TaycanLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaycanLogWPF : Window
    {
        OBDSession myOBDSession;
        string UIDeviceName;
        string ConfigFilename;
        Progress<OBDCommandViewModel> progressData;
        CancellationTokenSource cancel;
        public ObservableCollection<KeyValuePair<DateTime, double>> DataChart1 { get; private set; }



        public TaycanLogWPF()
        {
            InitializeComponent();
            this.DataContext = this;
            cancel = new CancellationTokenSource();

            UIDeviceName = Properties.Settings.Default.DeviceName;
            ConfigFilename = Properties.Settings.Default.ConfigFilename;
            Debug.WriteLine($"start log at {DateTime.Now}!");

            DataChart1 = new ObservableCollection<KeyValuePair<DateTime, double>>();
            DataChart1.Add(new KeyValuePair<DateTime, double>(DateTime.Now, 10.1));

            myOBDSession = new OBDSession(ConfigFilename, UIDeviceName);

            InitCOMDropbox();

        }

        private void InitCOMDropbox()
        {
            var list = myOBDSession.GetPairedDevices();
            DeviceDropBox.ItemsSource = list;
            if (list.Count == 0)
            {
                TextboxInformation.AppendText("no paired device found");
                Debug.WriteLine("no paired device found");
                StartButton.IsEnabled = false;
            }
            else
            {
                Debug.WriteLine("init device drop box, default: " + UIDeviceName);
                StartButton.IsEnabled = true;
                if (list.Contains(UIDeviceName))
                    DeviceDropBox.SelectedItem = UIDeviceName;
            }
        }
        private void OnTabChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void OnDataChanged(object sender, OBDCommandViewModel e)
        {
            TextboxInformation.AppendText(e.logline + Environment.NewLine);
            DataChart1.Add(new KeyValuePair<DateTime, double>(DateTime.Now, Convert.ToDouble(e.DataList[1].ResponseValue)));
            C1.DataContext = DataChart1;
            //   series1.Points.AddY(e.DataList[0].ResponseValue);
            // series2.Points.AddY(e.DataList[1].ResponseValue);
        }

        async private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            progressData = new Progress<OBDCommandViewModel>();
            progressData.ProgressChanged += OnDataChanged;


            await myOBDSession.DoLogAsync(UIDeviceName, progressData, cancel.Token);
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            TextboxInformation.AppendText("stopped....\r\n");
            // progressData.ProgressChanged
            cancel.Cancel();
        }

        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Device_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            UIDeviceName = e.AddedItems[0].ToString();
            Debug.WriteLine($"{UIDeviceName} seleceted");
            TextboxInformation.Text += $"{UIDeviceName} seleceted\r\n";

            Properties.Settings.Default.DeviceName = UIDeviceName;
            Properties.Settings.Default.Save();
        }

        private void Device_DropDownClosed(object sender, EventArgs e)
        {
            InitCOMDropbox();
        }

        private void TextboxInformation_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // set selection to end of document
            tb.Focus();
            tb.CaretIndex = tb.Text.Length;
            // tb.ScrollToEnd();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }

}
