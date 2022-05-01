using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;

namespace OBDEngine
{
  internal static class OBDMasterInfo
  {
    public static short GetLatestVersion()
    {
      //Anyway to get the latest version automatically without producing loads of exceptions?
      return 1;
    }

    public static string? GetMasterXml(short p_Version)
    {
      return p_Version switch
      {
        0 => null, //null does not work with negative value when reading from the file
        1 => "C9kFACwOeBPr4aT3ClvER25AiIOSZ+g9YXd/D9aZ+qQXsCzdpB4HoXNNOxsmkutLWR8tEJ/j91cb1zV/zFsIpUE4JSHUBZxULymkC8WP4dwDfAa0+sjLWxx7k9VV3dVqOm4kuokXlzsoXse7/hXF/80evwZY7Jbk4nGUGmNpIRercXlHwWNwlb9faLhNHtEp3Anu8CDYwpNgBZ/79wdOFYTasmVWAF/2C0BRFhCAeA2Xm/XiviRK9TmS+rXxj5FsCmCI9wgSoSe386NCqUrlDlcp9p1bT31VblRXteb+G3Oq6UILTwZ6D3teW7SjwaXxrz9+qdfTiScjLAd9uMlHI6efUh0ReCWg/WWQB7QGhOBhffaoIcRSCXSgc34CjDqMQO+7zfwaLyBXWyrnF+FAmTwf8x2deJ31lr9sczwbZIVwJjBged5pBJjqfBg42b6Cy2heAcjLHLQOW7cWZ2PjZ9mljf0sPcUgdoyZvtpYHYsnhBSLICIra+S+MDLKxprBcAeGJyPayTyo5A0SlEPgnDQhKg5hXYJWP1ZTuJ77/BH1+fiIZh+nTMK8B0Kqe3OW16mrDaVZM6jzk7Ap97OO64vc/QMg27lwdUPUxfktdBwnU1Lmrr9Sr71eMmAg+SNcw5Ami5/MuTE8lK8h5WFtdyvrCr8IkJS+GMXzzpL1zgFT4bw0TOtv0KhRJtKzcYbLFLtMkbws6DN+J14wNDJdT/HvS0tpyBIH/fCIaMTKVKy3BamZw2QlItmGdGnCSD/cAAM=",
        _ => throw new Exception($"CTL Version {p_Version} not supported!")
      };
    }

    public static XDocument GetMasterXDocument(short p_Version)
    {
      string? v_MasterXml = GetMasterXml(p_Version);
      if (string.IsNullOrEmpty(v_MasterXml))
        throw new Exception($"CTL Version {p_Version} not supported!");
      using (var v_MemoryStream = new MemoryStream(Convert.FromBase64String(v_MasterXml)))
      using (var v_BrotliStream = new BrotliStream(v_MemoryStream, CompressionMode.Decompress))
        return XDocument.Load(v_BrotliStream, LoadOptions.None);
    }

  }
}