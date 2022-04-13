using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaycanLogger
{
  public class FileRawLogger : IDisposable
  {
    private BinaryWriter m_BinaryWriter;
    private SemaphoreSlim m_SemaphoreSlim;

    public FileRawLogger(string p_DongleName)
    {
      m_SemaphoreSlim = new SemaphoreSlim(1);
      string v_DongleName = p_DongleName;
      foreach (var c in Path.GetInvalidFileNameChars())
        v_DongleName = v_DongleName.Replace(c, '-');

      var v_Stream = File.Open(Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, $"{v_DongleName}-{DateTime.Now:yyMMddHHmmss}.raw"), FileMode.Create);
      m_BinaryWriter = new BinaryWriter(v_Stream);
    }

    public async Task WriteAsync(byte[] p_Buffer, int p_Count, bool p_Sent)
    {
      await m_SemaphoreSlim.WaitAsync(2000);
      try
      {
        m_BinaryWriter.Write(p_Count * (p_Sent ? -1 : 1));
        m_BinaryWriter.Write(DateTime.Now.ToBinary());
        m_BinaryWriter.Write(p_Buffer, 0, p_Count);
        m_BinaryWriter.Flush();
      }
      finally
      {
        m_SemaphoreSlim.Release();
      }
    }

    public void Dispose()
    {
      m_BinaryWriter.Close();
    }
  }
}