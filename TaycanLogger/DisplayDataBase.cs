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

    public DataListDrawItem(string p_Title, string p_Text)
    {
      Title = p_Title;
      Text = p_Text;
    }

    public void Paint(Graphics p_Graphics, Font p_FontText, Font p_FontTitle, Color p_Color, Rectangle p_LayoutRectangle, TextFormatFlags p_TextFormatFlags)
    {
      TextRenderer.DrawText(p_Graphics, Text, p_FontText, p_LayoutRectangle, p_Color, Color.Transparent, p_TextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Top);
      TextRenderer.DrawText(p_Graphics, Title, p_FontTitle, p_LayoutRectangle, p_Color, Color.Transparent, p_TextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Bottom);
    }
  }

  internal class DataListDraw
  {
    private Dictionary<string, DataListDrawItem> m_Items;

    internal float Width { get; set; }

    internal DataListDraw()
    {
      m_Items = new Dictionary<string, DataListDrawItem>();
      Width = 0;
    }

    internal void AddItem(string p_Name, string p_Title, string p_Text)
    {
      m_Items.Add(p_Name, new DataListDrawItem(p_Title, p_Text));
    }

    internal void SetItemText(string p_Name, string p_Text)
    {
      m_Items[p_Name].Text = p_Text;
    }

    private float m_ItemHeight;
    private const int spacer = 8;//make static value to cahnge...

    internal void Paint(Graphics p_Graphics, Color p_Color)
    {
      m_ItemHeight = TextRenderer.MeasureText("0", FormControlGlobals.FontDisplayTitle).Height * 2.1f;
      Rectangle v_LayoutRectangle = new Rectangle(spacer, spacer, (int)(Width - spacer * 2f), (int)m_ItemHeight);
      int v_Index = m_Items.Count - 1;
      foreach (DataListDrawItem l_DataListItem in m_Items.Values)
      {
        l_DataListItem.Paint(p_Graphics, FormControlGlobals.FontDisplayText, FormControlGlobals.FontDisplayTitle, p_Color, v_LayoutRectangle, FormControlGlobals.DefaultTextFormatFlags);
        v_LayoutRectangle.Offset(0, v_LayoutRectangle.Height + spacer);
        if (v_Index-- > 0)
          p_Graphics.DrawLine(Pens.White, v_LayoutRectangle.X, v_LayoutRectangle.Y, v_LayoutRectangle.Right, v_LayoutRectangle.Y);
        v_LayoutRectangle.Offset(0, spacer);
      }
    }
  }
}