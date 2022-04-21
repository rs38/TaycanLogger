namespace TaycanLogger
{
  internal class PlotterConsumption : PlotterBase
  {
    private PlotterDrawPosNeg m_PlotterDraw;
    public double ValueMin { get => m_PlotterDraw.ValueMin; set => m_PlotterDraw.ValueMin = value; }
    public double ValueMax { get => m_PlotterDraw.ValueMax; set => m_PlotterDraw.ValueMax = value; }

    public PlotterConsumption()
    {
      DoubleBuffered = true;
      BackColor = SystemColors.Control;
      m_PlotterDraw = new PlotterDrawPosNeg();
      m_PlotterDraw.ForeColorPos = ColorPower;
      m_PlotterDraw.ForeColorNeg = ColorRecup;
      m_PlotterDraw.ValueMin = -10;
      m_PlotterDraw.ValueMax = 10;
      m_PlotterDraw.Flow = FlowDirection.LeftToRight;
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
      PaintText(e.Graphics, "Consumption", StringAlignment.Center, true);
      if (m_ValueMin < double.MaxValue)
        PaintText(e.Graphics, Math.Round(m_ValueMin).ToString(), StringAlignment.Far, true);
      if (m_ValueMax > double.MinValue)
        PaintText(e.Graphics, Math.Round(m_ValueMax).ToString(), StringAlignment.Far, false);
      if (!double.IsNaN(m_ValueCurrent))
        PaintText(e.Graphics, $"{Math.Round(m_ValueCurrent)} kWh/100km", StringAlignment.Near, true);
    }
  }
}
