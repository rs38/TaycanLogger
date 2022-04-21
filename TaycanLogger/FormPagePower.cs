namespace TaycanLogger
{
  public class FormPagePower : FormPage
  {
    public override Type Type { get => this.GetType(); }

    private SpeedMeter m_SpeedMeter;
    private PlotterPower m_PlotterPower;
    private PlotterAmpere m_PlotterAmpere;
    private PlotterVoltage m_PlotterVolt;
    private PlotterConsumption m_PlotterConsumption;
    private PlotterSpeedSoC m_PlotterSpeedSoC;


    public FormPagePower(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      m_SpeedMeter = new SpeedMeter();
      m_PlotterAmpere = new PlotterAmpere();
      m_PlotterPower = new PlotterPower();
      m_PlotterVolt = new PlotterVoltage();
      m_PlotterConsumption = new PlotterConsumption();
      m_PlotterSpeedSoC = new PlotterSpeedSoC();
    }

    public override void Load()
    {
      base.Load();
      m_SpeedMeter.Font = Parent.Font;
      m_PlotterAmpere.Font = Parent.Font;
      m_PlotterPower.Font = Parent.Font;
      m_PlotterVolt.Font = Parent.Font;
      m_PlotterConsumption.Font = Parent.Font;
      m_PlotterSpeedSoC.Font = Parent.Font;
      m_SpeedMeter.ForeColor = Color.White;
      m_PlotterAmpere.ForeColor = Color.White;
      m_PlotterPower.ForeColor = Color.White;
      m_PlotterVolt.ForeColor = Color.White;
      m_PlotterConsumption.ForeColor = Color.White;
      m_PlotterSpeedSoC.ForeColor = Color.White;
      m_SpeedMeter.BackColor = Color.Black;
      m_PlotterAmpere.BackColor = Color.Black;
      m_PlotterPower.BackColor = Color.Black;
      m_PlotterVolt.BackColor = Color.Black;
      m_PlotterConsumption.BackColor = Color.Black;
      m_PlotterSpeedSoC.BackColor = Color.Black;
      m_PlotterPower.Dock = System.Windows.Forms.DockStyle.Fill;
      m_SpeedMeter.Dock = System.Windows.Forms.DockStyle.Fill;
      m_PlotterAmpere.Dock = System.Windows.Forms.DockStyle.Fill;
      m_PlotterVolt.Dock = System.Windows.Forms.DockStyle.Fill;
      m_PlotterConsumption.Dock = System.Windows.Forms.DockStyle.Fill;
      m_PlotterSpeedSoC.Dock = System.Windows.Forms.DockStyle.Fill;
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
      Controls.Add(m_PlotterAmpere, 0, 1);
      SetColumnSpan(m_PlotterPower, 2);
      Controls.Add(m_PlotterPower, 1, 1);
      Controls.Add(m_PlotterVolt, 2, 1);
      SetColumnSpan(m_PlotterSpeedSoC, 2);
      Controls.Add(m_PlotterSpeedSoC, 0, 2);
      SetColumnSpan(m_PlotterConsumption, 2);
      Controls.Add(m_PlotterConsumption, 2, 2);
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
        m_PlotterAmpere.AddValue(p_Value);
        //can we use these 2 values to run the power plotter?
        if (!double.IsNaN(m_LastVoltageValue))
          m_PlotterPower.AddValue(m_LastVoltageValue * -p_Value);
      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "BatV")
      {
        m_PlotterVolt.AddValue(p_Value);
        m_LastVoltageValue = p_Value;
        m_LastVoltageTime = DateTime.Now;
      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "Speed")
      {
        m_SpeedMeter.SetSpeed(p_Value);
        m_PlotterSpeedSoC.AddValueSpeed(p_Value);
        m_LastSpeedValue = p_Value;
        m_LastSpeedTime = DateTime.Now;
        // need the math to calculate the consumption based on speed over time is distance
        // m_LastVoltageValue * -p_Value Amps, gives us power together with m_LastVoltageTime energy.
        // matching up the time from above we can calculate current consumption values...
        m_PlotterConsumption.AddValue(18.3);//garbage test value

      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "SoCDiplay")
      {
        m_PlotterSpeedSoC.AddValueSoC(p_Value);
      }
    }
  }
}