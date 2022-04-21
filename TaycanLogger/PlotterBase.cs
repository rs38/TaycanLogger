namespace TaycanLogger
{
  public enum StartPinFlow { StartLeftPinTop, StartLeftPinBottom, StartRightPinTop, StartRightPinBottom, StartTopPinLeft, StartTopPinRight, StartBottomPinLeft, StartBottomPinRight }

  public class PlotterBase : Control
  {
    protected StringFormat m_StringFormat;
    protected float m_TextHeight;
    protected Brush? m_Brush;

    public static Color ColorPower = Color.FromArgb(0, 176, 244);
    public static Color ColorRecup = Color.FromArgb(16, 185, 0);
    public static float TextMargin = 2f;

    public PlotterBase()
    {
      DoubleBuffered = true;
      BackColor = SystemColors.Control;
      m_StringFormat = new StringFormat();
      m_StringFormat.Alignment = StringAlignment.Center;
      m_StringFormat.LineAlignment = StringAlignment.Center;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      base.OnPaint(e);
      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      m_TextHeight = e.Graphics.MeasureString("0", Font).Height;
    }


    protected virtual void PaintText(Graphics p_Graphics, string p_Text, StringAlignment p_StringAlignment, bool p_Bottom, bool p_Second = false)
    {
      if (m_Brush is null)
        m_Brush = new SolidBrush(ForeColor);
      m_StringFormat.Alignment = p_StringAlignment;
      RectangleF v_LayoutRectangle;
      if (p_Bottom)
        v_LayoutRectangle = new RectangleF(TextMargin, ClientSize.Height - m_TextHeight - TextMargin * 2, ClientSize.Width - TextMargin * 2, m_TextHeight);
      else
        v_LayoutRectangle = new RectangleF(TextMargin, TextMargin, ClientSize.Width - TextMargin * 2, m_TextHeight);
      if (p_Second)
        v_LayoutRectangle.Offset(0, (m_TextHeight + TextMargin) * (p_Bottom ? -1 : 1));
      p_Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
      p_Graphics.DrawString(p_Text, Font, m_Brush, v_LayoutRectangle, m_StringFormat);
    }

    internal class Buffer<T> : LinkedList<T>
    {
      private int m_Capacity;

      public Buffer(int p_Capacity)
      {
        this.m_Capacity = p_Capacity;
      }

      public Buffer(int p_Capacity, IEnumerable<T> collection) : base(collection)
      {
        this.m_Capacity = p_Capacity;
      }

      public void Push(T item)
      {
        while (Count > m_Capacity && Count > 0) RemoveLast();
        AddFirst(item);
      }
    }

    public static Color ChangeColorBrightness(Color color, float correctionFactor)
    {
      float red = (float)color.R;
      float green = (float)color.G;
      float blue = (float)color.B;

      if (correctionFactor < 0)
      {
        correctionFactor = 1 + correctionFactor;
        red *= correctionFactor;
        green *= correctionFactor;
        blue *= correctionFactor;
      }
      else
      {
        red = (255 - red) * correctionFactor + red;
        green = (255 - green) * correctionFactor + green;
        blue = (255 - blue) * correctionFactor + blue;
      }
      return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
    }

    internal class PlotterDraw
    {
      private Buffer<float>? m_Values;
      private PointF m_Location;
      private SizeF m_Size;
      private Pen[] m_Pens;

      internal Buffer<float>? Values { get => m_Values; }
      internal double ValueMin { get; set; }
      internal double ValueMax { get; set; }
      internal PointF Location { get => m_Location; set => SetLocationSize(value, null); }
      internal SizeF Size { get => m_Size; set => SetLocationSize(null, value); }
      internal StartPinFlow FlowDirection { get; set; }
      internal Color ForeColor { get => m_Pens[0].Color; set => SetForeColor(value); }

      internal PlotterDraw()
      {
        ValueMin = 0;
        ValueMax = 100;
        m_Location = new Point(0, 0);
        m_Size = new Size(100, 100);
        m_Pens = new Pen[] { SystemPens.ControlDark };
        FlowDirection = StartPinFlow.StartLeftPinTop;
      }

      private void SetLocationSize(PointF? p_Location, SizeF? p_Size)
      {
        bool v_Changed = false;
        if (p_Location is not null && m_Location != p_Location)
        {
          m_Location = (PointF)p_Location;
          v_Changed = true;
        }
        if (p_Size is not null && m_Size != p_Size)
        {
          m_Size = (SizeF)p_Size;
          v_Changed = true;
        }
        if (v_Changed)
        {
          float v_Capacity = FlowDirection switch
          {
            StartPinFlow.StartLeftPinTop => m_Size.Width - m_Location.X,
            StartPinFlow.StartLeftPinBottom => m_Size.Width - m_Location.X,
            StartPinFlow.StartRightPinTop => m_Size.Width - m_Location.X,
            StartPinFlow.StartRightPinBottom => m_Size.Width - m_Location.X,
            StartPinFlow.StartTopPinLeft => m_Size.Height - m_Location.Y,
            StartPinFlow.StartTopPinRight => m_Size.Height - m_Location.Y,
            StartPinFlow.StartBottomPinLeft => m_Size.Height - m_Location.Y,
            StartPinFlow.StartBottomPinRight => m_Size.Height - m_Location.Y
          };
          if (m_Values is null)
            m_Values = new Buffer<float>((int)v_Capacity + 1);
          else
            m_Values = new Buffer<float>((int)v_Capacity + 1, m_Values);
        }
      }

      protected void SetForeColor(Color p_Color)
      {
        m_Pens = new Pen[12];
        for (int i = 0; i < m_Pens.Length; i++)
          m_Pens[i] = new Pen(ChangeColorBrightness(p_Color, i / -50f), 1f);
      }

      internal void Reset()
      {
        m_Values?.Clear();
      }

      internal void AddValue(double p_Value)
      {
        m_Values?.Push((float)Math.Round(p_Value, 1));
      }

      internal void Paint(Graphics p_Graphics)
      {
        if (m_Values != null)
        {
          float v_Disp = FlowDirection switch
          {
            StartPinFlow.StartLeftPinTop => m_Size.Height,
            StartPinFlow.StartLeftPinBottom => m_Size.Height,
            StartPinFlow.StartRightPinTop => m_Size.Height,
            StartPinFlow.StartRightPinBottom => m_Size.Height,
            StartPinFlow.StartTopPinLeft => m_Size.Width,
            StartPinFlow.StartTopPinRight => m_Size.Width,
            StartPinFlow.StartBottomPinLeft => m_Size.Width,
            StartPinFlow.StartBottomPinRight => m_Size.Width
          };
          v_Disp /= (float)(ValueMax - ValueMin);
          float v_Flow = 0f;
          foreach (var l_Value in m_Values)
          {
            float v_Value = (float)((l_Value - ValueMin) * v_Disp);
            if (v_Value > 0f)
            {
              (float X1, float Y1, float X2, float Y2) v_DrawLine = FlowDirection switch
              {
                StartPinFlow.StartLeftPinTop => (v_Flow, 0f, v_Flow, v_Value),
                StartPinFlow.StartLeftPinBottom => (v_Flow, m_Size.Height, v_Flow, m_Size.Height - v_Value),
                StartPinFlow.StartRightPinTop => (m_Size.Width - v_Flow, 0f, m_Size.Width - v_Flow, v_Value),
                StartPinFlow.StartRightPinBottom => (m_Size.Width - v_Flow, m_Size.Height, m_Size.Width - v_Flow, m_Size.Height - v_Value),
                StartPinFlow.StartTopPinLeft => (0f, v_Flow, v_Value, v_Flow),
                StartPinFlow.StartTopPinRight => (m_Size.Width, v_Flow, m_Size.Width - v_Value, v_Flow),
                StartPinFlow.StartBottomPinLeft => (0f, m_Size.Height - v_Flow, v_Value, m_Size.Height - v_Flow),
                StartPinFlow.StartBottomPinRight => (m_Size.Width, m_Size.Height - v_Flow, m_Size.Width - v_Value, m_Size.Height - v_Flow)
              };
              p_Graphics.DrawLine(m_Pens[(int)Math.Min(m_Pens.Length - 1, v_Flow / 2)], m_Location.X + v_DrawLine.X1, m_Location.Y + v_DrawLine.Y1, m_Location.X + v_DrawLine.X2, m_Location.Y + v_DrawLine.Y2);
            }
            v_Flow++;
          }
        }
      }
    }

    internal class PlotterDrawPosNeg
    {
      private PlotterDraw m_PlotterDrawPos;
      private PlotterDraw m_PlotterDrawNeg;
      private PointF m_Location;
      private SizeF m_Size;

      internal PointF Location { get => m_Location; set => SetLocationSize(value, null); }

      internal SizeF Size { get => m_Size; set => SetLocationSize(null, value); }

      internal double ValueMin { get => m_PlotterDrawNeg.ValueMax * -1; set => m_PlotterDrawNeg.ValueMax = value * -1; }
      internal double ValueMax { get => m_PlotterDrawPos.ValueMax; set => m_PlotterDrawPos.ValueMax = value; }
      internal Color ForeColorPos { get => m_PlotterDrawPos.ForeColor; set => m_PlotterDrawPos.ForeColor = value; }
      internal Color ForeColorNeg { get => m_PlotterDrawNeg.ForeColor; set => m_PlotterDrawNeg.ForeColor = value; }

      internal FlowDirection Flow
      {
        get => m_PlotterDrawPos.FlowDirection switch
        {
          StartPinFlow.StartLeftPinTop => FlowDirection.LeftToRight,
          StartPinFlow.StartLeftPinBottom => FlowDirection.LeftToRight,
          StartPinFlow.StartRightPinTop => FlowDirection.RightToLeft,
          StartPinFlow.StartRightPinBottom => FlowDirection.RightToLeft,
          StartPinFlow.StartTopPinLeft => FlowDirection.TopDown,
          StartPinFlow.StartTopPinRight => FlowDirection.TopDown,
          StartPinFlow.StartBottomPinLeft => FlowDirection.BottomUp,
          StartPinFlow.StartBottomPinRight => FlowDirection.BottomUp
        };
        set => SetFlowDirection(value);
      }

      internal PlotterDrawPosNeg()
      {
        m_PlotterDrawPos = new PlotterDraw();
        m_PlotterDrawNeg = new PlotterDraw();
        m_PlotterDrawPos.ValueMin = 0;
        m_PlotterDrawPos.ValueMax = 50;
        m_PlotterDrawNeg.ValueMin = 0;
        m_PlotterDrawNeg.ValueMax = 50;
        m_PlotterDrawPos.FlowDirection = StartPinFlow.StartLeftPinBottom;
        m_PlotterDrawNeg.FlowDirection = StartPinFlow.StartLeftPinTop;
      }

      private void SetFlowDirection(FlowDirection p_FlowDirection)
      {
        switch (p_FlowDirection)
        {
          case FlowDirection.LeftToRight:
            m_PlotterDrawPos.FlowDirection = StartPinFlow.StartLeftPinBottom;
            m_PlotterDrawNeg.FlowDirection = StartPinFlow.StartLeftPinTop;
            break;
          case FlowDirection.RightToLeft:
            m_PlotterDrawPos.FlowDirection = StartPinFlow.StartRightPinBottom;
            m_PlotterDrawNeg.FlowDirection = StartPinFlow.StartRightPinTop;
            break;
          case FlowDirection.TopDown:
            m_PlotterDrawPos.FlowDirection = StartPinFlow.StartTopPinLeft;
            m_PlotterDrawNeg.FlowDirection = StartPinFlow.StartTopPinRight;
            break;
          case FlowDirection.BottomUp:
            m_PlotterDrawPos.FlowDirection = StartPinFlow.StartBottomPinLeft;
            m_PlotterDrawNeg.FlowDirection = StartPinFlow.StartBottomPinRight;
            break;
        }
      }

      private void SetLocationSize(PointF? p_Location, SizeF? p_Size)
      {
        bool v_Changed = false;
        if (p_Location is not null && m_Location != p_Location)
        {
          m_Location = (PointF)p_Location;
          v_Changed = true;
        }
        if (p_Size is not null && m_Size != p_Size)
        {
          m_Size = (SizeF)p_Size;
          v_Changed = true;
        }
        if (v_Changed)
        {
          switch (Flow)
          {
            case FlowDirection.LeftToRight:
            case FlowDirection.RightToLeft:
              m_PlotterDrawPos.Location = new PointF(0, 0);
              m_PlotterDrawPos.Size = new SizeF(m_Size.Width, m_Size.Height / 2 - 1);
              m_PlotterDrawNeg.Location = new PointF(0, m_Size.Height / 2);
              m_PlotterDrawNeg.Size = new SizeF(m_Size.Width, m_Size.Height / 2 - 1);
              break;
            case FlowDirection.TopDown:
            case FlowDirection.BottomUp:
              m_PlotterDrawPos.Location = new PointF(m_Size.Width / 2, 0);
              m_PlotterDrawPos.Size = new SizeF(m_Size.Width / 2, m_Size.Height - 1);
              m_PlotterDrawNeg.Location = new PointF(0, 0);
              m_PlotterDrawNeg.Size = new SizeF(m_Size.Width / 2 - 1, m_Size.Height - 1);
              break;
          }
        }
      }

      internal void Reset()
      {
        m_PlotterDrawPos.Reset();
        m_PlotterDrawPos.Reset();
      }

      internal void AddValue(double p_Value)
      {
        m_PlotterDrawPos.AddValue(Math.Max(0, p_Value));
        m_PlotterDrawNeg.AddValue(Math.Min(0, p_Value) * -1);
      }

      internal void AddValuePos(double p_Value)
      {
        m_PlotterDrawPos.AddValue(Math.Max(0, p_Value));
      }

      internal void AddValueNeg(double p_Value)
      {
        m_PlotterDrawNeg.AddValue(Math.Max(0, p_Value));
      }

      internal void Paint(Graphics p_Graphics)
      {
        m_PlotterDrawPos.Paint(p_Graphics);
        m_PlotterDrawNeg.Paint(p_Graphics);
      }
    }
  }
}