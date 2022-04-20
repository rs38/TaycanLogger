namespace TaycanLogger
{
  internal class SpeedSoCGauge : BaseGauge
  {
    private DrawPosNegGauge m_DrawGauge;
    public double ValueMin { get => m_DrawGauge.ValueMin; set => m_DrawGauge.ValueMin = value; }
    public double ValueMax { get => m_DrawGauge.ValueMax; set => m_DrawGauge.ValueMax = value; }

    public SpeedSoCGauge()
    {
      m_DrawGauge = new DrawPosNegGauge();
      m_DrawGauge.ForeColorPos = ColorPower;
      m_DrawGauge.ForeColorNeg = ColorRecup;
      m_DrawGauge.ValueMin = -100;
      m_DrawGauge.ValueMax = 50;
      m_DrawGauge.Flow = FlowDirection.RightToLeft;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      m_DrawGauge.Size = ClientSize;
      Invalidate();
    }

    public void Reset()
    {
      m_DrawGauge.Reset();
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
      m_DrawGauge.AddValuePos(p_Value);
      if (!double.IsNaN(m_ValueCurrentSoC))
        m_DrawGauge.AddValueNeg(m_ValueCurrentSoC);
      m_DrawGauge.ValueMax = m_ValueMax + 10f;
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
      m_DrawGauge.Paint(e.Graphics);
      PaintText(e.Graphics, "Speed", StringAlignment.Center, false);
      if (m_ValueMax > double.MinValue)
        PaintText(e.Graphics, $"{Math.Round(m_ValueMax)} km/h", StringAlignment.Near, false);
      PaintText(e.Graphics, "SoC", StringAlignment.Center, true);
      if (m_ValueMin > double.MinValue)
        PaintText(e.Graphics, $"{Math.Round(m_ValueMin, 1)} %", StringAlignment.Near, true);
      if (!double.IsNaN(m_ValueCurrentSoC))
        PaintText(e.Graphics, $"{Math.Round(m_ValueCurrentSoC, 1)} %", StringAlignment.Far, true);
    }
  }
}
