using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaycanLogger
{
  public partial class FormMain : Form
  {
    public FormMain()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      //load app settings
      string? v_WindowSize = ReadAppSetting("WindowSize");
      if (v_WindowSize != null)
      {
        RectangleConverter v_RectangleConverter = new RectangleConverter();
        Rectangle? v_WindowRect = (Rectangle?)v_RectangleConverter.ConvertFromString(v_WindowSize);
        if (v_WindowRect != null)
        {
          Left = v_WindowRect.Value.Left;
          Top = v_WindowRect.Value.Top;
          Width = v_WindowRect.Value.Width;
          Height = v_WindowRect.Value.Height;
        }
      }
      string? v_WindowState = ReadAppSetting("WindowState");
      if (v_WindowState != null)
        WindowState = (FormWindowState)Enum.Parse(WindowState.GetType(), v_WindowState);


    }

    protected override void OnClosed(EventArgs e)
    {
      //write app settings
      RectangleConverter v_RectangleConverter = new RectangleConverter();
      if (WindowState == FormWindowState.Normal)
        WriteAppSetting("WindowSize", v_RectangleConverter.ConvertToString(this.Bounds));
      WriteAppSetting("WindowState", WindowState.ToString());

      base.OnClosed(e);
    }





    /// <summary>
    /// Use to read and write app setting
    /// </summary>
    public static string? ReadAppSetting(string p_Key)
    {
      string? v_Value = null;
      var v_Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var v_Settings = v_Configuration.AppSettings.Settings;
      if (v_Settings[p_Key] is not null)
        v_Value = v_Settings[p_Key].Value;
      v_Configuration.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(v_Configuration.AppSettings.SectionInformation.Name);
      return v_Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="p_Key"></param>
    /// <param name="p_Value">if Value is empty, we remove the key from the settings</param>
    public static void WriteAppSetting(string p_Key, string? p_Value)
    {
      var v_Configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      var v_Settings = v_Configuration.AppSettings.Settings;
      if (v_Settings[p_Key] is null && p_Value is not null)
        v_Settings.Add(p_Key, p_Value);
      if (v_Settings[p_Key] is not null && p_Value is not null)
        v_Settings[p_Key].Value = p_Value;
      if (v_Settings[p_Key] is not null && p_Value is null)
        v_Settings.Remove(p_Key);
      v_Configuration.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(v_Configuration.AppSettings.SectionInformation.Name);
    }
  }
}
