using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace OBDEngine
{
  public class OBDValue
  {
    private string m_Name;
    private string m_Units;
    private int m_IndexMax;
    private Func<byte[], double> m_Convert;

    public string Name { get => m_Name; }
    public string Units { get => m_Units; }

    public OBDValue(XElement p_XElement)
    {
      m_Name = (string)p_XElement.Attribute("name");
      m_Units = (string)p_XElement.Attribute("units");
      SetConversion((string)p_XElement.Attribute("conversion"));
    }

    public void SetConversion(string p_Conversion)
    {
      if (!string.IsNullOrEmpty(p_Conversion))
      {
        var v_ConversionParser = new ConversionParser(p_Conversion.ToUpper());
        var v_Expression = v_ConversionParser.ParseFormula(out m_IndexMax);
        m_Convert = Expression.Lambda<Func<byte[], double>>(v_Expression, v_ConversionParser.ParameterArray).Compile();
      }
    }

    internal double Execute(byte[] p_Buffer)
    {
      if (p_Buffer.Length - 1 < m_IndexMax)
        return double.NaN;
      return m_Convert(p_Buffer);
    }
  }
}