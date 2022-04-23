namespace TaycanLogger
{
  internal class PlotterSpeedSoC : PlotterBase
  {
    private PlotterDrawPosNeg m_PlotterDraw;
    public double ValueMin { get => m_PlotterDraw.ValueMin; set => m_PlotterDraw.ValueMin = value; }
    public double ValueMax { get => m_PlotterDraw.ValueMax; set => m_PlotterDraw.ValueMax = value; }

    public PlotterSpeedSoC()
    {
      m_PlotterDraw = new PlotterDrawPosNeg();
      m_PlotterDraw.ForeColorPos = FormControlGlobals.ColorPower;
      m_PlotterDraw.ForeColorNeg = FormControlGlobals.ColorRecup;
      m_PlotterDraw.ValueMin = -100;
      m_PlotterDraw.ValueMax = 50;
      m_PlotterDraw.Flow = FlowDirection.RightToLeft;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      m_PlotterDraw.Size = ClientSize;
      base.OnSizeChanged(e);
    }

    public void Reset()
    {
      m_PlotterDraw.Reset();
      Invalidate();
    }

    private double m_ValueMin = double.MinValue;
    private double m_ValueMax = double.MinValue;
    private double m_ValueCurrentSpeed = double.NaN;
    private double m_ValueCurrentSoC = double.NaN;

    public void AddValueSpeed(double p_Value)
    {
      m_ValueCurrentSpeed = p_Value;
      m_ValueMax = Math.Max(m_ValueMax, m_ValueCurrentSpeed);
      m_PlotterDraw.AddValuePos(p_Value);
      if (!double.IsNaN(m_ValueCurrentSoC))
        m_PlotterDraw.AddValueNeg(m_ValueCurrentSoC);
      m_PlotterDraw.ValueMax = m_ValueMax + 10f;
      Invalidate();
    }

    public void AddValueSoC(double p_Value)
    {
      m_ValueCurrentSoC = p_Value;
      m_ValueMin = Math.Max(m_ValueMin, m_ValueCurrentSoC);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_PlotterDraw.Paint(e.Graphics);
      PaintText(e.Graphics, "Speed km/h", FormControlGlobals.FontDisplayTitle, TextFormatFlags.HorizontalCenter, false);
      if (m_ValueMax > double.MinValue)
        PaintText(e.Graphics, Math.Round(m_ValueMax).ToString(), FormControlGlobals.FontDisplayText, TextFormatFlags.Left, false);
      PaintText(e.Graphics, "% SoC", FormControlGlobals.FontDisplayTitle, TextFormatFlags.HorizontalCenter, true);
      if (m_ValueMin > double.MinValue)
        PaintText(e.Graphics, Math.Round(m_ValueMin, 1).ToString(), FormControlGlobals.FontDisplayText, TextFormatFlags.Left, true);
      if (!double.IsNaN(m_ValueCurrentSoC))
        PaintText(e.Graphics, Math.Round(m_ValueCurrentSoC, 1).ToString(), FormControlGlobals.FontDisplayText, TextFormatFlags.Right, true);
    }
  }
}
