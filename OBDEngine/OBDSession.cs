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
    private Func<string, string, string?> m_GetFilename;
    private SemaphoreSlim m_SemaphoreSlimTinker;
    private OBDCommand? m_TinkerOBDCommand;

    public static Action<Exception>? GlobalErrorDisplay;

    public static void ExceptionCheck(Action p_SafeCall)
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

    public static T ExceptionCheck<T>(Func<T> p_SafeCall)
    {
      try
      {
        return p_SafeCall();
      }
      catch (Exception p_Exception)
      {
        GlobalErrorDisplay?.Invoke(p_Exception);
      }
      return default(T);
    }

    public OBDSession(Func<string, string, string?> p_GetFilename)
    {
      m_GetFilename = p_GetFilename;
      m_SemaphoreSlimTinker = new SemaphoreSlim(1);
    }

    public List<(string Name, ulong Addess, DateTime LastSeen)> GetPairedDevices() => new BluetoothClient().PairedDevices.Select<BluetoothDeviceInfo, (string Name, ulong Addess, DateTime LastSeen)>(bdi => (bdi.DeviceName, bdi.DeviceAddress.ToUInt64(), DateTime.Now)).ToList();

    private CtlBinaryWriter? m_CtlBinaryWriter;
    private CtlBinaryReader? m_CtlBinaryReader;

    public void Initialise(string p_DeviceName, ulong p_DeviceAddress, bool p_WriteToRaw)
    {
      //clean up this mess! Remove RAW from the project, can get it back via GIT in some crazy emergency...
      if (p_DeviceName == "RawDevice")
      {
        string? v_UserFilename = m_GetFilename(".xml", "OBD Engine File (.xml)|*.xml");
        XDocument v_XDocumentRaw = XDocument.Load(v_UserFilename);
        CtlFileSetup v_CtlFileSetup = new CtlFileSetup();
        string? v_Error = v_CtlFileSetup.CheckValidityAndEnrichWithmcid4ctl(v_XDocumentRaw);
        if (!string.IsNullOrEmpty(v_Error))
          throw new Exception("User OBD XML config file is not valid: " + v_Error);
        m_InitOBDCommands = (from p_XElement in v_XDocumentRaw.Root.Elements("init").Elements("command") select new OBDCommandInit(p_XElement)).ToList();
        m_LoopOBDCommands = (from p_XElement in v_XDocumentRaw.Root.Elements("rotation").Elements("command") select new OBDCommand(p_XElement)).ToList();

        string? v_RawFilename = m_GetFilename(".raw", "RawDevice data (.raw)|*.raw");
        if (!string.IsNullOrEmpty(v_RawFilename))
          m_Stream = new ReadRawStream(v_RawFilename);
        //we never do this, delete this comment code later...
        //this creates a copy of the raw stream. Used for debugging and can be deleted...
        //if (p_WriteToRaw)
        //  m_Stream = new WriteRawStream(p_DeviceName, m_Stream);
        return;
      }



      m_CtlBinaryWriter = null;
      if (p_DeviceAddress == ulong.MaxValue)
      {
        string? v_CtlFilename = m_GetFilename(".ctl", "Taycan Logger data file (.ctl)|*.ctl");
        if (!string.IsNullOrEmpty(v_CtlFilename))
          m_CtlBinaryReader = new CtlBinaryReader(v_CtlFilename);
      }
      else
        m_CtlBinaryReader = null;

      //this code is only used for RAW or ODB Dongle, not CTL playback
      //get the master and set it up to run.
      XDocument v_XDocument;
      if (m_CtlBinaryReader is not null)
        v_XDocument = OBDMasterInfo.GetMasterXDocument(m_CtlBinaryReader.Version);
      else
        v_XDocument = OBDMasterInfo.GetMasterXDocument(OBDMasterInfo.GetLatestVersion());
      m_InitOBDCommands = (from p_XElement in v_XDocument.Root.Elements("init").Elements("command") select new OBDCommandInit(p_XElement)).ToList();
      m_LoopOBDCommands = (from p_XElement in v_XDocument.Root.Elements("rotation").Elements("command") select new OBDCommand(p_XElement)).ToList();

      if (m_CtlBinaryReader is not null)
      {
        v_XDocument = m_CtlBinaryReader.LoadUserXml();
        if (v_XDocument is not null)
        {
          m_InitOBDCommands.AddRange((from p_XElement in v_XDocument.Root.Elements("init").Elements("command") select new OBDCommandInit(p_XElement)).ToList());
          m_LoopOBDCommands.AddRange((from p_XElement in v_XDocument.Root.Elements("rotation").Elements("command") select new OBDCommand(p_XElement)).ToList());
        }
      }
      else
      {
        string? v_UserFilename = m_GetFilename(".xml", "OBD Engine File (.xml)|*.xml");
        if (!string.IsNullOrEmpty(v_UserFilename))
        {
          //load user xml, but filter out duplicates on init
          v_XDocument = XDocument.Load(v_UserFilename);
          CtlFileSetup v_CtlFileSetup = new CtlFileSetup();
          string? v_Error = v_CtlFileSetup.CheckValidityAndEnrichWithmcid4ctl(v_XDocument);
          if (!string.IsNullOrEmpty(v_Error))
            throw new Exception("User OBD XML config file is not valid: " + v_Error);
          m_InitOBDCommands.AddRange((from p_XElement in v_XDocument.Root.Elements("init").Elements("command") select new OBDCommandInit(p_XElement)).ToList());
          m_LoopOBDCommands.AddRange((from p_XElement in v_XDocument.Root.Elements("rotation").Elements("command") select new OBDCommand(p_XElement)).ToList());

          //if we want to record, we create a binary writer...
          m_CtlBinaryWriter = new CtlBinaryWriter(p_DeviceName, v_XDocument);
        }
        else
          m_CtlBinaryWriter = new CtlBinaryWriter(p_DeviceName);
      }

      if (m_CtlBinaryReader is null)
      {
        m_OBDDevice = new OBDDevice();
        m_OBDDevice.Open(p_DeviceAddress);
        m_Stream = m_OBDDevice.Stream;
        if (m_CtlBinaryWriter is null)
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
      m_CtlBinaryWriter?.Dispose();
      m_CtlBinaryWriter = null;
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
      Task.Run(() =>
      {
        try
        {
          if (m_CtlBinaryReader is null)
            ExecuteDevice(p_CancellationToken);
          else
            ExecuteCtlPlayback(p_CancellationToken);
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

    private void ExecuteDevice(CancellationToken p_CancellationToken)
    {
      if (m_Stream is null)
        return;
      byte[] v_Buffer = new byte[4096];
      string v_Header = string.Empty;
      foreach (var l_OBDCommand in m_InitOBDCommands)
      {
        l_OBDCommand.Execute(m_Stream, v_Buffer, false, p_BytesRead => ProcessInitRaw(l_OBDCommand, v_Buffer, p_BytesRead));
        v_Header = l_OBDCommand.Send;
        if (p_CancellationToken.IsCancellationRequested)
          break;
      }
      SessionInitCompleted?.Invoke();
      ulong v_CommandLoopIndex = 0;
      OBDCommand? v_TinkerOBDCommand = null;
      bool v_IsReadRawStream = m_Stream is ReadRawStream;
      while (!p_CancellationToken.IsCancellationRequested)
      {
        v_CommandLoopIndex++;
        foreach (var l_OBDCommand in m_LoopOBDCommands)
        {
          v_TinkerOBDCommand = SetTinkerCommand(v_TinkerOBDCommand);
          // not a RAW stream but have a tinker command, so just execute it.
          if (!v_IsReadRawStream && v_TinkerOBDCommand is not null)
          {
            bool v_Error = !v_TinkerOBDCommand.Execute(m_Stream, v_Buffer, v_Header != v_TinkerOBDCommand.Header, (p_OBDValue, p_Value) => ProcessTinkerValue(p_OBDValue, p_Value), null, (p_ResultRaw, p_ResultProcessed) => ProcessTinkerRaw(p_ResultRaw, p_ResultProcessed));
            v_Header = v_TinkerOBDCommand.Header;
            CommandExecuted?.Invoke(v_Error);
            v_TinkerOBDCommand = null;
          }
          if (v_CommandLoopIndex % (ulong)l_OBDCommand.SkipCount == 0)
          {
            bool v_Error = false;
            // running a RAW stream, have a tinker command, and matching to current command, so execute it instead of the current command.
            if (v_IsReadRawStream && v_TinkerOBDCommand is not null && l_OBDCommand == v_TinkerOBDCommand)
            {
              v_Error = !v_TinkerOBDCommand.Execute(m_Stream, v_Buffer, v_Header != v_TinkerOBDCommand.Header, (p_OBDValue, p_Value) => ProcessTinkerValue(p_OBDValue, p_Value), null, (p_ResultRaw, p_ResultProcessed) => ProcessTinkerRaw(p_ResultRaw, p_ResultProcessed));
              v_TinkerOBDCommand = null;
            }
            else
              v_Error = !l_OBDCommand.Execute(m_Stream, v_Buffer, v_Header != l_OBDCommand.Header, (p_OBDValue, p_Value) => ProcessSessionValue(p_OBDValue, p_Value), p_BytesRead => ProcessRaw(l_OBDCommand, v_Buffer, p_BytesRead));
            v_Header = l_OBDCommand.Header;
            CommandExecuted?.Invoke(v_Error);
            if (p_CancellationToken.IsCancellationRequested)
              break;
          }
        }
      }
    }


    private void ExecuteCtlPlayback(CancellationToken p_CancellationToken)
    {
      Dictionary<int, OBDCommandInit> v_OBDInitCommands = new Dictionary<int, OBDCommandInit>();
      m_InitOBDCommands?.ForEach(p => v_OBDInitCommands.Add(p.ID, p));
      Dictionary<int, OBDCommand> v_OBDLoopCommands = new Dictionary<int, OBDCommand>();
      m_LoopOBDCommands?.ForEach(p => v_OBDLoopCommands.Add(p.ID, p));
      bool v_InitCompleted = false;
      while (!p_CancellationToken.IsCancellationRequested)
      {
        byte[] v_Buffer = new byte[4096];
        var v_Result = m_CtlBinaryReader.Read(v_Buffer, v_Buffer.Length);
        if (v_Result.Mcid4ctl == 0)
          break;
        if (!v_OBDLoopCommands.ContainsKey(v_Result.Mcid4ctl))
        {
          OBDCommandInit v_OBDCommandInit = v_OBDInitCommands[v_Result.Mcid4ctl];
          v_OBDCommandInit.Execute(v_Buffer, v_Result.Count, p_BytesRead => ProcessInitRaw(v_OBDCommandInit, v_Buffer, p_BytesRead));
        }
        else
        {
          if (!v_InitCompleted)
          {
            v_InitCompleted = true;
            SessionInitCompleted?.Invoke();
          }
          OBDCommand v_OBDCommand = v_OBDLoopCommands[v_Result.Mcid4ctl];
          bool v_Error = !v_OBDCommand.Execute(v_Buffer, v_Result.Count, (p_OBDValue, p_Value) => ProcessSessionValue(p_OBDValue, p_Value), p_BytesRead => ProcessRaw(v_OBDCommand, v_Buffer, p_BytesRead));
          CommandExecuted?.Invoke(v_Error);
        }
      }
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

    private void ProcessInitRaw(OBDCommandInit p_OBDCommand, byte[] p_ResultRaw, int p_BytesRead)
    {
      m_CtlBinaryWriter?.Write(p_OBDCommand.ID, p_ResultRaw, 0, p_BytesRead);
      SessionInitExecuted?.Invoke(System.Text.Encoding.UTF8.GetString(p_ResultRaw, 0, p_BytesRead));
    }

    private void ProcessRaw(OBDCommand p_OBDCommand, byte[] p_ResultRaw, int p_BytesRead)
    {
      m_CtlBinaryWriter?.Write(p_OBDCommand.ID, p_ResultRaw, 0, p_BytesRead);
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