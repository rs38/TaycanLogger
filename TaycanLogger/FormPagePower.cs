namespace TaycanLogger
{
  public class FormPagePower : FormPage
  {
    public override Type Type { get => this.GetType(); }

    private DisplaySpeedMeter m_DisplaySpeedMeter;
    private PlotterPower m_PlotterPower;
    private PlotterAmpere m_PlotterAmpere;
    private PlotterVoltage m_PlotterVolt;
    private PlotterConsumption m_PlotterConsumption;
    private PlotterSpeedSoC m_PlotterSpeedSoC;
    private DisplayDataList m_DataListDisplayLeft;
    private DisplayDataList m_DataListDisplayRight;

    public FormPagePower(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      m_DataListDisplayLeft = new DisplayDataList();
      m_DataListDisplayRight = new DisplayDataList();
      m_DisplaySpeedMeter = new DisplaySpeedMeter();
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
      SetupControl(m_DisplaySpeedMeter);
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
      SetColumnSpan(m_DisplaySpeedMeter, 2);
      Controls.Add(m_DisplaySpeedMeter, 2, 0);
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
      //m_DataListDisplayLeft.AddItem("i3", "Average consumption", "18.1 kWh/100 km");
      //m_DataListDisplayLeft.AddItem("i4", "Average speed", "10 km/h");
      m_DataListDisplayRight.AddItem("i1", "Drive time", "10:34 h");
      m_DataListDisplayRight.AddItem("i2", "Distance", "932.2 km");
      //m_DataListDisplayRight.AddItem("i3", "Average consumption", "18.1 kWh/100 km");
      //m_DataListDisplayRight.AddItem("i4", "Average speed", "10 km/h");

    }

    private void SetupControl(Control p_Control)
    {
      p_Control.Font = Parent.Font;
      p_Control.ForeColor = Color.White;
      p_Control.BackColor = Color.Black;
      p_Control.Dock = System.Windows.Forms.DockStyle.Fill;
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

    private FormPagePowerCalc? m_FormPagePowerCalc;

    public override void SessionValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      if (m_FormPagePowerCalc is null)
      {
        m_FormPagePowerCalc = new FormPagePowerCalc(
          p_Value => m_DisplaySpeedMeter.SetSpeed(p_Value),
          p_Value => m_PlotterAmpere.AddValue(p_Value),
          p_Value => m_PlotterConsumption.AddValue(p_Value),
          p_Value => m_PlotterPower.AddValue(p_Value),
          p_Value => m_PlotterVolt.AddValue(p_Value),
          p_Value => m_PlotterSpeedSoC.AddValueSpeed(p_Value),
          p_Value => m_PlotterSpeedSoC.AddValueSoC(p_Value));
      }
      m_FormPagePowerCalc.SessionValueExecuted(p_Name, p_Units, p_Value);
    }
  }
}