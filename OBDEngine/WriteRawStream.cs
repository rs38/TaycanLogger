using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OBDEngine
{
  public class WriteRawStream : Stream
  {
    public Stream BaseStream;
    private BinaryWriter m_BinaryWriter;
    private SemaphoreSlim m_SemaphoreSlim;

    public WriteRawStream(string p_DeviceName, Stream p_Stream)
    {
      BaseStream = p_Stream;
      m_SemaphoreSlim = new SemaphoreSlim(1);
      string v_DongleName = p_DeviceName;
      foreach (var c in Path.GetInvalidFileNameChars())
        v_DongleName = v_DongleName.Replace(c, '-');
      m_BinaryWriter = new BinaryWriter(File.Open(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{v_DongleName}-{DateTime.Now:yyMMddHHmmss}.raw"), FileMode.Create));
    }

    public override bool CanRead => BaseStream.CanRead;

    public override bool CanSeek => BaseStream.CanSeek;

    public override bool CanWrite => BaseStream.CanWrite;

    public override long Length => BaseStream.Length;

    public override long Position { get => BaseStream.Position; set => BaseStream.Position = value; }

    public override void Flush()
    {
      BaseStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int v_Read = BaseStream.Read(buffer, offset, count);
      LogAsync(buffer, offset, v_Read, false).Wait();
      return v_Read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      return BaseStream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
      BaseStream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      Task v_Task = LogAsync(buffer, offset, count, true);
      BaseStream.Write(buffer, offset, count);
      v_Task.Wait();
    }

    private bool m_Written;

    private async Task LogAsync(byte[] p_Buffer, int offset, int p_Count, bool p_Sent)
    {
      if (await m_SemaphoreSlim.WaitAsync(15000))
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
        }
        finally
        {
          m_SemaphoreSlim.Release();
        }
      else
        throw new Exception("LogAsync could not write to raw file after waiting for 15 seconds!");
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