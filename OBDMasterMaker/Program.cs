using OBDEngine;
using System.IO.Compression;
using System.Xml.Linq;


class Program
{
  static void Main(string[] args)
  {
    //set the filename
    string v_NewMasterXml = @"obd2_Taycan_Version_1.xml"; //increase version by 1...
                                                          //create the master config
    XDocument v_XDocument = XDocument.Load(v_NewMasterXml);
    v_NewMasterXml = v_XDocument.ToString(SaveOptions.DisableFormatting);
    Console.WriteLine($"Converting new Master XML {v_NewMasterXml.Length} length.");
    Console.WriteLine($"{v_NewMasterXml.Substring(0, Math.Min(200, v_NewMasterXml.Length - 1))}...");
    Console.WriteLine();
    Console.WriteLine("Checking for errors and generating command IDs...");

    //check for errors in the xml config...
    CtlFileSetup v_CtlFileSetup = new CtlFileSetup(true);
    string? v_ErrorMessage = v_CtlFileSetup.CheckValidityAndEnrichWithmcid4ctl(v_XDocument);
    if (!string.IsNullOrEmpty(v_ErrorMessage))
    {
      Console.WriteLine("Cannot convert XML to master due to the following error:");
      Console.WriteLine(v_ErrorMessage);
      return;
    }

    using (var v_MemoryStream = new MemoryStream())
    {
      using (var v_BrotliStream = new BrotliStream(v_MemoryStream, CompressionLevel.Optimal))
        v_XDocument.Save(v_BrotliStream, SaveOptions.DisableFormatting);
      v_NewMasterXml = Convert.ToBase64String(v_MemoryStream.ToArray());
    }

    Console.WriteLine();
    Console.WriteLine($"New Master XML converted {v_NewMasterXml.Length} length.");
    Console.WriteLine($"{v_NewMasterXml.Substring(0, Math.Min(200, v_NewMasterXml.Length - 1))}...");

    System.Diagnostics.Debugger.Break();

    //copy in debug mode the new base64 master in v_NewMasterXml to the TaycanLogger project!
    //of course could write code to automaticall extend the switch statement int the code file in the project....
    //Open OBDEngine.MasterInfo the following code and add new version into switch statement...
    //public static string? GetMasterXml(short p_Version)
    //{
    //  return p_Version switch
    //  {
    //    0 => null, //null does not work with negative value when reading from the file
    //    1 => "i3wGIKyOt6FhGFHliZfmVeeyGnoPnoTd/e0XlZW2R2Nmyhd4LgO0MthH0IvFHKZPXn6B+Pz8tVrc5sEFHPyAW28jyhYsIi+aKug8um==",
    //    _ => throw new Exception($"CTL Version {p_Version} not supported!")
    //  };
    //}

    Console.ReadKey();
    return;
  }
}