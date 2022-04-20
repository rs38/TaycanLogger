namespace TaycanLogger
{
  internal class FormPageSettings : FormPage
  {
    private ComboBox comboBox1;
    private Button btRefresh;
    private TextBox tbError;

    public override Type Type { get => this.GetType(); }

    public string? CurrentDevice { get => (string?)comboBox1.SelectedItem; }

    public FormPageSettings(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      comboBox1 = new System.Windows.Forms.ComboBox();
      btRefresh = new System.Windows.Forms.Button();
      tbError = new System.Windows.Forms.TextBox();
      if (Form1.GlobalErrorDisplay is null)
        Form1.GlobalErrorDisplay = p_Exception => ShowErrorMessage(p_Exception);
    }

    public override void Load()
    {
      base.Load();
      comboBox1.BackColor = System.Drawing.Color.Black;
      comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      comboBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
      comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      comboBox1.Font = Parent.Font;
      comboBox1.ForeColor = System.Drawing.Color.White;
      comboBox1.FormattingEnabled = true;
      comboBox1.Margin = new System.Windows.Forms.Padding(4, 6, 4, 4);
      comboBox1.Name = "comboBox1";
      comboBox1.Sorted = true;
      comboBox1.TabIndex = 3;
      comboBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(comboBox1_DrawItem);
      btRefresh.BackColor = System.Drawing.Color.Black;
      btRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
      btRefresh.FlatAppearance.BorderSize = 0;
      btRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
      btRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
      btRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      btRefresh.Font = Parent.Font;
      btRefresh.ForeColor = System.Drawing.Color.White;
      btRefresh.Margin = new System.Windows.Forms.Padding(4);
      btRefresh.Name = "btRefresh";
      btRefresh.TabIndex = 4;
      btRefresh.Text = "Refresh";
      btRefresh.UseVisualStyleBackColor = false;
      btRefresh.Click += new System.EventHandler(btRefresh_Click);
      tbError.BackColor = System.Drawing.Color.Black;
      tbError.BorderStyle = System.Windows.Forms.BorderStyle.None;
      tbError.Dock = System.Windows.Forms.DockStyle.Fill;
      tbError.Font = Parent.Font;
      tbError.ForeColor = System.Drawing.Color.White;
      tbError.Margin = new System.Windows.Forms.Padding(4);
      tbError.Multiline = true;
      tbError.Name = "tbError";
      tbError.TabIndex = 9;
      tbError.WordWrap = false;
      ColumnCount = 3;
      ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
      ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140F));
      ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
      RowCount = 3;
      RowStyles.Add(new RowStyle(SizeType.Absolute, 47F));
      RowStyles.Add(new RowStyle(SizeType.Absolute, 47F));
      RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
      Controls.Add(comboBox1, 0, 0);
      Controls.Add(btRefresh, 1, 0);
      SetColumnSpan(tbError, 3);
      Controls.Add(tbError, 0, 2);
    }

    public event Action RefreshDevices;

    private void btRefresh_Click(object sender, EventArgs e)
    {
      RefreshDevices?.Invoke();
    }

    public void ShowErrorMessage(Exception p_Exception)
    {
      this.InvokeIfRequired(() =>
      {
        if (string.IsNullOrEmpty(tbError.Text))
          tbError.Text = p_Exception.ToString();
        else
        {
          tbError.Text = $"{p_Exception.ToString()}{Environment.NewLine}---------------------------------------------------------------{Environment.NewLine}{tbError.Text}";
        }
        OnActivateRequested();
      });
    }

    public override void UpdateDevices(string[]? p_Devices)
    {
      comboBox1.Items.Clear();
      comboBox1.Items.AddRange(p_Devices);
      string? v_LastUsedDevice = ControlExtensions.ReadAppSetting("LastUsedDevice");
      if (!string.IsNullOrEmpty(v_LastUsedDevice))
        comboBox1.SelectedItem = v_LastUsedDevice;
      if (comboBox1.Items.Count > 0 && comboBox1.SelectedIndex < 0)
        comboBox1.SelectedIndex = 0;
    }

    private void comboBox1_DrawItem(object sender, DrawItemEventArgs e)
    {
      if (comboBox1.Items.Count > 0)
      {
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        e.DrawBackground();
        if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
        {
          e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(53, 53, 53)), e.Bounds);
          e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(), comboBox1.Font, Brushes.White, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
        }
        else if (e.Index >= 0)
        {
          e.Graphics.FillRectangle(Brushes.Black, e.Bounds);
          e.Graphics.DrawString(comboBox1.Items[e.Index].ToString(), comboBox1.Font, Brushes.White, new Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height));
        }
      }
    }
  }
}