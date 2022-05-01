namespace TaycanLogger
{
  internal class DisplayButton
  {
    public string Text { get; set; }
    public Rectangle CanvasBounds { get; set; }

    protected Action<Rectangle> Invalidate;

    public DisplayButton(string p_Text, Action<Rectangle> p_Invalidate)
    {
      Text = p_Text;
      Invalidate = p_Invalidate;
    }

    public virtual void SizeChanged(Rectangle p_CanvasBounds)
    {
      CanvasBounds = p_CanvasBounds;
      Invalidate(CanvasBounds);
    }

    internal event Action Pressed;

    public virtual void MouseUp(Point p_Point)
    {
      if (CanvasBounds.Contains(p_Point))
      {
        Pressed();
      }
    }

    internal virtual void MouseMove(Point p_Point)
    {
      if (!m_Hot && CanvasBounds.Contains(p_Point))
      {
        m_Hot = true;
        Invalidate(CanvasBounds);
      }
      if (m_Hot && !CanvasBounds.Contains(p_Point))
      {
        m_Hot = false;
        Invalidate(CanvasBounds);
      }
    }

    internal virtual void MouseLeave()
    {
      if (m_Hot)
      {
        m_Hot = false;
        Invalidate(CanvasBounds);
      }
    }

    private bool m_Hot;

    internal void Paint(Graphics p_Graphics, Color p_Color)
    {
      p_Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      p_Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      p_Graphics.SetClip(CanvasBounds);
      if (m_Hot)
        using (SolidBrush v_SolidBrush = new SolidBrush(FormControlGlobals.ColorButtonHover))
          p_Graphics.FillRectangle(v_SolidBrush, CanvasBounds);
      TextRenderer.DrawText(p_Graphics, Text, FormControlGlobals.FontDisplayText, CanvasBounds, p_Color, Color.Transparent, FormControlGlobals.DefaultTextFormatFlags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
      p_Graphics.ResetClip();
      //no outline frame...
      //p_Graphics.DrawRectangle(Pens.DarkGray, Rectangle.Inflate(CanvasBounds, 1, 1));
    }
  }
}