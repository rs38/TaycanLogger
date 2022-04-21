namespace TaycanLogger
{
  internal class PlotterPower : PlotterBase
  {
    private PlotterDrawPosNeg m_PlotterDrawPosNeg;
    public double ValueMin { get => m_PlotterDrawPosNeg.ValueMin; set => m_PlotterDrawPosNeg.ValueMin = value; }
    public double ValueMax { get => m_PlotterDrawPosNeg.ValueMax; set => m_PlotterDrawPosNeg.ValueMax = value; }

    internal PlotterPower()
    {
      m_PlotterDrawPosNeg = new PlotterDrawPosNeg();
      m_PlotterDrawPosNeg.ForeColorPos = ColorPower;
      m_PlotterDrawPosNeg.ForeColorNeg = ColorRecup;
      m_PlotterDrawPosNeg.ValueMin = -50;
      m_PlotterDrawPosNeg.ValueMax = 50;
      m_PlotterDrawPosNeg.Flow = FlowDirection.TopDown;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      m_PlotterDrawPosNeg.Size = new SizeF(ClientSize.Width, ClientSize.Height - 80);
      base.OnSizeChanged(e);
    }

    public void Reset()
    {
      m_PlotterDrawPosNeg.Reset();
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
      m_PlotterDrawPosNeg.AddValue(p_Value);
      m_PlotterDrawPosNeg.ValueMin = m_ValueMin - 10f;
      m_PlotterDrawPosNeg.ValueMax = m_ValueMax + 10f;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_PlotterDrawPosNeg.Paint(e.Graphics);
      PaintText(e.Graphics, "Recup", StringAlignment.Near, true);
      PaintText(e.Graphics, "Power", StringAlignment.Far, true);
      if (m_ValueMin < double.MaxValue)
        PaintText(e.Graphics, Math.Round(m_ValueMin / 1000, 1).ToString(), StringAlignment.Near, true, true);
      if (m_ValueMax > double.MinValue)
        PaintText(e.Graphics, Math.Round(m_ValueMax / 1000, 1).ToString(), StringAlignment.Far, true, true);
      if (!double.IsNaN(m_ValueCurrent))
        PaintText(e.Graphics, $"{Math.Round(m_ValueCurrent / 1000, 1)} kW", StringAlignment.Center, true, true);
    }

  }
}