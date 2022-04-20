namespace TaycanLogger
{
  public class FormPage : TableLayoutPanel
  {
    public bool Activated { get => this.Visible; }
    public virtual Type Type { get => this.GetType(); }

    public event Action<FormPage>? ActivateRequested;

    public FormPage(int p_ColumnSpan)
    {
      SetColumnSpan(this, p_ColumnSpan);
      Dock = System.Windows.Forms.DockStyle.Fill;
      Location = new System.Drawing.Point(0, 0);
      Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
      Name = this.GetType().Name;
      Size = new System.Drawing.Size(914, 568);
      TabIndex = 0;
      Visible = false;
    }

    public virtual void Load() { }
    public virtual void Unload() { }
    public virtual void Activate() { this.Visible = true; }
    public virtual void Deactivate() { this.Visible = false; }


    public virtual void UpdateDevices(string[]? p_Devices) { }
    public virtual void CommandExecuted(bool p_Error) { }
    public virtual void SessionValueExecuted(string p_Name, string p_Units, double p_Value) { }


    public virtual void ShowResultValue(string p_Name, string p_Units, double p_Value) { }
    public virtual void ShowResultRaw(byte[] p_ResultRaw, byte[] p_ResultProcessed) { }

    protected virtual void OnActivateRequested()
    {
      ActivateRequested?.Invoke(this);
    }

  }
}