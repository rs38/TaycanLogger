using System.IO.Compression;
using System.Xml.Linq;

namespace OBDEngine
{
  internal class CtlBinaryWriter : IDisposable
  {
    private BinaryWriter m_BinaryWriterFile;
    private BinaryWriter m_BinaryWriter;
    private bool m_Open;
    private long m_TotalCount = 0;
    private bool disposedValue;

    internal CtlBinaryWriter(string p_DeviceName, XDocument? p_XUserXML = null)
    {
      string v_DongleName = p_DeviceName;
      foreach (var c in Path.GetInvalidFileNameChars())
        v_DongleName = v_DongleName.Replace(c, '-');
      m_BinaryWriterFile = new BinaryWriter(File.Open(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{v_DongleName}-{DateTime.Now:yyyyMMdd-HHmmss}.ctl"), FileMode.Create));
      short v_Version = OBDMasterInfo.GetLatestVersion();
      if (p_XUserXML is not null)
        v_Version *= -1;
      m_BinaryWriterFile.Write(v_Version);
      m_BinaryWriterFile.Write((long)m_TotalCount);
      m_BinaryWriter = new BinaryWriter(new BrotliStream(m_BinaryWriterFile.BaseStream, CompressionMode.Compress, true));
      m_Open = true;
      if (p_XUserXML is not null)
      {
        using (MemoryStream v_MemoryStream = new MemoryStream())
        {
          p_XUserXML.Save(v_MemoryStream, SaveOptions.DisableFormatting);
          m_BinaryWriter.Write((int)v_MemoryStream.Length);
          m_BinaryWriter.Write(v_MemoryStream.GetBuffer(), 0, (int)v_MemoryStream.Length);
        }
      }
    }

    internal void Write(int p_Mcid4ctl, byte[] buffer, int offset, int count)
    {
      //we want to use System.Threading.Channels.Channel
      //so we can do the writing with occacional flush in another thread...
      m_BinaryWriter.Write(p_Mcid4ctl);
      m_BinaryWriter.Write(DateTime.Now.ToBinary());
      m_BinaryWriter.Write(count - offset);
      m_BinaryWriter.Write(buffer, offset, count);
      m_TotalCount++;
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

        //still not sure how to implement this, that the streams are closed and length is written to
        //prevent total data loss. I cannot debug this, just run and test and guess what's happening.
        try
        {
          m_BinaryWriter.Flush();
          m_BinaryWriter.Dispose();
          m_BinaryWriterFile.BaseStream.Position = 2;
          m_BinaryWriterFile.Write(m_TotalCount);
          m_BinaryWriterFile.Dispose();
        }
        catch (ObjectDisposedException) { }
        disposedValue = true;
      }
    }

    // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    ~CtlBinaryWriter()
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