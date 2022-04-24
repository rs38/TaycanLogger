namespace TaycanLogger
{
  public class DisplaySpeedMeter : Control
  {
    private Font m_Font;

    public DisplaySpeedMeter()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      BackColor = SystemColors.Control;
      m_Font = Font;
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
      if (m_Resized)
        for (int l_Size = (int)(ClientSize.Height * 72 / e.Graphics.DpiY); l_Size >= 8; --l_Size)
        {
          m_Font = new Font(FormControlGlobals.FontFamily, l_Size, FontStyle.Regular);
          if (TextRenderer.MeasureText("000", m_Font).Width <= ClientSize.Width) break;
        }
      m_Resized = false;
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      RectangleF v_Rectangle = ClientRectangle;
      //this TT font requires some offset! Yikes...
      v_Rectangle.Offset(0, v_Rectangle.Height * 0.05f);
      TextRenderer.DrawText(e.Graphics, Text, m_Font, v_Rectangle.ToRectangle(), ForeColor, Color.Transparent, FormControlGlobals.DefaultTextFormatFlags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
    }
  }

}