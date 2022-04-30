using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace OBDEngine
{
  public class OBDDevice
  {
    private BluetoothClient? m_BluetoothClient;
    private Stream? m_Stream;
    public Stream Stream { get => m_Stream; }

    public void Open(ulong p_DeviceAddress)
    {
      m_BluetoothClient = new();
      BluetoothDeviceInfo v_BluetoothDeviceInfo = m_BluetoothClient.PairedDevices.Where(x => x.DeviceAddress.ToUInt64() == p_DeviceAddress).FirstOrDefault();
      if (!v_BluetoothDeviceInfo.Authenticated)
        BluetoothSecurity.PairRequest(v_BluetoothDeviceInfo.DeviceAddress, "1234");
      if (!m_BluetoothClient.Connected)
        m_BluetoothClient.Connect(v_BluetoothDeviceInfo.DeviceAddress, BluetoothService.SerialPort);
      if (m_BluetoothClient.Connected)
        m_Stream = m_BluetoothClient.GetStream();
    }

    public void Close()
    {
      m_BluetoothClient?.Dispose();
    }
  }
}