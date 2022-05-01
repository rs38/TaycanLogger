using System.IO.Compression;
using System.Xml.Linq;

namespace OBDEngine
{
  internal class CtlBinaryReader : IDisposable
  {
    private long m_TotalCount = 0;
    private bool m_HasUserConfig;
    private BinaryReader m_BinaryReader;
    private long m_TickWrite = long.MaxValue;
    private bool disposedValue;

    public short Version { get; init; }

    public long TotalCount { get; init; }

    public CtlBinaryReader(string p_Filename)
    {
      m_BinaryReader = new BinaryReader(File.OpenRead(p_Filename), System.Text.Encoding.UTF8, true);
      short v_Version = m_BinaryReader.ReadInt16();
      m_TotalCount = m_BinaryReader.ReadInt64();
      m_BinaryReader = new BinaryReader(new BrotliStream(m_BinaryReader.BaseStream, CompressionMode.Decompress));
      m_HasUserConfig = v_Version < 0;
      Version = Math.Abs(v_Version);
      TotalCount = m_TotalCount;
    }

    public XDocument? LoadUserXml()
    {
      XDocument? v_XDocument = null;
      if (m_HasUserConfig)
      {
        int v_Length = m_BinaryReader.ReadInt32();
        byte[] v_Buffer = new byte[v_Length];
        int v_BytesRead = m_BinaryReader.Read(v_Buffer, 0, v_Buffer.Length);
        using (MemoryStream v_MemoryStream = new MemoryStream(v_Buffer))
          v_XDocument = XDocument.Load(v_MemoryStream); //always loads to the end of the stream...
      }
      return v_XDocument;
    }

    public (int Mcid4ctl, int Count) Read(byte[] buffer, int count)
    {
      if (m_TotalCount-- > 0)
      {
        int v_Mcid4ctl = m_BinaryReader.ReadInt32();
        long v_TickRead = m_BinaryReader.ReadInt64();
        if (m_TickWrite < v_TickRead)
          Task.Delay((int)((v_TickRead - m_TickWrite) / 10000)).Wait(); //should be 10000, but increase speed for dev work. This needs to be a settings value from UI...
        m_TickWrite = v_TickRead;
        int v_Count = m_BinaryReader.ReadInt32();
        m_BinaryReader.Read(buffer, 0, v_Count);
        return (v_Mcid4ctl, v_Count);
      }
      return (0, 0);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects)
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        try
        {
          m_BinaryReader.Dispose();
        }
        catch (ObjectDisposedException) { }
        disposedValue = true;
      }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~CtlBinaryReader()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: false);
    }

    public void Dispose()
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }
  }
}
