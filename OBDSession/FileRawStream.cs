using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaycanLogger
{
  public class FileRawStream : Stream
  {
    private BinaryReader m_BinaryReader;
    private SemaphoreSlim m_SemaphoreSlim;
    private long m_TickWrite = long.MaxValue;

    public FileRawStream(string p_Filename)
    {
      m_SemaphoreSlim = new SemaphoreSlim(0);
      m_BinaryReader = new BinaryReader(File.OpenRead(p_Filename));
      m_BinaryReader.BaseStream.Position = 0;
      m_Count = m_BinaryReader.ReadInt32();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => 0;

    public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      int v_Count = 0;
      m_SemaphoreSlim.Wait(2000);
      try
      {
        if (m_Count > 0)
        {
          v_Count = m_Count;
          if (m_BinaryReader.BaseStream.Position <= m_BinaryReader.BaseStream.Length - 12 - v_Count)
          {
            long v_TickRead = m_BinaryReader.ReadInt64();
            if (m_TickWrite < v_TickRead)
              Task.Delay((int)((v_TickRead - m_TickWrite) / 10000)).Wait();
            m_BinaryReader.Read(buffer, offset, v_Count);
            m_Count = m_BinaryReader.ReadInt32();
          }
          else
            m_Count = 0;
        }
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
      //System.Diagnostics.Debug.WriteLine(Encoding.ASCII.GetString(buffer, offset, v_Count));
      return v_Count;
    }

    private int m_Count = 0;
    private byte[] m_Buffer = new byte[4096];

    public override void Write(byte[] buffer, int offset, int count)
    {
      int v_Count = 0;
      m_SemaphoreSlim.Wait(2000);
      try
      {
        if (m_Count < 0)
        {
          v_Count = m_Count * -1;
          if (m_BinaryReader.BaseStream.Position <= m_BinaryReader.BaseStream.Length - 12 - v_Count)
          {
            m_TickWrite = m_BinaryReader.ReadInt64();
            m_BinaryReader.Read(m_Buffer, 0, v_Count);
            m_Count = 0;
            m_Count = m_BinaryReader.ReadInt32();
          }
          else
            m_Count = 0;
        }
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
      // must be the same...
      //  System.Diagnostics.Debug.WriteLine(Encoding.ASCII.GetString(buffer, offset, count));
      //  System.Diagnostics.Debug.WriteLine(Encoding.ASCII.GetString(m_Buffer, 0, v_Count));
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new System.NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new System.NotImplementedException();
    }
  }
}