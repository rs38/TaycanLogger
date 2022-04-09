using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaycanLogger
{
    public interface IOBDDevice
    {
        void Dispose();
        bool init(string _dongleName);
        Task writeAllAsync(string[] array);
        Task writeAsync(string str);
        Task<string> WriteReadAsync(string str);
        public string CurrentECUHeader { get; set; }
    }
}