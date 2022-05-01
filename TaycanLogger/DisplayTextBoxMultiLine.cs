namespace TaycanLogger
{
  internal class DisplayTextBoxMultiLine : DisplayContentCanvas
  {
    public string Text { get; private set; }

    internal DisplayTextBoxMultiLine(Rectangle p_CanvasBounds, Action<Rectangle> p_Invalidate) : base(p_CanvasBounds, p_Invalidate)
    {
      Text = string.Empty;
      m_TextChanged = true;
    }

    internal void AddLine(string p_Line)
    {
      if (string.IsNullOrEmpty(Text))
        Text = p_Line;
      else
        Text += Environment.NewLine + p_Line;
      m_TextChanged = true;
      m_ShowLastLine = true;
    }

    internal void SetText(string p_Text)
    {
      Text = p_Text;
      m_TextChanged = true;
    }

    internal void Clear()
    {
      Text = string.Empty;
      m_TextChanged = true;
    }

    private bool m_TextChanged;
    private bool m_ShowLastLine;

    internal void Paint(Graphics p_Graphics, Color p_Color)
    {
      p_Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      p_Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      if (m_TextChanged)
      {
        ContentSize = TextRenderer.MeasureText(p_Graphics, Text, FormControlGlobals.FontDisplayText, Size.Empty, FormControlGlobals.DefaultTextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Top);
        ContentSize = new Size((int)(ContentSize.Width + FormControlGlobals.TextMarginWidth * 2), (int)(ContentSize.Height + FormControlGlobals.TextMarginHeight * 2));
        m_TextChanged = false;
      }
      p_Graphics.SetClip(CanvasBounds);
      if (m_ShowLastLine)
        MoveContent(new Size(0, int.MinValue / 2));
      m_ShowLastLine = false;
      TextRenderer.DrawText(p_Graphics, Text, FormControlGlobals.FontDisplayText, new Point((int)(ContentLocation.X + FormControlGlobals.TextMarginWidth), (int)(ContentLocation.Y + FormControlGlobals.TextMarginHeight)), p_Color, Color.Transparent, FormControlGlobals.DefaultTextFormatFlags | TextFormatFlags.Left | TextFormatFlags.Top);
      p_Graphics.ResetClip();
    }
  }
}