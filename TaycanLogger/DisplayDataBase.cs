namespace TaycanLogger
{
  internal class DisplayDataBase : Control
  {
    protected StringFormat m_StringFormat;
    protected Brush? m_Brush;

    public DisplayDataBase()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      BackColor = SystemColors.Control;
      m_StringFormat = new StringFormat();
      m_StringFormat.Alignment = StringAlignment.Near;
      m_StringFormat.LineAlignment = StringAlignment.Near;
    }

    protected bool m_Resized;

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
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
    }
  }

  internal class DataListItem
  {
    public string Title { get; set; }
    public string Text { get; set; }

    public DataListItem(string p_Title, string p_Text)
    {
      Title = p_Title;
      Text = p_Text;
    }

    public void Paint(Graphics p_Graphics, Font p_FontText, Font p_FontTitle, Brush p_Brush, RectangleF p_LayoutRectangle, StringFormat p_StringFormat)
    {
      p_StringFormat.LineAlignment = StringAlignment.Near;
      p_Graphics.DrawString(Text, p_FontText, p_Brush, p_LayoutRectangle, p_StringFormat);
      p_StringFormat.LineAlignment = StringAlignment.Far;
      p_Graphics.DrawString(Title, p_FontTitle, p_Brush, p_LayoutRectangle, p_StringFormat);
    }
  }

  internal class DisplayDataList : DisplayDataBase
  {
    private Font m_FontTitle;
    private Dictionary<string, DataListItem> m_Dictionary;

    public DisplayDataList()
    {
      m_Dictionary = new Dictionary<string, DataListItem>();
    }

    public void AddItem(string p_Name, string p_Title, string p_Text)
    {
      m_Dictionary.Add(p_Name, new DataListItem(p_Title, p_Text));
    }

    public void SetItemText(string p_Name, string p_Text)
    {
      m_Dictionary[p_Name].Text = p_Text;
      Invalidate();
    }

    private float m_ItemHeight;
    private const float spacer = 8;//make static value to cahnge...

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      if (m_Brush is null)
        m_Brush = new SolidBrush(ForeColor);
      if (m_Resized)
      {
        m_ItemHeight = TextRenderer.MeasureText("0", Font).Height * 2f;
      }
      m_Resized = false;
      //  for (int size = (int)(ClientSize.Height * 72 / e.Graphics.DpiY); size >= 8; --size)
      //  {
      //    Font = new Font(Font.FontFamily, size, Font.Style);
      //    if (TextRenderer.MeasureText("000", Font).Width <= ClientSize.Width) break;
      //  }
      //m_Resized = false;
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

      RectangleF v_LayoutRectangle = new RectangleF(spacer, spacer, ClientSize.Width - spacer * 2, m_ItemHeight);
      m_FontTitle = new Font(Font.Name, Font.Size * 0.8f, Font.Style);
      int v_Index = m_Dictionary.Count - 1;
      foreach (DataListItem l_DataListItem in m_Dictionary.Values)
      {
        l_DataListItem.Paint(e.Graphics, Font, m_FontTitle, m_Brush, v_LayoutRectangle, m_StringFormat);
        v_LayoutRectangle.Offset(0, v_LayoutRectangle.Height + spacer);
        if (v_Index-- > 0)
          e.Graphics.DrawLine(Pens.Gray, v_LayoutRectangle.X, v_LayoutRectangle.Y, v_LayoutRectangle.Width, v_LayoutRectangle.Y);
        v_LayoutRectangle.Offset(0, spacer);
      }
    }
  }

}