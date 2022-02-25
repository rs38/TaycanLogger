using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace TaycanLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaycanLogWPF : Window
    {
        OBDSession myOBDSession { get; set; }
        string UIDeviceName;
        string ConfigFilename;
        Progress<OBDCommandViewModel> progressData;
        LineSeries lineSeries;
        CancellationTokenSource cancel;
        public ObservableCollection<KeyValuePair<DateTime, double>> DataChart1 { get; private set; }

        List<LogitemViewModel> LoglineGrid;


        public TaycanLogWPF()
        {
            InitializeComponent();
            this.DataContext = this;

            UIDeviceName = Properties.Settings.Default.DeviceName;
            ConfigFilename = Properties.Settings.Default.ConfigFilename;
            Debug.WriteLine($"start log at {DateTime.Now}!");

            DataChart1 = new ObservableCollection<KeyValuePair<DateTime, double>>();
            DataChart1.Add(new KeyValuePair<DateTime, double>(DateTime.Now, 10.1));

            myOBDSession = new OBDSession(ConfigFilename, UIDeviceName);

            InitCOMDropbox();
            progressData = new Progress<OBDCommandViewModel>();
            progressData.ProgressChanged += OnDataChanged;

            LoglineGrid = new List<LogitemViewModel>();
            dataGrid1.ItemsSource = LoglineGrid;
            InitPlot();

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

            //quick and dirty

            var currentItem = new LogitemViewModel
            {
                Time = DateTime.Now,
                Voltage = Convert.ToDouble(e.DataList[0].ResponseValue),
                Current = Convert.ToDouble(e.DataList[1].ResponseValue)
            };
            LoglineGrid.Add(currentItem);
            dataGrid1.Items.Refresh();

            var r = new Random();

            var value = currentItem.Current + ( r.NextDouble()*50);

            lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), value));

            // PlotExtern.Plot1.Model = PlotViewModel.MyModel;
            PlotViewModel.MyModel.Axes[0].Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(-2));
            PlotExtern.Plot1.Model.InvalidatePlot(true);
            

            // DataChart1.Add(new KeyValuePair<DateTime, double>(DateTime.Now, Convert.ToDouble(e.DataList[1].ResponseValue)));
           // C1.DataContext = DataChart1;
            //   series1.Points.AddY(e.DataList[0].ResponseValue);
            // series2.Points.AddY(e.DataList[1].ResponseValue);
        }

        async private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            cancel = new CancellationTokenSource();
            if (!await myOBDSession.InitDevice())
            {
                TextboxInformation.AppendText("errors while reading config/init" + Environment.NewLine);
                return;
            }
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
            myOBDSession.Devicename = UIDeviceName;
            Properties.Settings.Default.DeviceName = UIDeviceName;
            Properties.Settings.Default.Save();
        }

        private void Device_DropDownClosed(object sender, EventArgs e)
        {
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
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

         private void InitPlot()
        {
            PlotViewModel.MyModel = new PlotModel { Title = "Voltage" };
            lineSeries = new LineSeries ();



            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddMinutes(1);

            var minValue = DateTimeAxis.ToDouble(startDate);
            var maxValue = DateTimeAxis.ToDouble(endDate);

            PlotViewModel.MyModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom , Minimum = minValue});


            //var r = new Random(314);
            //for (int i = 0; i < 100; i++)
            //{
            //    var y = r.NextDouble() * 100;
            //    lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now.AddSeconds(i*1)), y));
            //}

            PlotViewModel.MyModel.Series.Add(lineSeries);

            PlotExtern.Plot1.Model = PlotViewModel.MyModel;
            PlotExtern.Plot1.Model.InvalidatePlot(true);

        }
    }

}
