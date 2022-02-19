
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogFormMain));
            this.buttonDoLog = new System.Windows.Forms.Button();
            this.comboBoxCOMPort = new System.Windows.Forms.ComboBox();
            this.textBoxDebug = new System.Windows.Forms.TextBox();
            this.buttonStop = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBoxIsDebug = new System.Windows.Forms.CheckBox();
            this.numericUpDownWaitMs = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWaitMs)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonDoLog
            // 
            this.buttonDoLog.Location = new System.Drawing.Point(1642, 852);
            this.buttonDoLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonDoLog.Name = "buttonDoLog";
            this.buttonDoLog.Size = new System.Drawing.Size(252, 142);
            this.buttonDoLog.TabIndex = 0;
            this.buttonDoLog.Text = "Log!";
            this.buttonDoLog.UseVisualStyleBackColor = true;
            this.buttonDoLog.Click += new System.EventHandler(this.ButtonDoLog_Click);
            // 
            // comboBoxCOMPort
            // 
            this.comboBoxCOMPort.FormattingEnabled = true;
            this.comboBoxCOMPort.Location = new System.Drawing.Point(1760, 1018);
            this.comboBoxCOMPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.comboBoxCOMPort.Name = "comboBoxCOMPort";
            this.comboBoxCOMPort.Size = new System.Drawing.Size(134, 33);
            this.comboBoxCOMPort.TabIndex = 2;
            this.comboBoxCOMPort.Text = "COM1";
            this.comboBoxCOMPort.SelectedIndexChanged += new System.EventHandler(this.comboBoxCOMPort_SelectedIndexChanged);
            // 
            // textBoxDebug
            // 
            this.textBoxDebug.Location = new System.Drawing.Point(31, 618);
            this.textBoxDebug.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBoxDebug.Multiline = true;
            this.textBoxDebug.Name = "textBoxDebug";
            this.textBoxDebug.Size = new System.Drawing.Size(1867, 205);
            this.textBoxDebug.TabIndex = 3;
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(1361, 938);
            this.buttonStop.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(206, 58);
            this.buttonStop.TabIndex = 4;
            this.buttonStop.Text = "Stop";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 1010);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 42);
            this.button2.TabIndex = 5;
            this.button2.Text = "test";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // checkBoxIsDebug
            // 
            this.checkBoxIsDebug.AutoSize = true;
            this.checkBoxIsDebug.Checked = true;
            this.checkBoxIsDebug.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIsDebug.Location = new System.Drawing.Point(14, 964);
            this.checkBoxIsDebug.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.checkBoxIsDebug.Name = "checkBoxIsDebug";
            this.checkBoxIsDebug.Size = new System.Drawing.Size(90, 29);
            this.checkBoxIsDebug.TabIndex = 7;
            this.checkBoxIsDebug.Text = "debug";
            this.checkBoxIsDebug.UseVisualStyleBackColor = true;
            this.checkBoxIsDebug.CheckedChanged += new System.EventHandler(this.checkBoxIsDebug_CheckedChanged);
            // 
            // numericUpDownWaitMs
            // 
            this.numericUpDownWaitMs.Location = new System.Drawing.Point(79, 885);
            this.numericUpDownWaitMs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
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
            this.numericUpDownWaitMs.Size = new System.Drawing.Size(87, 31);
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
            this.label1.Location = new System.Drawing.Point(171, 892);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 25);
            this.label1.TabIndex = 10;
            this.label1.Text = "ms delay";
            // 
            // LogFormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1941, 1078);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownWaitMs);
            this.Controls.Add(this.checkBoxIsDebug);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.textBoxDebug);
            this.Controls.Add(this.comboBoxCOMPort);
            this.Controls.Add(this.buttonDoLog);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LogFormMain";
            this.Text = "TayCANBusLog v0.03";
            this.Load += new System.EventHandler(this.LogFormMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownWaitMs)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonDoLog;
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

