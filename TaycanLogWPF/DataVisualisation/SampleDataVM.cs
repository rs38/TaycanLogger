using System.Collections.Generic;

namespace System.Windows.Controls.Samples.DataVisualisation
{
    public class SampleDataVM
    {
        public Dictionary<DateTime,int>  Data { get; private set; }

        public SampleDataVM()
        {
            Data = new Dictionary<DateTime, int>();

            Data.Add(DateTime.Now, 10);
           
        }

        
    }
}
