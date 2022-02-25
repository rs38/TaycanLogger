using System;

namespace TaycanLogger
{
    internal class LogitemViewModel
    {
        public DateTime Time { get; set; }
        public double Voltage { get; set; }

        public double Current { get; set; }
    }
}