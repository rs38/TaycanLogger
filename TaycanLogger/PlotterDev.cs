namespace TaycanLogger
{
  public class PlotterDev : PlotterBase
  {
    private PlotterDraw m_PlotterDraw;
    public double ValueMin { get => m_PlotterDraw.ValueMin; set => m_PlotterDraw.ValueMin = value; }
    public double ValueMax { get => m_PlotterDraw.ValueMax; set => m_PlotterDraw.ValueMax = value; }

    //this does not need to be a property...
    public StartPinFlow FlowDirection { get => m_PlotterDraw.FlowDirection; set => m_PlotterDraw.FlowDirection = value; }

    public PlotterDev()
    {
      m_PlotterDraw = new PlotterDraw();
      DoubleBuffered = true;
      BackColor = SystemColors.Control;
      m_PlotterDraw.ForeColor =ColorPower;
      ValueMin = 0;
      ValueMax = 100;
      FlowDirection = StartPinFlow.StartLeftPinTop;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      m_PlotterDraw.Location = new PointF(2, 2);
      m_PlotterDraw.Size = new SizeF(ClientSize.Width - m_PlotterDraw.Location.X*2, ClientSize.Height - m_PlotterDraw.Location.Y*2);
      Invalidate();
    }

    public void Reset()
    {
      m_PlotterDraw.Reset();
      Invalidate();
    }

    public void AddValue(double p_Value)
    {
      m_PlotterDraw.AddValue(p_Value);
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_PlotterDraw.Paint(e.Graphics);
    }
  }
}
