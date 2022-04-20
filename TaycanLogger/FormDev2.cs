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

namespace TaycanLogger
{
  public partial class FormDev2 : Form
  {
    private AmpereGauge m_AmpereGauge;
    public FormDev2()
    {
      InitializeComponent();
      this.Padding = new Padding(5, 5, 5, 50);
      m_AmpereGauge = new AmpereGauge();
      //m_VoltGauge.Location = new Point(10, 10);
      //m_VoltGauge.Size = new Size(200, 200);
      m_AmpereGauge.Dock = DockStyle.Fill;
      m_AmpereGauge.ForeColor = DevGauge.ColorPower;
      Controls.Add(m_AmpereGauge);
      BackColor = SystemColors.ControlDark;
    }

    IDisposable? m_Disposable;

    //protected override void OnClick(EventArgs e)
    //{
    //  base.OnClick(e);
    //  this.Text = m_AmpereGauge.Flow.ToString();
    //  m_Disposable?.Dispose();
    //  m_Disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() => m_AmpereGauge.AddValue((float)Math.Sin(l / 10.0) * 50)));
    //  //m_Disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() =>
    //  //  m_VoltGauge.AddValue((float)Math.Max(((int)(l / 10f) % 3) / 2f * 100, 1))));
    //  //this.Text = m_VoltGauge.FlowDirection.ToString();
    //}

    //protected override void OnDoubleClick(EventArgs e)
    //{
    //  base.OnDoubleClick(e);
    //  m_AmpereGauge.Flow = (FlowDirection)((int)m_AmpereGauge.Flow + 1);
    //  this.Text = m_AmpereGauge.Flow.ToString() + " " + m_AmpereGauge.Size.ToString();
    //  m_AmpereGauge.Reset();
    //  m_Disposable?.Dispose();
    //  m_Disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() => m_AmpereGauge.AddValue((float)Math.Sin(l / 10.0) * 50)));
    //  //m_Disposable = Observable.Interval(TimeSpan.FromMilliseconds(30)).Subscribe(l => this.InvokeIfRequired(() =>
    //  //  m_AmpereGauge.AddValue((float)Math.Max(((int)(l / 10f) % 3) / 2f * 100, 1))));
    //}

    //protected override void OnResize(EventArgs e)
    //{
    //  base.OnResize(e);
    //  if (m_AmpereGauge != null)
    //    this.Text = m_AmpereGauge.Flow.ToString() + " " + m_AmpereGauge.Size.ToString();
    //}
  }
}
