namespace TaycanLogger
{
  public class PlotterVoltage : PlotterBase
  {
    private PlotterDraw m_PlotterDraw;
    public double ValueMin { get => m_PlotterDraw.ValueMin; set => m_PlotterDraw.ValueMin = value; }
    public double ValueMax { get => m_PlotterDraw.ValueMax; set => m_PlotterDraw.ValueMax = value; }

    public PlotterVoltage()
    {
      m_PlotterDraw = new PlotterDraw();
      m_PlotterDraw.ForeColor = FormControlGlobals.ColorPower;
      m_PlotterDraw.ValueMin = 600;
      m_PlotterDraw.ValueMax = 850;
      m_PlotterDraw.FlowDirection = StartPinFlow.StartLeftPinBottom;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      m_PlotterDraw.Size = new SizeF(ClientSize.Width, ClientSize.Height - 80);
      base.OnSizeChanged(e);
    }

    public void Reset()
    {
      m_PlotterDraw.Reset();
      Invalidate();
    }

    private double m_ValueMin = double.MaxValue;
    private double m_ValueMax = double.MinValue;
    private double m_ValueCurrent = double.NaN;

    public void AddValue(double p_Value)
    {
      m_ValueCurrent = p_Value;
      m_ValueMin = Math.Min(m_ValueMin, m_ValueCurrent);
      m_ValueMax = Math.Max(m_ValueMax, m_ValueCurrent);
      m_PlotterDraw.AddValue(p_Value);
      m_PlotterDraw.ValueMin = m_ValueMin - 10f;
      m_PlotterDraw.ValueMax = m_ValueMax + 10f;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_PlotterDraw.Paint(e.Graphics);
      PaintText(e.Graphics, "Voltage", FormControlGlobals.FontDisplayTitle, TextFormatFlags.HorizontalCenter, true);
      if (m_ValueMin < double.MaxValue)
        PaintText(e.Graphics, Math.Round(m_ValueMin).ToString(), FormControlGlobals.FontDisplayText, TextFormatFlags.Right, true, true);
      if (m_ValueMax > double.MinValue)
        PaintText(e.Graphics, Math.Round(m_ValueMax).ToString(), FormControlGlobals.FontDisplayText, TextFormatFlags.Left, true, true);
      if (!double.IsNaN(m_ValueCurrent))
        PaintText(e.Graphics, Math.Round(m_ValueCurrent).ToString(), FormControlGlobals.FontDisplayText, TextFormatFlags.HorizontalCenter, true, true);
    }
  }
}
