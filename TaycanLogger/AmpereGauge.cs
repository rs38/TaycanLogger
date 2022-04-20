namespace TaycanLogger
{
  internal class AmpereGauge : BaseGauge
  {
    private DrawPosNegGauge m_DrawGauge;
    public double ValueMin { get => m_DrawGauge.ValueMin; set => m_DrawGauge.ValueMin = value; }
    public double ValueMax { get => m_DrawGauge.ValueMax; set => m_DrawGauge.ValueMax = value; }

    internal AmpereGauge()
    {
      m_DrawGauge = new DrawPosNegGauge();
      m_DrawGauge.ForeColorPos = ColorPower;
      m_DrawGauge.ForeColorNeg = ColorRecup;
      m_DrawGauge.ValueMin = -50;
      m_DrawGauge.ValueMax = 50;
      m_DrawGauge.Flow = FlowDirection.RightToLeft;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      //m_DrawGauge.Location = new PointF(0, 0);
      m_DrawGauge.Size = new SizeF(ClientSize.Width, ClientSize.Height - 80);
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
      //we flip the value, since it is negative from the BMS, not in terms on consumption by the car.
      m_ValueCurrent = p_Value * -1;
       m_ValueMin = Math.Min(m_ValueMin, m_ValueCurrent);
      m_ValueMax = Math.Max(m_ValueMax, m_ValueCurrent);
      m_DrawGauge.AddValue(m_ValueCurrent);
      m_DrawGauge.ValueMin = m_ValueMin - 10f;
      m_DrawGauge.ValueMax = m_ValueMax + 10f;
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_DrawGauge.Paint(e.Graphics);
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