using OBDEngine;
using System.Configuration;

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
      m_OBDSession = new OBDSession(() => PickFileOpen(".raw", "RawDevice data (.raw)|*.raw"));
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

        FormPageSettings? v_FormPageSettings = (FormPageSettings?)m_PageManager.GetFormPage(typeof(FormPageSettings));
        if (v_FormPageSettings is not null)
          v_FormPageSettings.RefreshDevices += V_FormPageSettings_RefreshDevices;

        FormPageTinker? v_FormPageTinker = (FormPageTinker?)m_PageManager.GetFormPage(typeof(FormPageTinker));
        if (v_FormPageTinker is not null)
          v_FormPageTinker.XMLCommandChanged += M_FormPageTinker_XMLCommandChanged;

        m_OBDSession.CommandExecuted += M_OBDSession_CommandExecuted;
        m_OBDSession.SessionExecuted += M_OBDSession_SessionExecuted;
        m_OBDSession.SessionValueExecuted += M_OBDSession_SessionValueExecuted;
        m_OBDSession.TinkerRawExecuted += M_OBDSession_TinkerRawExecuted;
        m_OBDSession.TinkerValueExecuted += M_OBDSession_TinkerValueExecuted;
        m_PageManager.Load();
        V_FormPageSettings_RefreshDevices();
      });
    }

    private void V_FormPageSettings_RefreshDevices()
    {
      string[]? v_Devices = m_OBDSession.GetPairedDevices().ToArray();
      m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.UpdateDevices(v_Devices));
    }

    protected override void OnClosed(EventArgs e)
    {
      try
      {
        this.SaveFormSizeState();
        base.OnClosed(e);
      }
      catch (Exception p_Exception)
      {
        GlobalErrorDisplay?.Invoke(p_Exception);
      }
    }

    private bool m_Running = false;

    private void btStart_Click(object sender, EventArgs e)
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
          FormPageSettings? v_FormPageSettings = (FormPageSettings?)m_PageManager.GetFormPage(typeof(FormPageSettings));
          if (v_FormPageSettings is not null)
          {
            string? v_DeviceName = v_FormPageSettings.CurrentDevice;
            if (v_DeviceName is not null)
            {
              m_CancellationTokenSource = new CancellationTokenSource();
#if DEBUGx
              m_OBDSession.LoadConfig(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "obd2_Taycan.xml"));
              if (Control.ModifierKeys == Keys.Control)
                v_DeviceName = "RawDevice";
              m_OBDSession.Initialise(v_DeviceName, false);
#else
              m_OBDSession.LoadConfig(PickFileOpen(".xml", "Engine File (.xml)|*.xml"));
              //always write to raw, unless specified in config file...
              m_OBDSession.Initialise(v_DeviceName, true);
#endif
              ControlExtensions.WriteAppSetting("LastUsedDevice", v_DeviceName);
              m_PageManager.ActivateFormPage(typeof(FormPagePower));
              btStart.Text = "Stop";
              m_Running = true;
              m_OBDSession.Execute(m_CancellationTokenSource.Token);
            }
          }
        }
      });
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
          btStart.Text = "Start";
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
      ExceptionCheck(() => m_PageManager.ActivateFormPage(typeof(FormPageSettings)));
    }

    private void btTinker_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.ActivateFormPage(typeof(FormPageTinker)));
    }

    private void btPower_Click(object sender, EventArgs e)
    {
      ExceptionCheck(() => m_PageManager.ActivateFormPage(typeof(FormPagePower)));
    }

    private void btLogger_Click(object sender, EventArgs e)
    {
      //testing .... ExceptionCheck(() => throw new Exception("dddd"));

      //ExceptionCheck(() =>  m_PageManager.ActivateFormPage(typeof(FormPageLogger)));
    }

    private void M_FormPageTinker_XMLCommandChanged(string p_XmlCommand)
    {
      string? v_Result = m_OBDSession.LoadTinkerCommand(p_XmlCommand);
      if (v_Result is not null)
      {
        FormPageTinker? v_FormPageTinker = (FormPageTinker?)m_PageManager.GetFormPage(typeof(FormPageTinker));
        if (v_FormPageTinker is not null)
          v_FormPageTinker?.AddErrorMessage(v_Result);
      }
    }

    private void M_OBDSession_TinkerRawExecuted(byte[] p_ResultRaw, byte[] p_ResultProcessed)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.ShowResultRaw(p_ResultRaw, p_ResultProcessed)));
    }

    private void M_OBDSession_TinkerValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.ShowResultValue(p_Name, p_Units, p_Value)));
    }

    private void M_OBDSession_SessionValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      this.InvokeIfRequired(() => m_PageManager.Pages.ForEach(l_FormPage => l_FormPage.SessionValueExecuted(p_Name, p_Units, p_Value)));
    }
  }
}
