namespace TaycanLogger
{
  internal class ConsumptionGauge : BaseGauge
  {
    private DrawPosNegGauge m_DrawGauge;
    public double ValueMin { get => m_DrawGauge.ValueMin; set => m_DrawGauge.ValueMin = value; }
    public double ValueMax { get => m_DrawGauge.ValueMax; set => m_DrawGauge.ValueMax = value; }

    public ConsumptionGauge()
    {
      DoubleBuffered = true;
      BackColor = SystemColors.Control;
      m_DrawGauge = new DrawPosNegGauge();
      m_DrawGauge.ForeColorPos = ColorPower;
      m_DrawGauge.ForeColorNeg = ColorRecup;
      m_DrawGauge.ValueMin = -10;
      m_DrawGauge.ValueMax = 10;
      m_DrawGauge.Flow = FlowDirection.LeftToRight;
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

    private double m_ValueMin = double.MaxValue;
    private double m_ValueMax = double.MinValue;
    private double m_ValueCurrent = double.NaN;

    public void AddValue(double p_Value)
    {
      m_ValueCurrent = p_Value;
      m_ValueMin = Math.Min(m_ValueMin, m_ValueCurrent);
      m_ValueMax = Math.Max(m_ValueMax, m_ValueCurrent);
      m_DrawGauge.AddValue(p_Value);
      m_DrawGauge.ValueMin = m_ValueMin - 10f;
      m_DrawGauge.ValueMax = m_ValueMax + 10f;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_DrawGauge.Paint(e.Graphics);
      PaintText(e.Graphics, "Consumption", StringAlignment.Center, false);
      if (m_ValueMax > double.MinValue)
        PaintText(e.Graphics, Math.Round(m_ValueMax).ToString(), StringAlignment.Far, false);
      if (m_ValueMin < double.MaxValue)
        PaintText(e.Graphics, Math.Round(m_ValueMin).ToString(), StringAlignment.Far, true);
      if (!double.IsNaN(m_ValueCurrent))
        PaintText(e.Graphics, $"{Math.Round(m_ValueCurrent)} kWh/100km", StringAlignment.Near, true);
    }
  }
}
