﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

        ObservableCollection<List<double>> LoglineGrid;
       


        public TaycanLogWPF()
        {
            InitializeComponent();
            Trace.Listeners.Add(new TextWriterTraceListener(File.Create("TraceFile.txt")) );

            Trace.AutoFlush = true;

            this.DataContext = this;

            UIDeviceName = Properties.Settings.Default.DeviceName;
            ConfigFilename = Properties.Settings.Default.ConfigFilename;
            Trace.WriteLine($"start log at {DateTime.Now}!");

            DataChart1 = new ObservableCollection<KeyValuePair<DateTime, double>>();
            DataChart1.Add(new KeyValuePair<DateTime, double>(DateTime.Now, 10.1));

            myOBDSession = new OBDSession(ConfigFilename, UIDeviceName);

            InitCOMDropbox();
            progressData = new Progress<OBDCommandViewModel>();
            progressData.ProgressChanged += OnDataChanged;

          
            InitPlot();

        }

        private void InitCOMDropbox()
        {
            var list = myOBDSession.GetPairedDevices();
            DeviceDropBox.ItemsSource = list;
            if (list.Count == 0)
            {
                TextboxInformation.AppendText("no paired device found");
                Trace.WriteLine("no paired device found");
                StartButton.IsEnabled = false;
            }
            else
            {
                Trace.WriteLine("init device drop box, default: " + UIDeviceName);
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

            LoglineGrid.Add(e.DataList.Select(d => d.Value).ToList());// Select(d => new { name = d.name, value = d.Value }));
                
          //  dataGrid1.Items.Refresh();

            var r = new Random();

            var value = e.DataList.Where(d => d.Name == "Amp").First().Value;// + ( r.NextDouble()*50);

            lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), value));

            //moving time axis
            PlotViewModel.MyModel.Axes[0].Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(-1));
            PlotExtern.Plot1.Model.InvalidatePlot(true);
        }

        async private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            cancel = new CancellationTokenSource();
            if (!await myOBDSession.InitDevice())
            {
                TextboxInformation.AppendText("errors while reading config or init device" + Environment.NewLine);
                return;
            }
            LoglineGrid = new ObservableCollection<List<double>> {
                myOBDSession.cmds.SelectMany(c => c.Values).Select(v => v.Value).ToList()};
            dataGrid1.ItemsSource = LoglineGrid; 
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
            Trace.WriteLine($"{UIDeviceName} seleceted");
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
            PlotViewModel.MyModel = new PlotModel { Title = "Ampere" };
            lineSeries = new LineSeries ();

            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddMinutes(1);

            var minValue = DateTimeAxis.ToDouble(startDate);
            var maxValue = DateTimeAxis.ToDouble(endDate);

            PlotViewModel.MyModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom , Minimum = minValue});
            PlotViewModel.MyModel.Series.Add(lineSeries);

            PlotExtern.Plot1.Model = PlotViewModel.MyModel;
            PlotExtern.Plot1.Model.InvalidatePlot(true);

        }
    }

}
