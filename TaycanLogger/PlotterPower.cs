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
      base.OnSizeChanged(e);
      m_PlotterDrawPosNeg.Size = new SizeF(ClientSize.Width, ClientSize.Height - 80);
      Invalidate();
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
      float v_TextHeight = e.Graphics.MeasureString("0", Font).Height;
      StringFormat v_StringFormat = new StringFormat();
      v_StringFormat.Alignment = StringAlignment.Center;
      v_StringFormat.LineAlignment = StringAlignment.Center;
      var v_Brush = new SolidBrush(ForeColor);
      var v_Rect = new RectangleF(4, ClientSize.Height - v_TextHeight - 4, ClientSize.Width - 8, v_TextHeight);
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      v_StringFormat.Alignment = StringAlignment.Near;
      e.Graphics.DrawString("Recup", Font, v_Brush, v_Rect, v_StringFormat);
      v_StringFormat.Alignment = StringAlignment.Far;
      e.Graphics.DrawString("Power", Font, v_Brush, v_Rect, v_StringFormat);
      v_Rect.Offset(0, -v_TextHeight - 4);
      v_StringFormat.Alignment = StringAlignment.Near;
      if (m_ValueMin < 1000)
        e.Graphics.DrawString(Math.Round(m_ValueMin / 1000, 1).ToString(), Font, v_Brush, v_Rect, v_StringFormat);
      v_StringFormat.Alignment = StringAlignment.Far;
      if (m_ValueMax > 0)
        e.Graphics.DrawString(Math.Round(m_ValueMax / 1000, 1).ToString(), Font, v_Brush, v_Rect, v_StringFormat);
      v_StringFormat.Alignment = StringAlignment.Center;
      if (!double.IsNaN(m_ValueCurrent))
        e.Graphics.DrawString($"{Math.Round(m_ValueCurrent / 1000, 1)} kW", Font, v_Brush, v_Rect, v_StringFormat);
    }

  }
}