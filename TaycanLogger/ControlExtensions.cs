using System.Configuration;

namespace TaycanLogger
{
  public static class ControlExtensions
  {
    public static void ForEach<T>(this IEnumerable<T> p_Enumerable, Action<T> p_Action)
    {
      foreach (T element in p_Enumerable)
        p_Action(element);
    }

    public static void LoadFormSizeState(this Form p_Form, string? p_Name = null)
    {
      string? v_WindowSize = ControlExtensions.ReadAppSetting($"WindowSize{p_Name}");
      if (v_WindowSize != null)
      {
        RectangleConverter v_RectangleConverter = new RectangleConverter();
        Rectangle? v_WindowRect = (Rectangle?)v_RectangleConverter.ConvertFromString(v_WindowSize);
        if (v_WindowRect != null)
        {
          p_Form.Left = v_WindowRect.Value.Left;
          p_Form.Top = v_WindowRect.Value.Top;
          p_Form.Width = v_WindowRect.Value.Width;
          p_Form.Height = v_WindowRect.Value.Height;
        }
      }
      string? v_WindowState = ControlExtensions.ReadAppSetting($"WindowState{p_Name}");
      if (v_WindowState != null)
        p_Form.WindowState = (FormWindowState)Enum.Parse(p_Form.WindowState.GetType(), v_WindowState);
    }

    public static void SaveFormSizeState(this Form p_Form, string? p_Name = null)
    {
      RectangleConverter v_RectangleConverter = new RectangleConverter();
      if (p_Form.WindowState == FormWindowState.Normal)
        WriteAppSetting($"WindowSize{p_Name}", v_RectangleConverter.ConvertToString(p_Form.Bounds));
      WriteAppSetting($"WindowState{p_Name}", p_Form.WindowState.ToString());
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

    public static void InvokeIfRequired(this Control p_Control, Action p_Action)
    {
      try
      {
        if (p_Control.InvokeRequired)
          p_Control.Invoke(new Action(() => p_Action()));
        else
          p_Action();
      }
      catch (ObjectDisposedException) { }
    }


  }
}