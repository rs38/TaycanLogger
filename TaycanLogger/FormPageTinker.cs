using System.Xml.Linq;

namespace TaycanLogger
{
  public class FormPageTinker : FormPage
  {
    private TextBox tbXML;
    private TextBox tbResultRaw;
    private TextBox tbResultValue;

    public FormPageTinker(int p_ColumnSpan) : base(p_ColumnSpan)
    {
      tbXML = new System.Windows.Forms.TextBox();
      tbResultRaw = new System.Windows.Forms.TextBox();
      tbResultValue = new System.Windows.Forms.TextBox();
    }

    public override void Load()
    {
      base.Load();
      ColumnCount = 1;
      ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      Controls.Add(this.tbXML, 0, 0);
      Controls.Add(this.tbResultRaw, 0, 1);
      Controls.Add(this.tbResultValue, 0, 2);
      Name = "tableLayoutPanel1";
      RowCount = 3;
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
      RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
      tbXML.BackColor = System.Drawing.Color.Black;
      tbXML.BorderStyle = System.Windows.Forms.BorderStyle.None;
      tbXML.Dock = System.Windows.Forms.DockStyle.Fill;
      tbXML.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
      tbXML.ForeColor = System.Drawing.Color.White;
      tbXML.Margin = new System.Windows.Forms.Padding(4);
      tbXML.Multiline = true;
      tbXML.Name = "tbXML";
      tbXML.TabIndex = 0;
      tbXML.WordWrap = false;
      tbResultRaw.BackColor = System.Drawing.Color.Black;
      tbResultRaw.BorderStyle = System.Windows.Forms.BorderStyle.None;
      tbResultRaw.Dock = System.Windows.Forms.DockStyle.Fill;
      tbResultRaw.Font = tbXML.Font;
      tbResultRaw.ForeColor = System.Drawing.Color.White;
      tbResultRaw.Margin = new System.Windows.Forms.Padding(4);
      tbResultRaw.Multiline = true;
      tbResultRaw.Name = "tbResultRaw";
      tbResultRaw.TabIndex = 1;
      tbResultRaw.WordWrap = false;
      tbResultValue.BackColor = System.Drawing.Color.Black;
      tbResultValue.BorderStyle = System.Windows.Forms.BorderStyle.None;
      tbResultValue.Dock = System.Windows.Forms.DockStyle.Fill;
      tbResultValue.Font = tbXML.Font;
      tbResultValue.ForeColor = System.Drawing.Color.White;
      tbResultValue.Margin = new System.Windows.Forms.Padding(4);
      tbResultValue.Multiline = true;
      tbResultValue.Name = "tbResultValue";
      tbResultValue.TabIndex = 2;
      tbResultValue.WordWrap = false;
      tbXML.Text = "<command send=\"221E33\" header=\"atsh7e5\" skipCount=\"15\" >\r\n  <values>\r\n    <value name=\"CellVoltMax\" conversion=\"((B3*256)+B4)/10000\" units=\"V\"/>\r\n    <value name=\"Cell#Max\" conversion=\"B5\" units=\"#\"/>\r\n  </values>\r\n</command>";
      tbXML.TextChanged += TbXML_TextChanged;
    }

    public event Action<string> XMLCommandChanged;

    private void TbXML_TextChanged(object? sender, EventArgs e)
    {
      Form1.ExceptionCheck(() =>
      {
        bool v_Check = CheckXml(tbXML.Text);
        System.Diagnostics.Debug.WriteLine(v_Check);
        if (v_Check)
          XMLCommandChanged?.Invoke(tbXML.Text);
      });
    }

    private bool CheckXml(string p_Xml)
    {
      tbResultValue.Text = string.Empty;
      if (string.IsNullOrEmpty(p_Xml))
      {
        tbResultValue.Text = "Error: the command window is empty.";
        return false;
      }
      try
      {
        XDocument v_XDocument = XDocument.Load(new StringReader(p_Xml), LoadOptions.None);
        XElement? v_XCommand = v_XDocument.Root;
        if (v_XCommand?.Name != "command")
        {
          tbResultValue.Text = "XML node 'command' not found.";
          return false;
        }
        if (!CheckAttribute(v_XCommand, "send"))
          return false;
        if (!CheckAttribute(v_XCommand, "header"))
          return false;
        if (!CheckAttribute(v_XCommand, "skipCount"))
          return false;
        bool v_Result = true;
        if (v_XCommand.HasElements)
        {
          if (!CheckElement(v_XCommand, "values"))
            return false;
          var v_CommandValues = v_XCommand.Element("values");
          if (!CheckElement(v_CommandValues, "value"))
            return false;
          v_CommandValues.Elements("value")?.ForEach(l_XValue => v_Result &= CheckCommandValue(l_XValue));
        }
        else
          v_Result &= CheckCommandValue(v_XCommand);
        return v_Result;
      }
      catch (Exception p_Exception)
      {
        tbResultValue.Text = p_Exception.Message;
        return false;
      }
    }

