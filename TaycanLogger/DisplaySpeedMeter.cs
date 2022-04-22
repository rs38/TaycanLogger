namespace TaycanLogger
{
  public class DisplaySpeedMeter : Control
  {
    private StringFormat m_StringFormat;
    private Brush? m_Brush;
    private Font m_Font;

    public DisplaySpeedMeter()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      BackColor = SystemColors.Control;
      m_StringFormat = new StringFormat();
      m_StringFormat.Alignment = StringAlignment.Center;
      m_StringFormat.LineAlignment = StringAlignment.Center;
      m_Font = FormControlGlobals.FontDisplayText;
      Text = "0";
    }

    public void SetSpeed(double p_Value)
    {
      Text = Math.Round(p_Value).ToString();
      Invalidate();
    }

    private bool m_Resized;

    protected override void OnSizeChanged(EventArgs e)
    {
      m_Resized = true;
      base.OnSizeChanged(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      if (m_Brush is null)
        m_Brush = new SolidBrush(ForeColor);
      if (m_Resized)
        for (int size = (int)(ClientSize.Height * 72 / e.Graphics.DpiY); size >= 8; --size)
        {
          m_Font = new Font(m_Font.FontFamily, size, m_Font.Style);
          if (TextRenderer.MeasureText("000", m_Font).Width <= ClientSize.Width) break;
        }
      m_Resized = false;
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      RectangleF v_RectangleF = ClientRectangle;
      //this TT font requires some offset! Yikes...
      v_RectangleF.Offset(0, v_RectangleF.Height * 0.1f);
      e.Graphics.DrawString(Text, m_Font, m_Brush, v_RectangleF, m_StringFormat);
    }
  }

}