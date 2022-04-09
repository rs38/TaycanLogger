using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaycanLogger
{
  public class OBDRawDevice : IDisposable, IOBDDevice
  {
    private FileRawStream m_FileRawStream;
    public string CurrentECUHeader { get; set; }
    public Func<string> Filename = null;

    public OBDRawDevice()
    {
    }

    public bool init(string _dongleName)
    {
      CurrentECUHeader = "7E5";
      if (Filename != null)
      {
        string v_Filename = Filename();
        if (!string.IsNullOrEmpty(v_Filename) && File.Exists(v_Filename))
        {
          //place the path you need to load a raw file.
          //ideally this would need to come from a file picker dialog, but that cannot go into here
          //need to add a function callback to get the right file...
          m_FileRawStream = new FileRawStream(v_Filename);
          return true;
        }
      }
      return false;
    }

    public async Task writeAllAsync(string[] array)
    {
      foreach (var str in array)
      {
        await WriteReadAsync(str);
      }
    }

    public async Task writeAsync(string str)
    {
      await m_FileRawStream.WriteAsync(Encoding.ASCII.GetBytes(str + '\r'), 0, str.Length + 1);
    }

    public async Task<string> WriteReadAsync(string str)
    {
      await writeAsync(str);
      return await readAsync();
    }

    private const int IO_BUFFER_SIZE = 400;

    private async Task<byte[]> ReadBufferFromStreamAsync(Stream stream)
    {
      var totalRead = 0;
      byte[] buffer = new byte[IO_BUFFER_SIZE];
      while (!buffer.Contains((byte)'>'))
      {
        var read = await stream.ReadAsync(buffer, totalRead, buffer.Length - totalRead);
        totalRead += read;
      }
      return buffer;
    }

    private async Task<string> readAsync()
          => Encoding.UTF8.GetString(await ReadBufferFromStreamAsync(m_FileRawStream));

    public void Dispose()
    {

    }
  }
}