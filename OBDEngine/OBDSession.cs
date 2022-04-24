using InTheHand.Net.Sockets;
using System.Xml.Linq;

namespace OBDEngine
{
  public class OBDSession
  {
    private List<OBDCommandInit>? m_InitOBDCommands;
    private List<OBDCommand>? m_LoopOBDCommands;
    private OBDDevice? m_OBDDevice;
    private Stream? m_Stream;
    private Func<string?> m_RawFilename;
    private SemaphoreSlim m_SemaphoreSlimTinker;
    private OBDCommand? m_TinkerOBDCommand;

    public OBDSession(Func<string?> p_RawFilename)
    {
      m_RawFilename = p_RawFilename;
      m_SemaphoreSlimTinker = new SemaphoreSlim(1);
    }

    public static Action<Exception>? GlobalErrorDisplay;

    public void ExceptionCheck(Action p_SafeCall)
    {
      try
      {
        p_SafeCall();
      }
      catch (Exception p_Exception)
      {
        GlobalErrorDisplay?.Invoke(p_Exception);
      }
    }

    public List<string> GetPairedDevices() => new BluetoothClient().PairedDevices.Select<BluetoothDeviceInfo, string>(bdi => bdi.DeviceName).Append("RawDevice").ToList();

    public void LoadConfig(string p_Filename)
    {
      if (!string.IsNullOrEmpty(p_Filename) && File.Exists(p_Filename))
      {
        XDocument v_XDocument = XDocument.Load(p_Filename);
        m_InitOBDCommands = (from p_XElement in v_XDocument.Root.Elements("init").Elements("command") select new OBDCommandInit(p_XElement)).ToList();
        m_LoopOBDCommands = (from p_XElement in v_XDocument.Root.Elements("rotation").Elements("command") select new OBDCommand(p_XElement)).ToList();
      }
    }

    public void Initialise(string p_DeviceName, bool p_WriteToRaw)
    {
      if (p_DeviceName == "RawDevice")
      {
        string? v_Filename = m_RawFilename();
        if (!string.IsNullOrEmpty(v_Filename))
          m_Stream = new ReadRawStream(v_Filename);
        //this creates a copy of the raw stream. Used for debugging and can be deleted...
        //if (p_WriteToRaw)
        //  m_Stream = new WriteRawStream(p_DeviceName, m_Stream);
      }
      else
      {
        m_OBDDevice = new OBDDevice();
        m_OBDDevice.Open(p_DeviceName);
        m_Stream = m_OBDDevice.Stream;
        if (p_WriteToRaw)
          m_Stream = new WriteRawStream(p_DeviceName, m_Stream);
      }
    }

    public void Close()
    {
      if (m_Stream is WriteRawStream)
        m_Stream.Close();
      if (m_Stream is ReadRawStream)
        m_Stream.Close();
      m_OBDDevice?.Close();
    }


    public event Action<bool>? CommandExecuted;
    public event Action? SessionExecuted;
    public event Action<string>? SessionInitExecuted;
    public event Action? SessionInitCompleted;
    public event Action<string, string, double>? SessionValueExecuted;
    public event Action<byte[], byte[]>? TinkerRawExecuted;
    public event Action<string, string, double>? TinkerValueExecuted;

