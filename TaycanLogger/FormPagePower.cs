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
    private DataListDisplay m_DataListDisplayLeft;
    private DataListDisplay m_DataListDisplayRight;

    public FormPagePower(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      m_DataListDisplayLeft = new DataListDisplay();
      m_DataListDisplayRight = new DataListDisplay();
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
      SetupControl(m_DataListDisplayLeft);
      SetupControl(m_DataListDisplayRight);
      SetupControl(m_SpeedMeter);
      SetupControl(m_PlotterAmpere);
      SetupControl(m_PlotterPower);
      SetupControl(m_PlotterVolt);
      SetupControl(m_PlotterConsumption);
      SetupControl(m_PlotterSpeedSoC);
      ColumnCount = 6;
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
      RowCount = 3;
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 15F));
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
      SetColumnSpan(m_DataListDisplayLeft, 2);
      Controls.Add(m_DataListDisplayLeft, 0, 0);
      SetColumnSpan(m_DataListDisplayRight, 2);
      Controls.Add(m_DataListDisplayRight, 4, 0);
      SetColumnSpan(m_SpeedMeter, 2);
      Controls.Add(m_SpeedMeter, 2, 0);
      Controls.Add(m_PlotterAmpere, 0, 1);
      SetColumnSpan(m_PlotterPower, 4);
      Controls.Add(m_PlotterPower, 1, 1);
      Controls.Add(m_PlotterVolt, 5, 1);
      SetColumnSpan(m_PlotterSpeedSoC, 3);
      Controls.Add(m_PlotterSpeedSoC, 0, 2);
      SetColumnSpan(m_PlotterConsumption, 3);
      Controls.Add(m_PlotterConsumption, 3, 2);





      //test data, delete...
      m_DataListDisplayLeft.AddItem("i1", "Drive time", "10:34 h");
      m_DataListDisplayLeft.AddItem("i2", "Distance", "932.2 km");
      m_DataListDisplayLeft.AddItem("i3", "Average consumption", "18.1 kWh/100 km");
      m_DataListDisplayLeft.AddItem("i4", "Average speed", "10 km/h");
      m_DataListDisplayRight.AddItem("i1", "Drive time", "10:34 h");
      m_DataListDisplayRight.AddItem("i2", "Distance", "932.2 km");
      m_DataListDisplayRight.AddItem("i3", "Average consumption", "18.1 kWh/100 km");
      m_DataListDisplayRight.AddItem("i4", "Average speed", "10 km/h");

    }

    private void SetupControl(Control m_SpeedMeter)
    {
      m_SpeedMeter.Font = Parent.Font;
      m_SpeedMeter.ForeColor = Color.White;
      m_SpeedMeter.BackColor = Color.Black;
      m_SpeedMeter.Dock = System.Windows.Forms.DockStyle.Fill;
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