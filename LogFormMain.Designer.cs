
namespace TaycanLogger
{
    partial class LogFormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogFormMain));
            this.button1 = new System.Windows.Forms.Button();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.comboBoxCOMPort = new System.Windows.Forms.ComboBox();
            this.textBoxDebug = new System.Windows.Forms.TextBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBoxIsDebug = new System.Windows.Forms.CheckBox();
            this.numericUpDownWaitMs = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWaitMs)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1478, 682);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(227, 114);
            this.button1.TabIndex = 0;
            this.button1.Text = "Log!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chart1
            // 
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(28, 12);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1681, 464);
            this.chart1.TabIndex = 1;
            this.chart1.Text = "chart1";
            this.chart1.Click += new System.EventHandler(this.chart1_Click);
            // 
            // comboBoxCOMPort
            // 
            this.comboBoxCOMPort.FormattingEnabled = true;
            this.comboBoxCOMPort.Location = new System.Drawing.Point(1584, 814);
            this.comboBoxCOMPort.Name = "comboBoxCOMPort";
            this.comboBoxCOMPort.Size = new System.Drawing.Size(121, 28);
            this.comboBoxCOMPort.TabIndex = 2;
            this.comboBoxCOMPort.Text = "COM1";
            this.comboBoxCOMPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOMPort_SelectedIndexChanged);
            // 
            // textBoxDebug
            // 
            this.textBoxDebug.Location = new System.Drawing.Point(28, 494);
            this.textBoxDebug.Multiline = true;
            this.textBoxDebug.Name = "textBoxDebug";
            this.textBoxDebug.Size = new System.Drawing.Size(1681, 165);
            this.textBoxDebug.TabIndex = 3;
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(1225, 750);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(185, 46);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(12, 808);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(81, 34);
            this.button2.TabIndex = 5;
            this.button2.Text = "test";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBoxIsDebug
            // 
            this.checkBoxIsDebug.AutoSize = true;
            this.checkBoxIsDebug.Checked = true;
            this.checkBoxIsDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsDebug.Location = new System.Drawing.Point(13, 771);
            this.checkBoxIsDebug.Name = "checkBoxIsDebug";
            this.checkBoxIsDebug.Size = new System.Drawing.Size(80, 24);
            this.checkBoxIsDebug.TabIndex = 7;
            this.checkBoxIsDebug.Text = "debug";
            this.checkBoxIsDebug.UseVisualStyleBackColor = true;
            this.checkBoxIsDebug.CheckedChanged += new System.EventHandler(this.checkBoxIsDebug_CheckedChanged);
            // 
            // numericUpDownWaitMs
            // 
            this.numericUpDownWaitMs.Location = new System.Drawing.Point(71, 708);
            this.numericUpDownWaitMs.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownWaitMs.Minimum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownWaitMs.Name = "numericUpDownWaitMs";
            this.numericUpDownWaitMs.Size = new System.Drawing.Size(78, 26);
            this.numericUpDownWaitMs.TabIndex = 9;
            this.numericUpDownWaitMs.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            this.numericUpDownWaitMs.ValueChanged += new System.EventHandler(this.numericUpDownWaitMs_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 714);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 20);
            this.label1.TabIndex = 10;
            this.label1.Text = "ms delay";
            // 
            // LogFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1747, 862);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownWaitMs);
            this.Controls.Add(this.checkBoxIsDebug);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.textBoxDebug);
            this.Controls.Add(this.comboBoxCOMPort);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogFormMain";
            this.Text = "TayCANBusLog v0.03";
            this.Load += new System.EventHandler(this.LogFormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWaitMs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ComboBox comboBoxCOMPort;
        private System.Windows.Forms.TextBox textBoxDebug;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBoxIsDebug;
        private System.Windows.Forms.NumericUpDown numericUpDownWaitMs;
        private System.Windows.Forms.Label label1;
    }
}