    public void Execute(CancellationToken p_CancellationToken)
    {
      if (m_Stream is not null)
        Task.Run(() =>
        {
          try
          {
            byte[] v_Buffer = new byte[4096];
            foreach (var l_OBDCommand in m_InitOBDCommands)
            {
              l_OBDCommand.Execute(m_Stream, v_Buffer, false, p_BytesRead => ProcessInitRaw(v_Buffer, p_BytesRead));
              if (p_CancellationToken.IsCancellationRequested)
                break;
            }
            SessionInitCompleted?.Invoke();
            ulong v_CommandLoopIndex = 0;
            string v_Header = "7E5";
            OBDCommand? v_TinkerOBDCommand = null;
            while (!p_CancellationToken.IsCancellationRequested)
            {
              v_CommandLoopIndex++;

#if DEVELOPTINKERWITHRAW
              #region DEVELOPTINKERWITHRAW

//only use this code to test tinker with recorded data...

              foreach (var l_OBDCommand in m_LoopOBDCommands)
              {
                if (v_CommandLoopIndex % (ulong)l_OBDCommand.SkipCount == 0)
                {
                  v_TinkerOBDCommand = SetTinkerCommand(v_TinkerOBDCommand);

                  //just for testing on the raw stream, to fire the tinker result and ignoring the regular command...
                  if (v_TinkerOBDCommand is not null && l_OBDCommand == v_TinkerOBDCommand)
                  {
                    v_TinkerOBDCommand.Execute(m_Stream, v_Buffer, v_Header != v_TinkerOBDCommand.Header, (p_OBDValue, p_Value) => ProcessTinkerValue(p_OBDValue, p_Value), (p_ResultRaw, p_ResultProcessed) => ProcessTinkerRaw(p_ResultRaw, p_ResultProcessed));
                    v_TinkerOBDCommand = null;
                  }
                  else
                  {
                    bool v_Error = !l_OBDCommand.Execute(m_Stream, v_Buffer, v_Header != l_OBDCommand.Header, (p_OBDValue, p_Value) => ProcessSessionValue(p_OBDValue, p_Value));
                    CommandExecuted?.Invoke(v_Error);
                  }

                  v_Header = l_OBDCommand.Header;
                  if (p_CancellationToken.IsCancellationRequested)
                    break;
                }
              }

              #endregion
#else

              foreach (var l_OBDCommand in m_LoopOBDCommands)
              {
                if (v_CommandLoopIndex % (ulong)l_OBDCommand.SkipCount == 0)
                {
                  bool v_Error = !l_OBDCommand.Execute(m_Stream, v_Buffer, v_Header != l_OBDCommand.Header, (p_OBDValue, p_Value) => ProcessSessionValue(p_OBDValue, p_Value));
                  v_Header = l_OBDCommand.Header;
                  CommandExecuted?.Invoke(v_Error);
                  if (p_CancellationToken.IsCancellationRequested)
                    break;
                }
              }

              v_TinkerOBDCommand = SetTinkerCommand();
              if (v_TinkerOBDCommand is not null)
              {
                v_TinkerOBDCommand.Execute(m_Stream, v_Buffer, v_Header != v_TinkerOBDCommand.Header, (p_OBDValue, p_Value) => ProcessTinkerValue(p_OBDValue, p_Value), (p_ResultRaw, p_ResultProcessed) => ProcessTinkerRaw(p_ResultRaw, p_ResultProcessed));
                v_Header = v_TinkerOBDCommand.Header;
                v_TinkerOBDCommand = null;
              }

#endif

            }
          }
          catch (Exception p_Exception)
          {
            GlobalErrorDisplay?.Invoke(p_Exception);
          }
          finally
          {
            SessionExecuted?.Invoke();
          }
        });
    }

    public string? LoadTinkerCommand(string p_XmlCommand)
    {
      try
      {
        XDocument v_XDocument = XDocument.Load(new StringReader(p_XmlCommand), LoadOptions.None);
        OBDCommand v_OBDCommand = new OBDCommand(v_XDocument.Root);
        //make it executed and get results to tinker window...
        if (m_SemaphoreSlimTinker.Wait(10000))
          try
          {
            m_TinkerOBDCommand = v_OBDCommand;
          }
          finally
          {
            m_SemaphoreSlimTinker.Release();
          }
      }
      catch (Exception p_Exception)
      {
        return p_Exception.Message;
      }
      return null;
    }

    private OBDCommand? SetTinkerCommand(OBDCommand? p_TinkerOBDCommand = null)
    {
      if (p_TinkerOBDCommand is not null)
        return p_TinkerOBDCommand;
      OBDCommand? v_TinkerOBDCommand = null;
      if (m_SemaphoreSlimTinker.Wait(0))
        try
        {
          if (m_TinkerOBDCommand is not null)
          {
            v_TinkerOBDCommand = m_TinkerOBDCommand;
            m_TinkerOBDCommand = null;
          }
        }
        finally
        {
          m_SemaphoreSlimTinker.Release();
        }
      return v_TinkerOBDCommand;
    }

    private void ProcessInitRaw(byte[] p_ResultRaw, int p_BytesRead)
    {
      SessionInitExecuted?.Invoke(System.Text.Encoding.UTF8.GetString(p_ResultRaw, 0, p_BytesRead));
    }

    private void ProcessSessionValue(OBDValue p_OBDValue, double p_Value)
    {

      // fire off events into the UI which are cross threaded, so need to be handled with care...
      // ideally put them together into a 
      // Pipelines to make sure we never have a back pressure problem

      SessionValueExecuted?.Invoke(p_OBDValue.Name, p_OBDValue.Units, p_Value);

    }

    private void ProcessTinkerRaw(byte[] p_ResultRaw, byte[] p_ResultProcessed)
    {
      TinkerRawExecuted?.Invoke(p_ResultRaw, p_ResultProcessed);
    }

    private void ProcessTinkerValue(OBDValue p_OBDValue, double p_Value)
    {
      TinkerValueExecuted?.Invoke(p_OBDValue.Name, p_OBDValue.Units, p_Value);
    }
  }
}