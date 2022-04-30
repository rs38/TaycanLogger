namespace TaycanLogger
{
  internal class DisplayContentCanvas
  {
    internal Point m_MouseMove = Point.Empty;
    internal Point m_ContentLocation;
    private bool m_MouseMoved;

    public Rectangle CanvasBounds { get; set; }

    public Point ContentLocation
    {
      get
      {
        return new Point(m_ContentLocation.X + CanvasBounds.X, m_ContentLocation.Y + CanvasBounds.Y);
      }
      set
      {
        m_ContentLocation.X = value.X - CanvasBounds.X;
        m_ContentLocation.Y = value.Y - CanvasBounds.Y;
      }
    }

    public Size ContentSize { get; set; }

    public DisplayContentCanvas(Rectangle p_CanvasBounds)
    {
      CanvasBounds = p_CanvasBounds;
      m_ContentLocation = new Point(0, 0);
      ContentSize = Size.Empty;
    }

    protected bool MoveContent(Size p_Size)
    {
      Point v_Location = m_ContentLocation;
      v_Location.X += p_Size.Width;
      if (p_Size.Width > 0)
      {
        if (v_Location.X > 0)
          v_Location.X = 0;
      }
      else
      {
        if (ContentSize.Width < CanvasBounds.Width - v_Location.X)
          v_Location.X = CanvasBounds.Width - ContentSize.Width;
      }
      if (CanvasBounds.Width > ContentSize.Width)
        v_Location.X = 0;

      v_Location.Y += p_Size.Height;
      if (p_Size.Height > 0)
      {
        if (v_Location.Y > 0)
          v_Location.Y = 0;
      }
      else
      {
        if (ContentSize.Height < CanvasBounds.Height - v_Location.Y)
          v_Location.Y = CanvasBounds.Height - ContentSize.Height;
      }
      if (CanvasBounds.Height > ContentSize.Height)
        v_Location.Y = 0;
      if (v_Location != m_ContentLocation)
      {
        m_ContentLocation = v_Location;
        return true;
      }
      return false;
    }

    public virtual void MouseDown(Point p_Point)
    {
      if (CanvasBounds.Contains(p_Point))
        m_MouseMove = p_Point;
      m_MouseMoved = false;
    }

    public virtual bool MouseUp(Point p_Point)
    {
      m_MouseMove = Point.Empty;
      return m_MouseMoved;
    }

    public virtual void MouseMove(Point p_Point, Action<Rectangle> p_Invalidate)
    {
      if (!m_MouseMove.IsEmpty)
      {
        m_MouseMoved = m_MouseMove != p_Point;
        bool v_Redraw = MoveContent(new Size(p_Point.X - m_MouseMove.X, p_Point.Y - m_MouseMove.Y));
        m_MouseMove = p_Point;
        if (v_Redraw)
          p_Invalidate(CanvasBounds);
      }
    }

    public virtual void MouseWheel(Point p_Point, int p_Delta, bool p_Horizontal, Action<Rectangle> p_Invalidate)
    {
      if (CanvasBounds.Contains(p_Point))
      {
        bool v_Redraw = false;
        if (p_Horizontal)
          v_Redraw = MoveContent(new Size(p_Delta, 0));
        else
          v_Redraw = MoveContent(new Size(0, p_Delta));
        if (v_Redraw)
          p_Invalidate(CanvasBounds);
      }
    }

    public virtual void SizeChanged(Rectangle p_CanvasBounds, Action<Rectangle> p_Invalidate)
    {
      CanvasBounds = p_CanvasBounds;
      if (MoveContent(new Size(0, 0)))
        p_Invalidate(CanvasBounds);
    }
  }


}