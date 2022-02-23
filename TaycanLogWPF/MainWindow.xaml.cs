using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaycanLogger;

namespace TaycanLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaycanLogWPF : Window
    {
        OBDSession myOBDSession;

        Progress<OBDCommandViewModel> progressData;
        CancellationTokenSource cancel;
        string UIDeviceName
        {
            get;
            set;
        }



        public TaycanLogWPF()
        {
            InitializeComponent();
            UIDeviceName = Properties.Settings.Default.DeviceName;
            Debug.WriteLine($"start log at {DateTime.Now}!");

         

            myOBDSession = new OBDSession();
            InitCOMDropbox();
        }

        private void InitCOMDropbox()
        {
           var list = myOBDSession.GetPairedDevices();
            DeviceDropBox.ItemsSource = list; 
            Debug.WriteLine("init device drop box, default: " + UIDeviceName);
            if (list.Contains(UIDeviceName))
                  DeviceDropBox.SelectedItem = UIDeviceName;
        }
        private void OnTabChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {


        }

        private void OnDataChanged(object sender, OBDCommandViewModel e)
        {
            TextboxInformation.AppendText(e.logline + Environment.NewLine);
         

            //   series1.Points.AddY(e.DataList[0].ResponseValue);
            // series2.Points.AddY(e.DataList[1].ResponseValue);
        }

        async private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            progressData = new Progress<OBDCommandViewModel>();

            progressData.ProgressChanged += OnDataChanged;

            cancel = new CancellationTokenSource();
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

        }

        private void TextboxInformation_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            // set selection to end of document
            tb.Focus();
            tb.CaretIndex = tb.Text.Length;
           // tb.ScrollToEnd();
        }
    }

}
