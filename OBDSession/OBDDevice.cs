﻿using InTheHand.Bluetooth;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TaycanLogger
{

    public class OBDDevice : IDisposable
    {
        BluetoothClient BTclient;
        BluetoothDeviceInfo device;

        int IO_BUFFER_SIZE = 400;
        NetworkStream myStream;
        byte[] buffer;
        char[] charsToTrim = { '\r', ' ', '>', '\0' };
        string dongleName;

        enum DeviceType { BT, IP, USB }
        DeviceType devicetype;

        public OBDDevice()
        {
            BTclient = new BluetoothClient();
        }

        public bool init(string _dongleName)
        {
            this.dongleName = _dongleName;
            buffer = new byte[800];
            devicetype = DeviceType.BT;
            device = getPairedAndroidDongle(dongleName);
            return initBT();
        }

        private void BluetoothClientConnectCallback(IAsyncResult ar)
        {
            Console.WriteLine(ar.ToString());
        }

        bool initBT()
        {
            try
            {
                //device.Refresh();
                Debug.WriteLine($"device auth:{device.Authenticated}");
                if (!BTclient.Connected)
                    BTclient.Connect(device.DeviceAddress, BluetoothService.SerialPort);


                myStream = BTclient.GetStream();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }



        public async Task<string> WriteReadAsync(string str)
        {
            Debug.Write($"send:{str},");
            await writeAsync(str);
            var value = await readAsync();
            Debug.Write($"received:{value}");

            return value;
        }
       
        public async Task writeAsync(string str)
        {
            await myStream.WriteAsync(Encoding.ASCII.GetBytes(str + '\r'), 0, str.Length + 1);
            // stream.FlushAsync();
        }

        public async Task writeAll(string[] array)
        {
            foreach (var str in array)
            {
                Console.Write( await WriteReadAsync(str) + "|");
            }
        }

     

         async Task<byte[]> ReadBufferFromStreamAsync(NetworkStream stream)
        {
            var totalRead = 0;
            byte[] buffer = new byte[IO_BUFFER_SIZE];
          
            Debug.Write("try read from stream,");
          
            while (!buffer.Contains((byte)'>'))
            {
                var read = await stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead);
                totalRead += read;

                Debug.Write($"got buffer of {read} bytes,");
            }

            Debug.Write($"got total buffer of {totalRead} bytes,");
            return buffer;
        }

         async Task<string> readAsync()
        {
            string answer = System.Text.Encoding.UTF8.GetString(await ReadBufferFromStreamAsync(myStream));
            Debug.Write(answer);
            //while (!answer.Contains('>'));
            return answer.Trim(charsToTrim); 
        }

        static void getpairedLE()
        {
            foreach (var d in Bluetooth.GetPairedDevicesAsync().Result)
            {
                Debug.WriteLine($"paired device:{d.Id} {d.Name}");
            }
        }

        public List<string> GetPairedDevices() => BTclient.PairedDevices.Select(p => p.DeviceName).ToList();

        BluetoothDeviceInfo getPairedAndroidDongle(string name)
        {
            var bts = BTclient.PairedDevices;
            bts.Select(b => b.DeviceName).Dump();
            var devs = bts.Where(b => b.DeviceName.Contains(name));//.Where(b => b.DeviceAddress.ToString()=="asd");
            var dev = devs.FirstOrDefault();
            //dev.Dump();
            if (dev == null)
                //dev = discover(name);


                if (!dev.Authenticated)
                {
                    BluetoothSecurity.PairRequest(dev.DeviceAddress, "1234");
                }
            return dev;
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
            myStream.Close();
            if (devicetype == DeviceType.BT)
                BTclient.Close();
        }
    }
}
