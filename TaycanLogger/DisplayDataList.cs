namespace TaycanLogger
{
  internal class DisplayDataList : DisplayDataBase
  {
    protected DataListDraw m_DataListDraw;

    public DisplayDataList(int p_ColumnCount, int p_ItemsPerColumn)
    {
      m_DataListDraw = new DataListDraw(p_ColumnCount, p_ItemsPerColumn);
      m_DataListDraw.Width = ClientSize.Width;
    }

    public void AddItem(string p_Name, string p_Title, string? p_Text=null)
    {
      m_DataListDraw.AddItem(p_Name, p_Title, p_Text);
      Invalidate();
    }

    public void SetItemText(string p_Name, string p_Text)
    {
      m_DataListDraw.SetItemText(p_Name, p_Text);
      Invalidate();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      m_DataListDraw.Width = ClientSize.Width;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_DataListDraw.Paint(e.Graphics, ForeColor);
    }
  }
}
