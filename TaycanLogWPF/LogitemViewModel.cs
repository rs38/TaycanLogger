using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TaycanLogger
{

    internal class LogitemViewModel : ObservableCollection<List<OBDValue>>
    {
        public LogitemViewModel(): base()
        {
            Add(new List<OBDValue> { new OBDValue() { Name = "Voltage" } });
        }
    }

    
}