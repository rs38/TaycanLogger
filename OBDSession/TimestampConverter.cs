using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDSession
{
    public class TimestampConverter
    {
        public static DateTime ConvertToDateTime(string timestamp)
        {
            UInt32 bytes = Convert.ToUInt32(timestamp, 16);
            byte[] byteArray = BitConverter.GetBytes(bytes);

            var year = 2000 + (byteArray[3] >> 2);
            var month = (BitConverter.ToUInt16(byteArray,2) & 0x03C0) >> 6;
            var day = (byteArray[2] & 0x3E) >> 1;
            var hour = (BitConverter.ToUInt16(byteArray, 1) & 0x01F0) >> 4;
            var min = (BitConverter.ToUInt16(byteArray, 0) & 0x0FC0) >> 6;
            var sec = byteArray[0] & 0x3F;

            DateTime dt = new DateTime(year, month, day, hour, min, sec);
            return dt;
        }
    }
}


//Umgebungsbedingungen

//Jahr, Monat, Tag, Stunde, Min, sek
//; Byte: 0; Bit: 2; Len: 7; : 2  0xFC000000
//; Byte: 1; Bit: 6; Len: 4; : 14 0X03C00000
//; Byte: 1; Bit: 1; Len: 5; : 9  0x003E0000

//; Byte: 2; Bit: 4; Len: 5; : 20 0x0001F000
//; Byte: 3; Bit: 6; Len: 6; : 30 0x00000FC0
//; Byte: 4; Bit: 0; Len: 6; : 24 0x0000003F
