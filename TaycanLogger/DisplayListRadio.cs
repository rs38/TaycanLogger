namespace TaycanLogger
{
  internal class DisplayListRadio<T> : DisplayContentCanvas where T : IComparable
  {

    internal class DrawListRadioItem
    {
      public string Title { get; set; }
      public string Text { get; set; }
      public bool Selected { get; set; }

      public DrawListRadioItem(string p_Title, string? p_Text, bool p_Selected = false)
      {
        Title = p_Title;
        Text = p_Text is null ? String.Empty : p_Text;
        Selected = p_Selected;
      }

      public void Paint(Graphics p_Graphics, Font p_FontText, Font p_FontTitle, Color p_Color, Pen p_Pen, SolidBrush p_SolidBrush, RectangleF p_LayoutRectangle, TextFormatFlags p_TextFormatFlags)
      {
        Rectangle v_RectangleText = new Rectangle((int)(p_LayoutRectangle.X + p_LayoutRectangle.Height / 5f), (int)p_LayoutRectangle.Y, (int)p_LayoutRectangle.Width, (int)p_LayoutRectangle.Height);
        TextRenderer.DrawText(p_Graphics, Text, p_FontText, v_RectangleText, p_Color, Color.Transparent, p_TextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Top);
        TextRenderer.DrawText(p_Graphics, Title, p_FontTitle, v_RectangleText, p_Color, Color.Transparent, p_TextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Bottom);
        RectangleF v_Rectangle = new RectangleF(p_LayoutRectangle.X + p_LayoutRectangle.Width - p_LayoutRectangle.Height, p_LayoutRectangle.Y, p_LayoutRectangle.Height, p_LayoutRectangle.Height);
        v_Rectangle = RectangleF.Inflate(v_Rectangle, -p_LayoutRectangle.Height / 5f, -p_LayoutRectangle.Height / 5f);
        p_Graphics.DrawEllipse(p_Pen, v_Rectangle);
        if (Selected)
          p_Graphics.FillEllipse(p_SolidBrush, RectangleF.Inflate(v_Rectangle, -p_LayoutRectangle.Height / 12f, -p_LayoutRectangle.Height / 12f));
      }
    }

    private Dictionary<T, DrawListRadioItem> m_Items;

    public float ItemVerticalSpace { get; set; }

    internal DisplayListRadio(Rectangle p_CanvasBounds) : base(p_CanvasBounds)
    {
      ItemVerticalSpace = 7f;
      m_Items = new Dictionary<T, DrawListRadioItem>();
    }

    internal void AddItem(T p_Name, string p_Title, string? p_Text, bool p_Selected = false)
    {
      m_Items.Add(p_Name, new DrawListRadioItem(p_Title, p_Text, p_Selected));
    }

    internal void SelectItem(T p_Name)
    {
      if (p_Name is not null)
        foreach (var l_Item in m_Items)
          l_Item.Value.Selected = l_Item.Key.Equals(p_Name);
    }

    internal void Clear()
    {
      m_Items.Clear();
    }

    internal T SelectedItem()
    {
      foreach (var l_Item in m_Items)
        if (l_Item.Value.Selected)
          return l_Item.Key;
      return default!;
    }

    internal string? GetItemText(T v_Key)
    {
      foreach (var l_Item in m_Items)
        if (l_Item.Key.Equals(v_Key))
          return l_Item.Value.Text;
      return null;
    }

    public override void SizeChanged(Rectangle p_CanvasBounds, Action<Rectangle> p_Invalidate)
    {
      if (ContentSize.Width != p_CanvasBounds.Width)
        ContentSize = new Size(p_CanvasBounds.Width, ContentSize.Height);
      base.SizeChanged(p_CanvasBounds, p_Invalidate);
    }

    public event Action<T>? ItemSelected;

    internal void MouseUp(Point p_Point, Action<Rectangle> p_Invalidate)
    {
      if (!base.MouseUp(p_Point) && CanvasBounds.Contains(p_Point))
      {
        int v_Index = (int)((p_Point.Y - ContentLocation.Y) * m_Items.Count / ContentSize.Height);
        if (0 <= v_Index && v_Index < m_Items.Count)
        {
          T? v_SelectedName = default;
          int l_Index = 0;
          foreach (var l_Item in m_Items)
          {
            if (l_Index == v_Index && !l_Item.Value.Selected)
              v_SelectedName = l_Item.Key;
            l_Item.Value.Selected = l_Index++ == v_Index;
          }
          p_Invalidate(CanvasBounds);
          //selected a new item, so we fire event...
          if (v_SelectedName is not null)
            ItemSelected?.Invoke(v_SelectedName);
        }
      }
    }

    private float m_ItemHeight = 0;

    internal void Paint(Graphics p_Graphics, Color p_Color, Color p_ColorSelected)
    {
      p_Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      p_Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      p_Graphics.SetClip(CanvasBounds);
      if (m_ItemHeight == 0)
        m_ItemHeight = (int)(TextRenderer.MeasureText("0", FormControlGlobals.FontDisplayTitle).Height * 2.2f);
      RectangleF v_LayoutRectangle = new RectangleF(new PointF(ContentLocation.X, ContentLocation.Y + ItemVerticalSpace), new SizeF(ContentSize.Width, m_ItemHeight));
      int v_Index = 1;
      using (Pen v_PenLine = new Pen(p_Color, 1f))
      {
        using (Pen v_Pen = new Pen(p_Color, 2f))
        using (SolidBrush v_SolidBrush = new SolidBrush(p_ColorSelected))
          foreach (DrawListRadioItem l_DataListItem in m_Items.Values)
          {
            l_DataListItem.Paint(p_Graphics, FormControlGlobals.FontDisplayText, FormControlGlobals.FontDisplayTitle, p_Color, v_Pen, v_SolidBrush, v_LayoutRectangle, FormControlGlobals.DefaultTextFormatFlags | TextFormatFlags.Left);
            v_LayoutRectangle.Offset(0, m_ItemHeight + ItemVerticalSpace);
            if (v_Index++ % m_Items.Count != 0)
            {
              p_Graphics.DrawLine(v_PenLine, v_LayoutRectangle.X, v_LayoutRectangle.Y, v_LayoutRectangle.Right, v_LayoutRectangle.Y);
              v_LayoutRectangle.Offset(0, ItemVerticalSpace);
            }
          }
        int v_ContentHeight = (int)(v_LayoutRectangle.Y - ContentLocation.Y);
        if (ContentSize.Height != v_ContentHeight)
          ContentSize = new Size(CanvasBounds.Width, v_ContentHeight);

        p_Graphics.ResetClip();
        //no outline frame...
        //p_Graphics.DrawRectangle(v_PenLine, Rectangle.Inflate(CanvasBounds, 1, 1));
      }

    }
  }
}