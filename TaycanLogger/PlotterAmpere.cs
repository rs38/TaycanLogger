namespace TaycanLogger
{
  internal class PlotterAmpere : PlotterBase
  {
    private PlotterDrawPosNeg m_PlotterDraw;
    public double ValueMin { get => m_PlotterDraw.ValueMin; set => m_PlotterDraw.ValueMin = value; }
    public double ValueMax { get => m_PlotterDraw.ValueMax; set => m_PlotterDraw.ValueMax = value; }

    internal PlotterAmpere()
    {
      m_PlotterDraw = new PlotterDrawPosNeg();
      m_PlotterDraw.ForeColorPos = ColorPower;
      m_PlotterDraw.ForeColorNeg = ColorRecup;
      m_PlotterDraw.ValueMin = -50;
      m_PlotterDraw.ValueMax = 50;
      m_PlotterDraw.Flow = FlowDirection.RightToLeft;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      m_PlotterDraw.Size = new SizeF(ClientSize.Width, ClientSize.Height - 80);
      Invalidate();
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
      //we flip the value, since it is negative from the BMS, not in terms on consumption by the car.
      m_ValueCurrent = p_Value * -1;
       m_ValueMin = Math.Min(m_ValueMin, m_ValueCurrent);
      m_ValueMax = Math.Max(m_ValueMax, m_ValueCurrent);
      m_PlotterDraw.AddValue(m_ValueCurrent);
      m_PlotterDraw.ValueMin = m_ValueMin - 10f;
      m_PlotterDraw.ValueMax = m_ValueMax + 10f;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_PlotterDraw.Paint(e.Graphics);
      float v_TextHeight = e.Graphics.MeasureString("0", Font).Height;
      StringFormat v_StringFormat = new StringFormat();
      v_StringFormat.Alignment = StringAlignment.Center;
      v_StringFormat.LineAlignment = StringAlignment.Center;
      var v_Brush = new SolidBrush(ForeColor);
      var v_Rect = new RectangleF(TextMargin, ClientSize.Height - v_TextHeight - TextMargin, ClientSize.Width - TextMargin*2, v_TextHeight);
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.DrawString("Ampere", Font, v_Brush, v_Rect, v_StringFormat);
      v_Rect.Offset(0, -v_TextHeight - TextMargin);
      v_StringFormat.Alignment = StringAlignment.Near;
      if (m_ValueMin < double.MaxValue)
        e.Graphics.DrawString(Math.Round(m_ValueMin, 1).ToString(), Font, v_Brush, v_Rect, v_StringFormat);
      v_StringFormat.Alignment = StringAlignment.Far;
      if (m_ValueMax > double.MinValue)
        e.Graphics.DrawString(Math.Round(m_ValueMax, 1).ToString(), Font, v_Brush, v_Rect, v_StringFormat);
      v_StringFormat.Alignment = StringAlignment.Center;
      if (!double.IsNaN(m_ValueCurrent))
        e.Graphics.DrawString($"{Math.Round(m_ValueCurrent, 1)} A", Font, v_Brush, v_Rect, v_StringFormat);
    }

  }
}