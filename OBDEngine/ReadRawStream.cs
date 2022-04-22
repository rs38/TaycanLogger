using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OBDEngine
{
  public class ReadRawStream : Stream
  {
    private BinaryReader m_BinaryReader;
    private SemaphoreSlim m_SemaphoreSlim;
    private long m_TickWrite = long.MaxValue;
    private byte[] m_Buffer;
    private int m_Count;

    public ReadRawStream(string p_Filename)
    {
      m_Buffer = new byte[4096];
      m_SemaphoreSlim = new SemaphoreSlim(0);
      m_BinaryReader = new BinaryReader(File.OpenRead(p_Filename));
      m_BinaryReader.BaseStream.Position = 0;
      m_Count = m_BinaryReader.ReadInt32();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => m_BinaryReader.BaseStream.Length;

    public override long Position { get => m_BinaryReader.BaseStream.Position; set => m_BinaryReader.BaseStream.Position = value; }

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
              Task.Delay((int)((v_TickRead - m_TickWrite) / 50000)).Wait(); //should be 10000, but increase speed for dev work...
            m_BinaryReader.Read(buffer, offset, v_Count);
            m_Count = m_BinaryReader.ReadInt32();

            //200 bytes max length of raw data, increase if not enough...
            if (m_Count < -200 || m_Count > 200)
              throw new Exception($"Raw stream missmatch! count record: {m_Count}");
          }
          else
            m_Count = 0;
        }
        else
          throw new Exception($"Raw stream read write missmatch. Expected read, not write!");
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
      //System.Diagnostics.Debug.WriteLine(Encoding.ASCII.GetString(buffer, offset, v_Count));
      return v_Count;
    }

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

            //200 bytes max length of raw data, increase if not enough...
            if (m_Count < -200 || m_Count > 200)
              throw new Exception($"Raw stream missmatch! count record: {m_Count}");
          }
          else
            m_Count = 0;
        }
        else
          throw new Exception($"Raw stream read write missmatch. Expected write, not read!");
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
      // must be the same...
      string v_Sent = Encoding.ASCII.GetString(buffer, offset, count);
      string v_Original = Encoding.ASCII.GetString(m_Buffer, 0, v_Count);
      if (v_Sent.ToUpper() != v_Original.ToUpper())
        System.Diagnostics.Debug.WriteLine($"Raw stream missmatch! Sent: {v_Sent} Original: {v_Original}");
      if (v_Sent != v_Original)
        System.Diagnostics.Debug.WriteLine($"Raw stream missmatch! Sent: {v_Sent} Original: {v_Original}");
      //throw new Exception($"Raw stream missmatch! Sent: {v_Sent} Original: {v_Original}");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new System.NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new System.NotImplementedException();
    }

    public override void Close()
    {
      m_BinaryReader.Close();
    }
  }
}