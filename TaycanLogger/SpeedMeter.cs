namespace TaycanLogger
{
  public class SpeedMeter : Control
  {
    private StringFormat m_StringFormat;
    private Brush? m_Brush;

    public SpeedMeter()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      BackColor = SystemColors.Control;
      m_StringFormat = new StringFormat();
      m_StringFormat.Alignment = StringAlignment.Center;
      m_StringFormat.LineAlignment = StringAlignment.Center;
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
          Font = new Font(Font.FontFamily, size, Font.Style);
          if (TextRenderer.MeasureText("000", Font).Width <= ClientSize.Width) break;
        }
      m_Resized = false;
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      e.Graphics.DrawString(Text, Font, m_Brush, ClientRectangle, m_StringFormat);
    }
  }

}