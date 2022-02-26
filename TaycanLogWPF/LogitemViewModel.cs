using System;
using System.Collections.ObjectModel;

namespace TaycanLogger
{

    internal class LogitemViewModel : ObservableCollection<Logitem>
    {
        public LogitemViewModel(): base()
        {
            Add(new Logitem() { Current = 2.2, Voltage = 730.1, SoC = 90 });
        }
    }

    
    internal class Logitem
    {
        public Logitem()
        {
            Time = DateTime.Now;
        }
        public DateTime Time { get; private set; }
        public double Voltage { get; set; }

        public double Current { get; set; }

        public int SoC { get; set; }

        public double Power { get; set; }

    }
}