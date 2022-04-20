using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OBDEngine
{
  public class WriteRawStream : Stream
  {
    private Stream m_Stream;
    private BinaryWriter m_BinaryWriter;
    private SemaphoreSlim m_SemaphoreSlim;

    public WriteRawStream(string p_DeviceName, Stream p_Stream)
    {
      m_Stream = p_Stream;
      m_SemaphoreSlim = new SemaphoreSlim(1);
      string v_DongleName = p_DeviceName;
      foreach (var c in Path.GetInvalidFileNameChars())
        v_DongleName = v_DongleName.Replace(c, '-');
      m_BinaryWriter = new BinaryWriter(File.Open(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{v_DongleName}-{DateTime.Now:yyMMddHHmmss}.raw"), FileMode.Create));
    }

    public override bool CanRead => m_Stream.CanRead;

    public override bool CanSeek => m_Stream.CanSeek;

    public override bool CanWrite => m_Stream.CanWrite;

    public override long Length => m_Stream.Length;

    public override long Position { get => m_Stream.Position; set => m_Stream.Position = value; }

    public override void Flush()
    {
      m_Stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int v_Read = m_Stream.Read(buffer, offset, count);
      Log(buffer, offset, v_Read, false);
      return v_Read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return m_Stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      m_Stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      Log(buffer, offset, count, true);
      m_Stream.Write(buffer, offset, count);
    }

    private bool m_Written;

    private void Log(byte[] p_Buffer, int offset, int p_Count, bool p_Sent)
    {
      m_SemaphoreSlim.Wait(2000);
      try
      {
        if (p_Sent)
          m_Written = false;
        if (!m_Written)
        {
          m_BinaryWriter.Write(p_Count * (p_Sent ? -1 : 1));
          m_BinaryWriter.Write(DateTime.Now.ToBinary());
          if (!p_Sent)
            m_Written = true;
        }
        m_BinaryWriter.Write(p_Buffer, offset, p_Count);
        m_BinaryWriter.Flush();
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
    }

    public override void Close()
    {
      m_SemaphoreSlim.Wait(2000);
      try
      {
        m_BinaryWriter.Flush();
        m_BinaryWriter.Close();
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
    }
  }
}