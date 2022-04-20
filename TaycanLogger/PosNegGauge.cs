namespace TaycanLogger
{
  //public class PosNegGauge : BaseGauge
  //{
  //  private DrawGauge m_DrawGaugePos;
  //  private DrawGauge m_DrawGaugeNeg;
  //  public double ValueMin { get => m_DrawGaugeNeg.ValueMax * -1; set => m_DrawGaugeNeg.ValueMax = value * -1; }
  //  public double ValueMax { get => m_DrawGaugePos.ValueMax; set => m_DrawGaugePos.ValueMax = value; }

  //  public FlowDirection Flow
  //  {
  //    get => m_DrawGaugePos.FlowDirection switch
  //    {
  //      StartPinFlow.StartLeftPinTop => FlowDirection.LeftToRight,
  //      StartPinFlow.StartLeftPinBottom => FlowDirection.LeftToRight,
  //      StartPinFlow.StartRightPinTop => FlowDirection.RightToLeft,
  //      StartPinFlow.StartRightPinBottom => FlowDirection.RightToLeft,
  //      StartPinFlow.StartTopPinLeft => FlowDirection.TopDown,
  //      StartPinFlow.StartTopPinRight => FlowDirection.TopDown,
  //      StartPinFlow.StartBottomPinLeft => FlowDirection.BottomUp,
  //      StartPinFlow.StartBottomPinRight => FlowDirection.BottomUp
  //    };
  //    set => SetFlowDirection(value);
  //  }

  //  protected void SetFlowDirection(FlowDirection p_FlowDirection)
  //  {
  //    switch (p_FlowDirection)
  //    {
  //      case FlowDirection.LeftToRight:
  //        m_DrawGaugePos.FlowDirection = StartPinFlow.StartLeftPinBottom;
  //        m_DrawGaugeNeg.FlowDirection = StartPinFlow.StartLeftPinTop;
  //        break;
  //      case FlowDirection.RightToLeft:
  //        m_DrawGaugePos.FlowDirection = StartPinFlow.StartRightPinBottom;
  //        m_DrawGaugeNeg.FlowDirection = StartPinFlow.StartRightPinTop;
  //        break;
  //      case FlowDirection.TopDown:
  //        m_DrawGaugePos.FlowDirection = StartPinFlow.StartTopPinLeft;
  //        m_DrawGaugeNeg.FlowDirection = StartPinFlow.StartTopPinRight;
  //        break;
  //      case FlowDirection.BottomUp:
  //        m_DrawGaugePos.FlowDirection = StartPinFlow.StartBottomPinLeft;
  //        m_DrawGaugeNeg.FlowDirection = StartPinFlow.StartBottomPinRight;
  //        break;
  //    }
  //    switch (p_FlowDirection)
  //    {
  //      case FlowDirection.LeftToRight:
  //      case FlowDirection.RightToLeft:
  //        m_DrawGaugePos.Location = new PointF(0, 0);
  //        m_DrawGaugePos.Size = new Size(ClientSize.Width, ClientSize.Height / 2 - 1);
  //        m_DrawGaugeNeg.Location = new PointF(0, ClientSize.Height / 2);
  //        m_DrawGaugeNeg.Size = new Size(ClientSize.Width, ClientSize.Height / 2 - 1);
  //        break;
  //      case FlowDirection.TopDown:
  //      case FlowDirection.BottomUp:
  //        m_DrawGaugePos.Location = new PointF(ClientSize.Width / 2, 0);
  //        m_DrawGaugePos.Size = new Size(ClientSize.Width / 2, ClientSize.Height - 1);
  //        m_DrawGaugeNeg.Location = new PointF(0, 0);
  //        m_DrawGaugeNeg.Size = new Size(ClientSize.Width / 2 - 1, ClientSize.Height - 1);
  //        break;
  //    }
  //  }

  //  protected PosNegGauge()
  //  {
  //    DoubleBuffered = true;
  //    BackColor = SystemColors.Control;
  //    SetFlowDirection(FlowDirection.RightToLeft);
  //  }

  //  internal override void OnSizeChanged(EventArgs e)
  //  {
  //    base.OnSizeChanged(e);
  //    SetFlowDirection(Flow);
  //    Invalidate();
  //  }

  //  internal virtual void Reset()
  //  {
  //    m_DrawGaugePos.Reset();
  //    m_DrawGaugeNeg.Reset();
  //    Invalidate();
  //  }

  //  protected virtual void AddValue(double p_Value)
  //  {
  //    m_DrawGaugePos.AddValue(Math.Max(0, p_Value));
  //    m_DrawGaugeNeg.AddValue(Math.Min(0, p_Value) * -1);
  //    float v_ValueMin = float.MinValue;
  //    float v_ValueMax = float.MinValue;
  //    foreach (var l_Value in m_DrawGaugePos.Values)
  //      v_ValueMax = Math.Max(v_ValueMax, l_Value);
  //    foreach (var l_Value in m_DrawGaugeNeg.Values)
  //      v_ValueMin = Math.Max(v_ValueMin, l_Value);
  //    m_DrawGaugePos.ValueMax = Math.Max(100, Math.Max(v_ValueMin, v_ValueMax)) ;
  //    m_DrawGaugeNeg.ValueMax = m_DrawGaugePos.ValueMax;
  //    Invalidate();
  //  }

  //  protected override void OnPaint(PaintEventArgs e)
  //  {
  //    base.OnPaint(e);
  //    m_DrawGaugePos.Paint(e.Graphics);
  //    m_DrawGaugeNeg.Paint(e.Graphics);
  //  }
  //}
}
