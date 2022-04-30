namespace TaycanLogger
{
  internal class FormPageSettings : FormPage
  {
    private FormPageSettingsControl m_FormPageSettingsControl;

    public FormPageSettings(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      m_FormPageSettingsControl = new FormPageSettingsControl();
      m_FormPageSettingsControl.RefreshPressed += M_FormPageSettingsControl_RefreshPressed;
      m_FormPageSettingsControl.StartPressed += M_FormPageSettingsControl_StartPressed;
      if (Form1.GlobalErrorDisplay is null)
        Form1.GlobalErrorDisplay = p_Exception => AddErrorMessage(p_Exception);
    }

    public override void Load()
    {
      base.Load();
      m_FormPageSettingsControl.BackColor = System.Drawing.Color.Black;
      m_FormPageSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
      m_FormPageSettingsControl.Font = Parent.Font;
      m_FormPageSettingsControl.ForeColor = System.Drawing.Color.White;
      m_FormPageSettingsControl.Margin = new System.Windows.Forms.Padding(4);
      m_FormPageSettingsControl.Name = "FormPageSettingsControl";
      m_FormPageSettingsControl.TabIndex = 9;
      ColumnCount = 1;
      ColumnStyles.Add(new ColumnStyle());
      RowCount = 1;
      RowStyles.Add(new RowStyle());
      Controls.Add(m_FormPageSettingsControl, 0, 0);
    }

    internal void SetStartStop(bool p_Stop)
    {
      m_FormPageSettingsControl.SetStartStop(p_Stop);

    }

    public event Action RefreshDevices;

    private void M_FormPageSettingsControl_RefreshPressed()
    {
      RefreshDevices();
    }

    public event Action StartLogging;


    private void M_FormPageSettingsControl_StartPressed()
    {
      StartLogging();
    }

    public void AddErrorMessage(Exception p_Exception)
    {
      this.InvokeIfRequired(() =>
      {
        m_FormPageSettingsControl.AddError(p_Exception);
        OnActivateRequested();
      });
    }

    public void AddInitResult(string p_Message)
    {
      if (!string.IsNullOrEmpty(p_Message))
      {
        string v_Message = p_Message.Replace((char)13, '¬');
        m_FormPageSettingsControl.AddLine(v_Message);
      }
    }

    public (string Name, ulong Addess) GetCurrentDevice()
    {
      return m_FormPageSettingsControl.GetCurrentDevice();
    }

    public void UpdateDevices(List<(string Name, ulong Addess, DateTime LastSeen)> p_Devices)
    {
      m_FormPageSettingsControl.UpdateDevices(p_Devices, ControlExtensions.ReadAppSetting("LastUsedDevice"));
    }
  }
}