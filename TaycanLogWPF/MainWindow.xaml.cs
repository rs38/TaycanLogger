using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    //string ConfigFilename;

    Progress<OBDCommandViewModel> progressData;
    LineSeries lineSeriesA;
    LineSeries lineSeriesV;
    DataTable dt;

    CancellationTokenSource cancel;
    public ObservableCollection<KeyValuePair<DateTime, double>> DataChart1 { get; private set; }


    public TaycanLogWPF()
    {
      InitializeComponent();
      Trace.Listeners.Add(new TextWriterTraceListener(File.Create(@$"{System.Environment.CurrentDirectory}\TraceFile.txt")));
      Trace.AutoFlush = true;

      this.DataContext = this; //???

      UIDeviceName = Properties.Settings.Default.DeviceName;
      //var configFilename = Properties.Settings.Default.ConfigFilename;
            var configFilename = "dash.xml";
            var configContent = Properties.Resources.obd2_TaycanSOH;
      configContent = File.ReadAllText(configFilename); //external file

      Trace.WriteLine($"start log at {DateTime.Now}!");

      DataChart1 = new ObservableCollection<KeyValuePair<DateTime, double>>();
      DataChart1.Add(new KeyValuePair<DateTime, double>(DateTime.Now, 10.1));

      myOBDSession = new OBDSession(configContent, UIDeviceName);
      if (myOBDSession != null)
        myOBDSession.RawFilename = () => PickRawFile();

      InitCOMDropbox();
      progressData = new Progress<OBDCommandViewModel>();
      progressData.ProgressChanged += OnDataChanged;

      InitPlot();

      if (!myOBDSession.hasValidConfig()) return;


      var valueLine = myOBDSession.cmds.SelectMany(c => c.Values).ToList();
      // valueTable.Add(valueLine);

      //var tab = new System.Windows.Controls.DataGrid { Name = "Grid1" };
      dt = new DataTable("dt1");
      var dc = new DataColumn("time", typeof(DateTime));
      dt.Columns.Add(dc);
      foreach (var element in valueLine)
      {
        dc = new DataColumn(element.Name, typeof(double));
        dt.Columns.Add(dc);
      }

      dataGrid1.DataContext = dt.DefaultView;
      dataGrid1.ItemsSource = dt.AsDataView();

    }

    private string PickRawFile()
    {
      // Configure open file dialog box
      var dialog = new Microsoft.Win32.OpenFileDialog();
      dialog.FileName = string.Empty;
      dialog.DefaultExt = ".raw"; // Default file extension
      dialog.Filter = "RawDevice data (.raw)|*.raw"; // Filter files by extension

      // Show open file dialog box
      bool? result = dialog.ShowDialog();

      // Process open file dialog box results
      if (result == true)
      {
        // Open document
        return dialog.FileName;
      }
      return null;
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
    { }

    private void OnDataChanged(object sender, OBDCommandViewModel e)
    {
      TextboxInformation.AppendText(e.logline + Environment.NewLine);

      var valueA = e.DataList.Where(d => d.Name == "Amp").First().Value;//
      lineSeriesA.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueA));
      var valueV = e.DataList.Where(d => d.Name == "BatV").First().Value;// 
      lineSeriesV.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), valueV));

      //moving time axis
      PlotViewModel.MyModelA.Axes[0].Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(-1));
      PlotViewModel.MyModelV.Axes[0].Minimum = DateTimeAxis.ToDouble(DateTime.Now.AddMinutes(-1));
      PlotAmp.Plot1.Model.InvalidatePlot(true);
      PlotV.Plot1.Model.InvalidatePlot(true);

      var row = dt.NewRow();
      var rowList = new List<object> { (object)DateTime.Now };
      row.ItemArray = rowList.Concat(e.DataList.Select(l => (object)l.Value)).ToArray();
      dt.Rows.Add(row);
      // dataGrid1.UpdateLayout();
    }

    async private void StartButton_Click(object sender, RoutedEventArgs e)
    {
      if (cancel is null)
      {
        cancel = new CancellationTokenSource();
        if (!await myOBDSession.InitDevice())
        {
          TextboxInformation.AppendText("errors while reading config or init device" + Environment.NewLine);
          return;
        }
        /*   LoglineGrid = new ObservableCollection<List<double>> {
               myOBDSession.cmds.SelectMany(c => c.Values).Select(v => v.Value).ToList()};
           dataGrid1.ItemsSource = LoglineGrid; 
          */
        await myOBDSession.DoLogAsync(UIDeviceName, progressData, cancel.Token);
      }
    }
        
    

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
      if (cancel is not null)
      {
        TextboxInformation.AppendText("stopped....\r\n");
        // progressData.ProgressChanged
        cancel.Cancel();
        cancel = null;
      }
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
    { }

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
      PlotViewModel.MyModelA = new PlotModel { Title = "Ampere" };
      lineSeriesA = new LineSeries();
      lineSeriesV = new LineSeries();
      var startDate = DateTime.Now;
      var endDate = DateTime.Now.AddMinutes(1);

      var minValue = DateTimeAxis.ToDouble(startDate);
      //  var maxValue = DateTimeAxis.ToDouble(endDate);

      PlotViewModel.MyModelA.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue });
      PlotViewModel.MyModelA.Series.Add(lineSeriesA);

      PlotAmp.Plot1.Model = PlotViewModel.MyModelA;
      PlotAmp.Plot1.Model.InvalidatePlot(true);


      PlotViewModel.MyModelV = new PlotModel { Title = "Voltage" };

      PlotViewModel.MyModelV.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, Minimum = minValue });
      PlotViewModel.MyModelV.Series.Add(lineSeriesV);

      PlotV.Plot1.Model = PlotViewModel.MyModelV;
      PlotV.Plot1.Model.InvalidatePlot(true);
    }

    private void tickWirteToRaw_Checked(object sender, RoutedEventArgs e)
    {
      myOBDSession.WriteToRaw = tickWirteToRaw.IsChecked ?? false;
    }
  }

}
