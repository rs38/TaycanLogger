namespace TaycanLogger
{
  internal class FormPageSettingsControl : Control
  {
    private DisplayListRadio<ulong> m_DrawListRadio;
    private DisplayButton m_DisplayButtonRefresh;
    private DisplayButton m_DisplayButtonStart;
    private DisplayTextBoxMultiLine m_TextBoxMultiLine;

    public FormPageSettingsControl()
    {
      DoubleBuffered = true;
      ResizeRedraw = true;
      m_DrawListRadio = new DisplayListRadio<ulong>(Rectangle.Empty, p_Rectangle => Invalidate(p_Rectangle));
      m_DisplayButtonRefresh = new DisplayButton("Refresh", p_Rectangle => Invalidate(p_Rectangle));
      m_DisplayButtonRefresh.Pressed += M_DisplayButtonRefresh_Pressed;
      m_DisplayButtonStart = new DisplayButton("Start", p_Rectangle => Invalidate(p_Rectangle));
      m_DisplayButtonStart.Pressed += M_DisplayButtonStart_Pressed;
      //m_DrawListRadio.ItemSelected += M_DrawListRadio_ItemSelected;
      m_TextBoxMultiLine = new DisplayTextBoxMultiLine(Rectangle.Empty, p_Rectangle => Invalidate(p_Rectangle));
    }

    internal event Action RefreshPressed;

    private void M_DisplayButtonRefresh_Pressed()
    {
      RefreshPressed();
    }

    internal event Action StartPressed;

    private void M_DisplayButtonStart_Pressed()
    {
      StartPressed();
    }

    protected override void OnSizeChanged(EventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        Rectangle v_Rectangle = new Rectangle(4, 4, 300, ClientSize.Height - 8 - 70 - 8 - 70 - 4);
        m_DrawListRadio.SizeChanged(v_Rectangle);
        v_Rectangle = new Rectangle(v_Rectangle.X, v_Rectangle.Height + 8, v_Rectangle.Width, 70);
        m_DisplayButtonRefresh.SizeChanged(v_Rectangle);
        v_Rectangle.Offset(0, v_Rectangle.Height + 4);
        m_DisplayButtonStart.SizeChanged(v_Rectangle);
        m_TextBoxMultiLine.SizeChanged(new Rectangle(v_Rectangle.Width + 32, 8, ClientSize.Width - v_Rectangle.Width - 16 - 32, ClientSize.Height - 16));
      });
      base.OnSizeChanged(e);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        m_DrawListRadio.MouseDown(e.Location);
        //m_DisplayButtonRefresh.MouseDown(e.Location);
        //m_DisplayButtonStart.MouseDown(e.Location);
        m_TextBoxMultiLine.MouseDown(e.Location);
      });
      base.OnMouseDown(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        m_DrawListRadio.MouseUp(e.Location);
        m_DisplayButtonRefresh.MouseUp(e.Location);
        m_DisplayButtonStart.MouseUp(e.Location);
        m_TextBoxMultiLine.MouseUp(e.Location);
        base.OnMouseUp(e);
      });
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        m_DrawListRadio.MouseMove(e.Location);
        m_DisplayButtonRefresh.MouseMove(e.Location);
        m_DisplayButtonStart.MouseMove(e.Location);
        m_TextBoxMultiLine.MouseMove(e.Location);
        base.OnMouseMove(e);
      });
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        m_DrawListRadio.MouseWheel(e.Location, e.Delta, ModifierKeys == Keys.Control);
        m_TextBoxMultiLine.MouseWheel(e.Location, e.Delta, ModifierKeys == Keys.Control);
        base.OnMouseWheel(e);
      });
    }

    protected override void OnMouseLeave(EventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        m_DisplayButtonRefresh.MouseLeave();
        m_DisplayButtonStart.MouseLeave();
        base.OnMouseLeave(e);
      });
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        m_DrawListRadio.Paint(e.Graphics, ForeColor, FormControlGlobals.ColorRecup);
        m_DisplayButtonRefresh.Paint(e.Graphics, ForeColor);
        m_DisplayButtonStart.Paint(e.Graphics, ForeColor);
        m_TextBoxMultiLine.Paint(e.Graphics, ForeColor);
      });
      base.OnPaint(e);
    }

    internal (string Name, ulong Addess) GetCurrentDevice()
    {
      ulong v_Addess = (ulong)m_DrawListRadio.SelectedItem();
      string? v_Name = m_DrawListRadio.GetItemText(v_Addess);
      return ((string)v_Name, (ulong)v_Addess);
    }

    internal void UpdateDevices(List<(string Name, ulong Addess, DateTime LastSeen)> p_Devices, string? p_LastUsedDevice)
    {
      if (m_DrawListRadio is not null && p_Devices is not null)
      {
        ulong v_LastUsedAddress = ulong.MaxValue;
        if (!ulong.TryParse(p_LastUsedDevice, out v_LastUsedAddress))
          v_LastUsedAddress = ulong.MaxValue;
        m_DrawListRadio.Clear();
        //obsolete, please delete the following line...
        m_DrawListRadio.AddItem(ulong.MaxValue - 1, "Playback RAW", "RawDevice", ulong.MaxValue - 1 == v_LastUsedAddress);
        foreach (var l_Device in p_Devices)
          m_DrawListRadio.AddItem(l_Device.Addess, l_Device.LastSeen.ToString(), l_Device.Name, l_Device.Addess == v_LastUsedAddress);
        m_DrawListRadio.AddItem(ulong.MaxValue, "Playback recordings", "CTL Player", ulong.MaxValue == v_LastUsedAddress);
        Invalidate(m_DrawListRadio.CanvasBounds);
      }
    }

    internal void SetStartStop(bool p_Stop)
    {
      m_DisplayButtonStart.Text = p_Stop ? "Stop" : "Start";
      Invalidate(m_DisplayButtonStart.CanvasBounds);
    }

    internal void SetText(string p_Text)
    {
      if (m_TextBoxMultiLine is not null)
      {
        m_TextBoxMultiLine.SetText(p_Text);
        Invalidate(m_TextBoxMultiLine.CanvasBounds);
      }
    }

    internal void AddLine(string p_Text)
    {
      if (m_TextBoxMultiLine is not null)
      {
        m_TextBoxMultiLine.AddLine(p_Text);
        Invalidate(m_TextBoxMultiLine.CanvasBounds);
      }
    }

    internal void AddError(Exception p_Exception)
    {
      if (m_TextBoxMultiLine is not null)
      {
        if (string.IsNullOrEmpty(m_TextBoxMultiLine.Text))
          m_TextBoxMultiLine.SetText(p_Exception.ToString());
        else
          m_TextBoxMultiLine.SetText($"{p_Exception.ToString()}{Environment.NewLine}---------------------------------------------------------------{Environment.NewLine}{m_TextBoxMultiLine.Text}");
        Invalidate(m_TextBoxMultiLine.CanvasBounds);
      }
    }
  }
}