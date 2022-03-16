using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace TaycanLogger
{
    public class PlotViewModel
    {
        public PlotViewModel()
        {
         //   MyModel = new PlotModel { Title = "Example 1" };
         //   MyModel.Series.Add(new FunctionSeries(Math.Cos, 0, 10, 0.1, "cos(x)"));

        }
        public static PlotModel MyModelA { get; set; }
        public static PlotModel MyModelV { get; set; }
    }
}
