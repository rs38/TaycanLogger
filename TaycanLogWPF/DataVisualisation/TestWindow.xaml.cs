using OxyPlot;
using OxyPlot.Series;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace TaycanLogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {

        public TestWindow()
        {
            InitializeComponent();
        }

        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
         /*   PlotViewModel.MyModelV = new PlotModel { Title = "Time" };
            PlotViewModel.MyModelA = new PlotModel { Title = "Time" };
            var lineSeries = new LineSeries { MarkerType = MarkerType.Star };
            var r = new Random(314);
            for (int i = 0; i < 10; i++)
            {
                var y = r.NextDouble() * 100;
                lineSeries.Points.Add(new DataPoint(i, y));
            }

            PlotViewModel.MyModel.Series.Add(lineSeries);
            PlotViewModel.MyModel.Series.Add(lineSeries);

            PlotExtern2.Plot1.Model = PlotViewModel.MyModelV;
            PlotExtern2.Plot1.Model.InvalidatePlot(true);

            for (int i = 10; i < 100; i++)
            {
                var y = r.NextDouble() * 100;
                lineSeries.Points.Add(new DataPoint(i, y));
                await Task.Delay(500);
                PlotExtern2.Plot1.Model.InvalidatePlot(true);
            }*/
        }
    }
}
