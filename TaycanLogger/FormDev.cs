using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace TaycanLogger
{
  public partial class FormDev : Form
  {
    private DevGauge m_DevGauge;

    public FormDev()
    {
      InitializeComponent();
      this.Padding = new Padding(5, 5, 5, 50);
      m_DevGauge = new DevGauge();
      //m_VoltGauge.Location = new Point(10, 10);
      //m_VoltGauge.Size = new Size(200, 200);
      m_DevGauge.Dock = DockStyle.Fill;
      m_DevGauge.ForeColor = DevGauge.ColorPower;
      m_DevGauge.ValueMin = -50;
      m_DevGauge.ValueMax = 200;
      m_DevGauge.ForeColor = DevGauge.ColorPower;
      Controls.Add(m_DevGauge);
      BackColor = SystemColors.ControlDark;

    }

    IDisposable? m_Disposable;

    private void FormDev_Click(object sender, EventArgs e)
    {
      m_Disposable?.Dispose();
      //disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() => m_VoltGauge.AddValue((float)Math.Sin(l / 10.0) * 50 + 50)));
      m_Disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() =>
        m_DevGauge.AddValue((float)Math.Max(((int)(l / 10f) % 3) / 2f * 100, 1))));
      this.Text = m_DevGauge.FlowDirection.ToString();
    }

    protected override void OnMouseDoubleClick(MouseEventArgs e)
    {
      base.OnMouseDoubleClick(e);
      m_DevGauge.FlowDirection = (StartPinFlow)((int)m_DevGauge.FlowDirection + 1);
      this.Text = m_DevGauge.FlowDirection.ToString();
      m_DevGauge.Reset();
      m_Disposable?.Dispose();
      //disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() => m_VoltGauge.AddValue((float)Math.Sin(l / 10.0) * 50 + 50)));
      m_Disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() =>
        m_DevGauge.AddValue((float)Math.Max(((int)(l / 10f) % 3) / 2f * 100, 1))));
    }

  }


}

