namespace TaycanLogger
{
  internal class DisplayDataBase : Control
  {

    public DisplayDataBase()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      BackColor = SystemColors.Control;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
    }
  }

  internal class DataListDrawItem
  {
    public string Title { get; set; }
    public string Text { get; set; }

    public DataListDrawItem(string p_Title, string? p_Text)
    {
      Title = p_Title;
      Text = p_Text is null ? String.Empty : p_Text;
    }

    public void Paint(Graphics p_Graphics, Font p_FontText, Font p_FontTitle, Color p_Color, RectangleF p_LayoutRectangle, TextFormatFlags p_TextFormatFlags)
    {
      TextRenderer.DrawText(p_Graphics, Text, p_FontText, p_LayoutRectangle.ToRectangle(), p_Color, Color.Transparent, p_TextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Top);
      TextRenderer.DrawText(p_Graphics, Title, p_FontTitle, p_LayoutRectangle.ToRectangle(), p_Color, Color.Transparent, p_TextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Bottom);
    }
  }

  internal class DataListDraw
  {
    private Dictionary<string, DataListDrawItem> m_Items;

    public float Width { get; set; }
    public int ColumnCount { get; set; }
    public int ItemsPerColumn { get; set; }
    public float ItemVerticalSpace { get; set; }

    internal DataListDraw(int p_ColumnCount, int p_ItemsPerColumn)
    {
      ColumnCount = p_ColumnCount;
      ItemsPerColumn = p_ItemsPerColumn;
      ItemVerticalSpace = 7f;
      m_Items = new Dictionary<string, DataListDrawItem>();
    }

    internal void AddItem(string p_Name, string p_Title, string? p_Text)
    {
      m_Items.Add(p_Name, new DataListDrawItem(p_Title,  p_Text));
    }

    internal void SetItemText(string p_Name, string p_Text)
    {
      if (m_Items.ContainsKey(p_Name))
        m_Items[p_Name].Text = p_Text;
    }

    private float m_ItemHeight;

    internal void Paint(Graphics p_Graphics, Color p_Color)
    {
      m_ItemHeight = TextRenderer.MeasureText("0", FormControlGlobals.FontDisplayTitle).Height * 2.1f;
      RectangleF v_LayoutRectangle = new RectangleF(FormControlGlobals.TextMarginWidth, ItemVerticalSpace, Width / ColumnCount - FormControlGlobals.TextMarginWidth * 2f, (int)m_ItemHeight);
      int v_ItemsPerColumn = ItemsPerColumn;
      if (v_ItemsPerColumn <= 0)
        v_ItemsPerColumn = m_Items.Count;
      int l_Column = 1;
      int v_Index = 1;
      foreach (DataListDrawItem l_DataListItem in m_Items.Values)
      {
        l_DataListItem.Paint(p_Graphics, FormControlGlobals.FontDisplayText, FormControlGlobals.FontDisplayTitle, p_Color, v_LayoutRectangle, FormControlGlobals.DefaultTextFormatFlags);
        v_LayoutRectangle.Offset(0, v_LayoutRectangle.Height + ItemVerticalSpace);
        if (v_Index++ % v_ItemsPerColumn != 0)
        {
          p_Graphics.DrawLine(Pens.White, v_LayoutRectangle.X, v_LayoutRectangle.Y, v_LayoutRectangle.Right, v_LayoutRectangle.Y);
          v_LayoutRectangle.Offset(0, ItemVerticalSpace);
        }
        else
          v_LayoutRectangle = new RectangleF(FormControlGlobals.TextMarginWidth + (Width / ColumnCount) * l_Column++, ItemVerticalSpace, Width / ColumnCount - FormControlGlobals.TextMarginWidth * 2f, (int)m_ItemHeight);
      }
    }
  }
}