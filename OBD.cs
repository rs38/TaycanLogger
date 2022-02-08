using InTheHand.Bluetooth;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TaycanLogger
{
    public class OBD : IDisposable
    {
        BluetoothClient BTclient;
        BluetoothDeviceInfo device;

        TcpClient NETclient;
        NetworkStream stream;
        byte[] buffer;
        char[] charsToTrim = { '\r', ' ', '>', '\0' };
        string dongleName;

        enum DeviceType { BT, IP, USB }
        DeviceType devicetype;

        public OBD( string conn)
        {
            dongleName = conn;
            BTclient = new BluetoothClient();
        }
        public void Init(string _dongleName)
        {
            this.dongleName = _dongleName;
            buffer = new byte[80];
        }

        public bool init()
        {
            if (dongleName.Contains("192"))
            {
                devicetype = DeviceType.IP;
                return initIP(dongleName);
            }
            else
            {
                devicetype = DeviceType.BT;
                device = getPairedAndroidDongle(dongleName);
                return initBT();
            }
        }

        private void BluetoothClientConnectCallback(IAsyncResult ar)
        {
            Console.WriteLine(ar.ToString());
        }

        bool initIP(string ip)
        {
            try
            {
                TcpClient client = new TcpClient(ip, 35000);
                NetworkStream stream = client.GetStream();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        bool initBT()
        {
            try
            {
                //device.Refresh();
                System.Diagnostics.Debug.WriteLine(device.Authenticated);
                if (!BTclient.Connected)
                    BTclient.Connect(device.DeviceAddress, BluetoothService.SerialPort);


                stream = BTclient.GetStream();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }



        public async Task<string> writeReadAsync(string str)
        {
            await writeAsync(str);
            return await readAsync();
        }

        public async Task writeAsync(string str)
        {
            await stream.WriteAsync(Encoding.ASCII.GetBytes(str + '\r'), 0, str.Length + 1);
            // stream.FlushAsync();
        }

        public void writeAll(string[] array)
        {
            foreach (var str in array)
            {
                Console.Write(writeReadAsync(str).Result + "|");
            }
        }

        public async Task<string> readAsync()
        {
            string answer = "";
            do
            {
                var x = stream.ReadAsync(buffer, 0, 40);
                answer += System.Text.Encoding.UTF8.GetString(buffer, 0, x.Result);
            } while (!answer.Contains('>'));
            //Console.WriteLine($"read {x} ");
            return answer.Trim(charsToTrim);
        }

        static void getpairedLE()
        {
            foreach (var d in Bluetooth.GetPairedDevicesAsync().Result)
            {
                Debug.WriteLine($"{d.Id} {d.Name}");
            }
        }

         BluetoothDeviceInfo getPairedAndroidDongle(string name)
        {
            var bts = BTclient.PairedDevices;
            var dev = bts.Where(b => b.DeviceName.Contains(name)).FirstOrDefault();

            if (dev == null)
                dev = discover(name);

            if (!dev.Authenticated)
            {
                BluetoothSecurity.PairRequest(dev.DeviceAddress, "1234");
            }
            return dev;
        }

        public string[] DiscoverDevices()
        {

            return BTclient.PairedDevices.Select(b => b.DeviceName).ToArray();
        }

        BluetoothDeviceInfo discover(string name)
        {
            foreach (var dev in BTclient.DiscoverDevices())
            {
                if (dev.DeviceName.Contains(name))
                {
                    device = dev;
                    break;
                }
            }
            return device;
        }

        public void Dispose()
        {


            stream.Close();
            if (devicetype == DeviceType.BT)
                BTclient.Close();
        }
    }
}

