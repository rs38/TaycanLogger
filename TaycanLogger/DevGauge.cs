namespace TaycanLogger
{
  public class DevGauge : BaseGauge
  {
    private DrawGauge m_DrawGauge;
    public double ValueMin { get => m_DrawGauge.ValueMin; set => m_DrawGauge.ValueMin = value; }
    public double ValueMax { get => m_DrawGauge.ValueMax; set => m_DrawGauge.ValueMax = value; }

    //this does not need to be a property...
    public StartPinFlow FlowDirection { get => m_DrawGauge.FlowDirection; set => m_DrawGauge.FlowDirection = value; }

    public DevGauge()
    {
      m_DrawGauge = new DrawGauge();
      DoubleBuffered = true;
      BackColor = SystemColors.Control;
      m_DrawGauge.ForeColor =ColorPower;
      ValueMin = 0;
      ValueMax = 100;
      FlowDirection = StartPinFlow.StartLeftPinTop;
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      base.OnSizeChanged(e);
      m_DrawGauge.Location = new PointF(2, 2);
      m_DrawGauge.Size = new SizeF(ClientSize.Width - m_DrawGauge.Location.X*2, ClientSize.Height - m_DrawGauge.Location.Y*2);
      Invalidate();
    }

    public void Reset()
    {
      m_DrawGauge.Reset();
      Invalidate();
    }

    public void AddValue(double p_Value)
    {
      m_DrawGauge.AddValue(p_Value);
      Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      m_DrawGauge.Paint(e.Graphics);
    }
  }
}