    private bool CheckCommandValue(XElement p_XElement)
    {
      if (!CheckAttribute(p_XElement, "name"))
        return false;
      if (!CheckAttribute(p_XElement, "units"))
        return false;
      if (!CheckAttribute(p_XElement, "conversion"))
        return false;
      return true;
    }

    private bool CheckElement(XElement p_XElement, string p_Name)
    {
      XElement? v_XElement = p_XElement.Element(p_Name);
      if (v_XElement is null)
      {
        AddErrorMessage($"XML node '{p_Name}' not found.");
        return false;
      }
      return true;
    }

    private bool CheckAttribute(XElement p_XElement, string p_Name)
    {
      XAttribute? v_XAttribute = p_XElement.Attribute(p_Name);
      if (v_XAttribute is null)
      {
        AddErrorMessage($"XML attribute '{p_Name}' not found.");
        return false;
      }
      return true;
    }

    public void AddErrorMessage(string p_ErrorMessage)
    {
      if (string.IsNullOrEmpty(tbResultValue.Text))
        tbResultValue.Text = p_ErrorMessage;
      else
        tbResultValue.Text = p_ErrorMessage + Environment.NewLine + tbResultValue.Text;
    }

    public override void ShowResultValue(string p_Name, string p_Units, double p_Value)
    {
      string v_Result = $"{p_Name}: {p_Value} {p_Units}";
      if (string.IsNullOrEmpty(tbResultValue.Text))
        tbResultValue.Text = v_Result;
      else
        tbResultValue.Text = v_Result + Environment.NewLine + tbResultValue.Text;
    }

    public override void ShowResultRaw(byte[] p_ResultRaw, byte[] p_ResultProcessed)
    {
      string v_ResultProcessed = Convert.ToHexString(p_ResultProcessed);
      Func<string, string> v_AddSpace = s => string.Join(null, Enumerable.Range(0, s.Length / 2).Select(i => s.Substring(i * 2, 2).PadLeft(3, ' ')).ToList());
      Func<string, string> v_AddBIndex = s => string.Join(null, Enumerable.Range(0, s.Length / 2).Select(i => $"B{i % 100}".PadLeft(3, ' ')).ToList());
      Func<byte[], string> v_ToDecimals = b => string.Join(null, Enumerable.Range(0, b.Length).Select(i => b[i].ToString().PadLeft(3, ' ')).ToList());
      Func<string, string> v_ToASCIIEscaped = s => string.Join(null, Enumerable.Range(0, s.Length).Select(i => s.Substring(i, 1) switch
      {
        "\0" => " \\0",  // - Unicode character 0
        "\a" => " \\a",  // - Alert(character 7)
        "\b" => " \\b",  // - Backspace(character 8)
        "\t" => " \\t",  // - Horizontal tab(character 9)
        "\n" => " \\n",  // - New line(character 10)
        "\v" => " \\v",  // - Vertical quote(character 11)
        "\f" => " \\f",  // - Form feed(character 12)
        "\r" => " \\r",  // - Carriage return (character 13)
        _ => s.Substring(i, 1).PadLeft(3, ' ')
      }).ToList());
      tbResultRaw.Text = $"{v_AddSpace(Convert.ToHexString(p_ResultRaw))}{Environment.NewLine}{v_ToDecimals(p_ResultRaw)}{Environment.NewLine}{v_ToASCIIEscaped(System.Text.Encoding.ASCII.GetString(p_ResultRaw))}{ Environment.NewLine}{ Environment.NewLine}{ v_AddSpace(v_ResultProcessed)}{Environment.NewLine}{v_AddBIndex(v_ResultProcessed)}";
    }
  }
}
