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
      m_DataListDisplayLeft = new DisplayDataList(3, 2);
      m_DataListDisplayRight = new DisplayDataList(3, 2);
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
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 156F));
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

      m_DataListDisplayLeft.AddItem("LeftCol1Row1", "Title Speed");// units = "km/h" />
      m_DataListDisplayLeft.AddItem("LeftCol1Row2", "Title SoCDiplay");// units = "%" />
      m_DataListDisplayLeft.AddItem("LeftCol2Row1", "Title AirT");// units = "C" />
      m_DataListDisplayLeft.AddItem("LeftCol2Row2", "Title InsideT");// units = "C" />
      m_DataListDisplayLeft.AddItem("LeftCol3Row1", "Title CompW");// units = "W" />
      m_DataListDisplayLeft.AddItem("LeftCol3Row2", "Title HeatA");// units = "A" />

      //need to set title and place it properly...
      m_DataListDisplayRight.AddItem("RightCol1Row1", "Title BatTOut"); //units = "C" />
      m_DataListDisplayRight.AddItem("RightCol1Row2", "Title BatTIn");   //units = "C" />
      m_DataListDisplayRight.AddItem("RightCol2Row1", "Title BatTemp"); //units = "C" />
      m_DataListDisplayRight.AddItem("RightCol2Row2", "Title CelSum");   //units = "V" />
      m_DataListDisplayRight.AddItem("RightCol3Row1", "Title BatLimC"); //units = "A" />
      m_DataListDisplayRight.AddItem("ErrorCount", "Errors/Total");

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
      m_DataListDisplayRight.SetItemText("ErrorCount", $"{m_CommandErrorCount}/{m_CommandExecutedCount}");
    }

    private FormPagePowerCalc? m_FormPagePowerCalc;

    public override void SessionValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      if (m_FormPagePowerCalc is null)
      {
        m_FormPagePowerCalc = new FormPagePowerCalc(
          p_Value => m_DisplaySpeedMeter.SetSpeed(p_Value),
          (p_Name, p_Text) => m_DataListDisplayLeft.SetItemText(p_Name, p_Text),
          (p_Name, p_Text) => m_DataListDisplayRight.SetItemText(p_Name, p_Text),
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