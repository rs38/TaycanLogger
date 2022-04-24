﻿using System.Text;
using System.Xml.Linq;

namespace OBDEngine
{
  internal class OBDCommandInit
  {
    protected byte[] m_Command;

    public OBDCommandInit(XElement p_XElement)
    {
      m_Command = Encoding.ASCII.GetBytes((string)p_XElement.Attribute("send") + '\r');
    }

    public void Execute(Stream p_Stream, byte[] p_Buffer, bool p_Tinker, Action<int>? p_BufferRead = null, byte[]? p_Command = null)
    {
      byte[] v_Command = m_Command;
      if (p_Command != null)
        v_Command = p_Command;

      // we do not want to record tinker commands...
      if (p_Tinker && p_Stream is WriteRawStream)
        (p_Stream as WriteRawStream)?.BaseStream.Write(v_Command, 0, v_Command.Length);
      else
        p_Stream.Write(v_Command, 0, v_Command.Length);
      int v_BytesRead = 0;
      while (!p_Buffer.Contains((byte)'>'))
      {
        // we do not want to record tinker commands...
        if (p_Tinker && p_Stream is WriteRawStream)
          v_BytesRead += (p_Stream as WriteRawStream).BaseStream.Read(p_Buffer, v_BytesRead, p_Buffer.Length - v_BytesRead);
        else
          v_BytesRead += p_Stream.Read(p_Buffer, v_BytesRead, p_Buffer.Length - v_BytesRead);
      }
      if (p_BufferRead is not null)
        p_BufferRead(v_BytesRead);
      Array.Clear(p_Buffer, 0, v_BytesRead);
    }

    protected byte[]? ProcessResult(byte[] p_Buffer)
    {
      string v_Result = System.Text.Encoding.UTF8.GetString(p_Buffer);

      if (v_Result.Contains("DATA") || v_Result.StartsWith("7F") ||
          v_Result.Contains("STOPPED") || v_Result.Contains("CAN ERROR"))
        return null;

      char[] charsToTrim = { '\r', ' ', '>', '\0' };
      if (v_Result.Contains(":"))
      {
        try
        {
          var sb = new System.Text.StringBuilder();

          var lines = v_Result.Split(':');
          for (int i = 1; i < lines.Length; i++)
          {
            int cr = lines[i].IndexOf("\r");
            sb.Append(lines[i].Substring(0, cr > 0 ? cr : lines[i].Length).Trim(charsToTrim));
          }
          v_Result = sb.ToString();
        }
        catch
        {
          return null;
        }
      }
      return Convert.FromHexString(v_Result.Replace(" ", "").Trim(charsToTrim));
    }
  }

  internal class OBDCommand : OBDCommandInit
  {
    private string m_Header;
    private int m_SkipCount;
    private List<OBDValue> m_OBDValues;

    public string Header { get => m_Header; }
    public int SkipCount { get => m_SkipCount; }

    public OBDCommand(XElement p_XElement) : base(p_XElement)
    {
      m_Header = ((string)p_XElement.Attribute("header")).ToUpper();
      m_SkipCount = (int)p_XElement.Attribute("skipCount") + 1;
      if (p_XElement.HasElements)
        m_OBDValues = (from p_XElementValue in p_XElement.Element("values").Elements("value") select new OBDValue(p_XElementValue)).ToList();
      else
      {
        m_OBDValues = new List<OBDValue>();
        m_OBDValues.Add(new OBDValue(p_XElement));
      }
    }

    public bool Execute(Stream p_Stream, byte[] p_Buffer, bool p_SendHeader, Action<OBDValue, double> p_ResultValue, Action<byte[], byte[]>? p_ResultRaw = null)
    {
      bool v_Result = false;
      if (p_SendHeader)
        base.Execute(p_Stream, p_Buffer, p_ResultRaw is not null, null, Encoding.ASCII.GetBytes(m_Header + '\r'));
      base.Execute(p_Stream, p_Buffer, p_ResultRaw is not null, p_BytesRead =>
      {
        var v_Buffer = ProcessResult(p_Buffer);
        if (v_Buffer != null)
        {
          if (p_ResultRaw != null)
          {
            byte[] v_BufferRaw = new byte[p_BytesRead];
            Array.Copy(p_Buffer, v_BufferRaw, v_BufferRaw.Length);
            p_ResultRaw(v_BufferRaw, v_Buffer);
          }
          m_OBDValues?.ForEach(l_OBDValue => p_ResultValue(l_OBDValue, l_OBDValue.Execute(v_Buffer)));
          v_Result = true;
        }
      });
      m_ExecuteCount++;
      return v_Result;
    }

    public uint m_ExecuteCount = 0;

    public override string ToString()
    {
      return $"{m_Header} {Convert.ToHexString(m_Command)} {m_ExecuteCount} executed";
    }

    public static bool operator ==(OBDCommand p_OBDCommand1, OBDCommand p_OBDCommand2)
    {
      return p_OBDCommand1.Equals(p_OBDCommand2);
    }

    public static bool operator !=(OBDCommand p_OBDCommand1, OBDCommand p_OBDCommand2)
    {
      return !p_OBDCommand1.Equals(p_OBDCommand2);
    }

    public bool Equals(OBDCommand? p_OBDCommand)
    {
      return m_Command.SequenceEqual(p_OBDCommand?.m_Command) && Header == p_OBDCommand.Header;
    }

  }
}