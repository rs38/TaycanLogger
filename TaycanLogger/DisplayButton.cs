namespace TaycanLogger
{
  internal class DisplayButton
  {
    public string Text { get; set; }
    public Rectangle CanvasBounds { get; set; }

    public DisplayButton(string p_Text)
    {
      Text = p_Text;
    }

    public virtual void SizeChanged(Rectangle p_CanvasBounds, Action<Rectangle> p_Invalidate)
    {
      CanvasBounds = p_CanvasBounds;
      p_Invalidate(CanvasBounds);
    }

    public virtual void MouseDown(Point p_Point, Action<Rectangle> p_Invalidate)
    {
      //if (CanvasBounds.Contains(p_Point))
      //{
      //  p_Invalidate(CanvasBounds);
      //}
    }

    internal event Action Pressed;

    public virtual void MouseUp(Point p_Point, Action<Rectangle> p_Invalidate)
    {
      if (CanvasBounds.Contains(p_Point))
      {
        Pressed();
        //p_Invalidate(CanvasBounds);
      }
    }

    //System.Diagnostics.Debug.WriteLine($"{ p_Point} { CanvasBounds.Contains(p_Point)}" );  


    internal virtual void MouseMove(Point p_Point, Action<Rectangle> p_Invalidate)
    {
      if (!m_Hot && CanvasBounds.Contains(p_Point))
      {
        m_Hot = true;
        p_Invalidate(CanvasBounds);
      }
      if (m_Hot && !CanvasBounds.Contains(p_Point))
      {
        m_Hot = false;
        p_Invalidate(CanvasBounds);
      }
    }

    internal virtual void MouseLeave(Action<Rectangle> p_Invalidate)
    {
      if (m_Hot)
      {
        m_Hot = false;
        p_Invalidate(CanvasBounds);
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