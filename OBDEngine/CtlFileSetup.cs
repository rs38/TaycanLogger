using System.Xml.Linq;

namespace OBDEngine
{
  public class CtlFileSetup
  {
    private bool m_Master;
    private int m_Mcid4ctl = 0;

    public CtlFileSetup(bool p_Master = false)
    {
      m_Master = p_Master;
    }

    public string? CheckValidityAndEnrichWithmcid4ctl(XDocument p_XDocument)
    {
      try
      {
        XElement? v_XRoot = p_XDocument.Root;
        if (v_XRoot?.Name != "OBD2")
          return "XML node 'OBD2' not found.";
        string? v_Result = CheckElement(v_XRoot, "init");
        foreach (var l_XCommand in v_XRoot.Element("init").Elements("command"))
        {
          v_Result = CheckCommand(l_XCommand, true);
          if (v_Result is not null)
            return v_Result;
        }
        if (v_Result is not null)
          return v_Result;
        v_Result = CheckElement(v_XRoot, "rotation");
        if (v_Result is not null)
          return v_Result;
        foreach (var l_XCommand in v_XRoot.Element("rotation").Elements("command"))
        {
          v_Result = CheckCommand(l_XCommand);
          if (v_Result is not null)
            return v_Result;
        }
      }
      catch (Exception p_Exception)
      {
        return p_Exception.Message;
      }
      return null;
    }

    private string? CheckCommand(XElement? p_XCommand, bool p_Init = false)
    {
      if (p_XCommand?.Name != "command")
      {
        return "XML node 'command' not found.";
      }
      string? v_Result = CheckAttribute(p_XCommand, "send");
      if (v_Result is not null)
        return v_Result;

      //every command need to be identifiable in the ctl file stream...
      p_XCommand?.SetAttributeValue("mcid4ctl", m_Master ? --m_Mcid4ctl : ++m_Mcid4ctl);

      if (!p_Init)
      {
        v_Result = CheckAttribute(p_XCommand, "header");
        if (v_Result is not null)
          return v_Result;
        v_Result = CheckAttribute(p_XCommand, "skipCount");
        if (v_Result is not null)
          return v_Result;
        if (p_XCommand.HasElements)
        {
          v_Result = CheckElement(p_XCommand, "values");
          if (v_Result is not null)
            return v_Result;
          v_Result = CheckElement(p_XCommand, "values");
          if (v_Result is not null)
            return v_Result;
          var v_CommandValues = p_XCommand.Element("values");
          v_Result = CheckElement(v_CommandValues, "value");
          if (v_Result is not null)
            return v_Result;
          foreach (var l_CommandValue in v_CommandValues.Elements("value"))
          {
            v_Result = CheckCommandValue(l_CommandValue);
            if (v_Result is not null)
              return v_Result;
          }
        }
        else
        {
          v_Result = CheckCommandValue(p_XCommand);
          if (v_Result is not null)
            return v_Result;
        }
      }
      return null;
    }

    private string? CheckCommandValue(XElement p_XElement)
    {
      string? v_Result = CheckAttribute(p_XElement, "name");
      if (v_Result is not null)
        return v_Result;
      v_Result = CheckAttribute(p_XElement, "units");
      if (v_Result is not null)
        return v_Result;
      v_Result = CheckAttribute(p_XElement, "conversion");
      if (v_Result is not null)
        return v_Result;
      return v_Result;
    }

    private string? CheckElement(XElement p_XElement, string p_Name)
    {
      XElement? v_XElement = p_XElement.Element(p_Name);
      if (v_XElement is null)
        return $"XML node '{p_Name}' not found.";
      return null;
    }

    private string? CheckAttribute(XElement p_XElement, string p_Name)
    {
      XAttribute? v_XAttribute = p_XElement.Attribute(p_Name);
      if (v_XAttribute is null)
        return $"XML attribute '{p_Name}' not found.";
      return null;
    }
  }
}