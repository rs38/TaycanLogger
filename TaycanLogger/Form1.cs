using OBDEngine;
using System.Configuration;
using System.Runtime.InteropServices;

namespace TaycanLogger
{
  public partial class Form1 : Form
  {
    private OBDSession m_OBDSession;
    private FormPages m_PageManager;
    private CancellationTokenSource? m_CancellationTokenSource;

    public Form1()
    {
      FormControlGlobals.LoadFonts();
      if (OBDSession.GlobalErrorDisplay is null)
        OBDSession.GlobalErrorDisplay = p_Exception => GlobalErrorDisplay?.Invoke(p_Exception);
      InitializeComponent();
      m_PageManager = new FormPages(this, this.tableLayoutPanel1);
      m_OBDSession = new OBDSession((p_DefaultExt, p_Filter) => PickFileOpen(p_DefaultExt, p_Filter));
    }

    [DllImport("DwmApi")] //System.Runtime.InteropServices
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

    protected override void OnHandleCreated(EventArgs e)
    {
      if (Environment.OSVersion.Version.Major >= 10 && Environment.OSVersion.Version.Build >= 17763)
        if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
          DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
    }

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

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      ExceptionCheck(() =>
      {
        this.LoadFormSizeState();
        FormPageSettings v_FormPageSettings = m_PageManager.GetFormPage<FormPageSettings>();
        v_FormPageSettings.RefreshDevices += V_FormPageSettings_RefreshDevices;
        v_FormPageSettings.StartLogging += V_FormPageSettings_StartLogging;
        m_PageManager.GetFormPage<FormPageTinker>().XMLCommandChanged += M_FormPageTinker_XMLCommandChanged;
        m_OBDSession.CommandExecuted += M_OBDSession_CommandExecuted;
        m_OBDSession.SessionExecuted += M_OBDSession_SessionExecuted;
        m_OBDSession.SessionInitExecuted += M_OBDSession_SessionInitExecuted;
        m_OBDSession.SessionInitCompleted += M_OBDSession_SessionInitCompleted;
        m_OBDSession.SessionValueExecuted += M_OBDSession_SessionValueExecuted;
        m_OBDSession.TinkerRawExecuted += M_OBDSession_TinkerRawExecuted;
        m_OBDSession.TinkerValueExecuted += M_OBDSession_TinkerValueExecuted;
        m_PageManager.Load();
        V_FormPageSettings_RefreshDevices();
      });
    }

    private void V_FormPageSettings_RefreshDevices()
    {
      List<(string Name, ulong Addess, DateTime LastSeen)> v_Devices = m_OBDSession.GetPairedDevices();
      m_PageManager.GetFormPage<FormPageSettings>().UpdateDevices(v_Devices);
    }

    private bool m_Running = false;

    private void V_FormPageSettings_StartLogging()
    {
      ExceptionCheck(() =>
      {
        if (m_Running)
        {
          m_CancellationTokenSource?.Cancel();
        }
        else
        {
          if (m_CancellationTokenSource == null)
            m_CancellationTokenSource?.Cancel();
          FormPageSettings v_FormPageSettings = m_PageManager.GetFormPage<FormPageSettings>();
          var v_CurrentDevice = v_FormPageSettings.GetCurrentDevice();
          m_CancellationTokenSource = new CancellationTokenSource();
#if DEBUG
          //if (Control.ModifierKeys == Keys.Control)
          //  v_DeviceName = "RawDevice";
          //m_OBDSession.Initialise(v_DeviceName, Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "obd2_Taycan.xml"), false);
          //use null to test only running the master xml
          m_OBDSession.Initialise(v_CurrentDevice.Name, v_CurrentDevice.Addess, true);
#else
              //always write to raw, unless specified in config file...
              m_OBDSession.Initialise(v_CurrentDevice.Name, v_CurrentDevice.Addess, true);
#endif
          ControlExtensions.WriteAppSetting("LastUsedDevice", v_CurrentDevice.Addess.ToString());
          v_FormPageSettings.SetStartStop(true);
          m_Running = true;
          m_OBDSession.Execute(m_CancellationTokenSource.Token);
        }
      });
    }

    protected override void OnClosed(EventArgs e)
    {
      try
      {
        m_OBDSession.Close();
        this.SaveFormSizeState();
        base.OnClosed(e);
      }
      catch (Exception p_Exception)
      {
        GlobalErrorDisplay?.Invoke(p_Exception);
      }
    }

    private void M_OBDSession_CommandExecuted(bool p_Error)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.CommandExecuted(p_Error)));
    }

    private void M_OBDSession_SessionExecuted()
    {
      this.InvokeIfRequired(() =>
      {
        try
        {
          m_OBDSession.Close();
        }
        catch (Exception p_Exception)
        {
          GlobalErrorDisplay?.Invoke(p_Exception);
        }
        finally
        {
          m_CancellationTokenSource = null;
          FormPageSettings v_FormPageSettings = m_PageManager.GetFormPage<FormPageSettings>();
          v_FormPageSettings.SetStartStop(false);
          m_Running = false;
        }
      });
    }

    private string? PickFileOpen(string p_DefaultExt, string p_Filter)
    {
      var dialog = new OpenFileDialog();
      dialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
      dialog.FileName = string.Empty;
      dialog.DefaultExt = p_DefaultExt;
      dialog.Filter = p_Filter;
      var result = dialog.ShowDialog();
      if (result == DialogResult.OK)
        return dialog.FileName;
      return null;
    }

    private void btLeft_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.RotateLeft());
    }

    private void btRight_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.RotateRight());
    }

    private void btSettings_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.ActivateFormPage<FormPageSettings>());
    }

    private void btTinker_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.ActivateFormPage<FormPageTinker>());
    }

    private void btPower_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.ActivateFormPage<FormPagePower>());
    }

    private void btBattery_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.ActivateFormPage<FormPageBattery>());
    }

    private void btLogger_Click(object sender, EventArgs e)
    {
      //testing .... ExceptionCheck(() => throw new Exception("dddd"));

      ExceptionCheck(() => m_PageManager.ActivateFormPage<FormPageLogger>());
    }

    private void M_OBDSession_SessionInitExecuted(string p_InitResult)
    {
      this.InvokeIfRequired(() => m_PageManager.GetFormPage<FormPageSettings>().AddInitResult(p_InitResult));
    }

    private void M_OBDSession_SessionInitCompleted()
    {
      this.InvokeIfRequired(() => m_PageManager.ActivateFormPage<FormPagePower>());
    }

    private void M_OBDSession_SessionValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.SessionValueExecuted(p_Name, p_Units, p_Value)));
    }

    private void M_FormPageTinker_XMLCommandChanged(string p_XmlCommand)
    {
      string? v_Result = m_OBDSession.LoadTinkerCommand(p_XmlCommand);
      if (v_Result is not null)
        m_PageManager.GetFormPage<FormPageTinker>().AddErrorMessage(v_Result);
    }

    private void M_OBDSession_TinkerRawExecuted(byte[] p_ResultRaw, byte[] p_ResultProcessed)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.ShowResultRaw(p_ResultRaw, p_ResultProcessed)));
    }

    private void M_OBDSession_TinkerValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.ShowResultValue(p_Name, p_Units, p_Value)));
    }
  }
}
