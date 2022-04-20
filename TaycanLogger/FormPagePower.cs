namespace TaycanLogger
{
  public class FormPagePower : FormPage
  {
    public override Type Type { get => this.GetType(); }

    private SpeedMeter m_SpeedMeter;
    private PowerGauge m_PowerGauge;
    private AmpereGauge m_AmpereGauge;
    private VoltGauge m_VoltGauge;
    private ConsumptionGauge m_ConsumptionGauge;
    private SpeedSoCGauge m_SpeedSoCGauge;


    public FormPagePower(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      m_SpeedMeter = new SpeedMeter();
      m_AmpereGauge = new AmpereGauge();
      m_PowerGauge = new PowerGauge();
      m_VoltGauge = new VoltGauge();
      m_ConsumptionGauge = new ConsumptionGauge();
      m_SpeedSoCGauge = new SpeedSoCGauge();
    }

    public override void Load()
    {
      base.Load();
      m_SpeedMeter.Font = Parent.Font;
      m_AmpereGauge.Font = Parent.Font;
      m_PowerGauge.Font = Parent.Font;
      m_VoltGauge.Font = Parent.Font;
      m_ConsumptionGauge.Font = Parent.Font;
      m_SpeedSoCGauge.Font = Parent.Font;
      m_SpeedMeter.ForeColor = Color.White;
      m_AmpereGauge.ForeColor = Color.White;
      m_PowerGauge.ForeColor = Color.White;
      m_VoltGauge.ForeColor = Color.White;
      m_ConsumptionGauge.ForeColor = Color.White;
      m_SpeedSoCGauge.ForeColor = Color.White;
      m_SpeedMeter.BackColor = Color.Black;
      m_AmpereGauge.BackColor = Color.Black;
      m_PowerGauge.BackColor = Color.Black;
      m_VoltGauge.BackColor = Color.Black;
      m_ConsumptionGauge.BackColor = Color.Black;
      m_SpeedSoCGauge.BackColor = Color.Black;
      m_PowerGauge.Dock = System.Windows.Forms.DockStyle.Fill;
      m_SpeedMeter.Dock = System.Windows.Forms.DockStyle.Fill;
      m_AmpereGauge.Dock = System.Windows.Forms.DockStyle.Fill;
      m_VoltGauge.Dock = System.Windows.Forms.DockStyle.Fill;
      m_ConsumptionGauge.Dock = System.Windows.Forms.DockStyle.Fill;
      m_SpeedSoCGauge.Dock = System.Windows.Forms.DockStyle.Fill;
      ColumnCount = 4;
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
      RowCount = 3;
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
      SetColumnSpan(m_SpeedMeter, 2);
      Controls.Add(m_SpeedMeter, 1, 0);
      Controls.Add(m_AmpereGauge, 0, 1);
      SetColumnSpan(m_PowerGauge, 2);
      Controls.Add(m_PowerGauge, 1, 1);
      Controls.Add(m_VoltGauge, 2, 1);
      SetColumnSpan(m_SpeedSoCGauge, 2);
      Controls.Add(m_SpeedSoCGauge, 0, 2);
      SetColumnSpan(m_ConsumptionGauge, 2);
      Controls.Add(m_ConsumptionGauge, 2, 2);
    }

    private uint m_CommandExecutedCount = 0;
    private uint m_CommandErrorCount = 0;

    public override void CommandExecuted(bool p_Error)
    {
      if (p_Error)
      {
        m_CommandErrorCount++;

      }
      m_CommandExecutedCount++;
    }

    private double m_LastVoltageValue = double.NaN;
    private DateTime m_LastVoltageTime = DateTime.MaxValue;
    private double m_LastSpeedValue = double.NaN;
    private DateTime m_LastSpeedTime = DateTime.MaxValue;

    public override void SessionValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "Amp")
      {
        m_AmpereGauge.AddValue(p_Value);
        //can we use these 2 values to run the power gauge?
        if (!double.IsNaN(m_LastVoltageValue))
          m_PowerGauge.AddValue(m_LastVoltageValue * -p_Value);
      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "BatV")
      {
        m_VoltGauge.AddValue(p_Value);
        m_LastVoltageValue = p_Value;
        m_LastVoltageTime = DateTime.Now;
      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "Speed")
      {
        m_SpeedMeter.SetSpeed(p_Value);
        m_SpeedSoCGauge.AddValueSpeed(p_Value);
        m_LastSpeedValue = p_Value;
        m_LastSpeedTime = DateTime.Now;
        // need the math to calculate the consumption based on speed over time is distance
        // m_LastVoltageValue * -p_Value Amps, gives us power together with m_LastVoltageTime energy.
        // matching up the time from above we can calculate current consumption values...
        m_ConsumptionGauge.AddValue(18.3);//garbage test value

      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "SoCDiplay")
      {
        m_SpeedSoCGauge.AddValueSoC(p_Value);
      }
    }
  }
}