using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDSession
{
   

    public class Zellmodul
    {
        public int Index;
        public int unknown;
        public int Temp1;
        public int Temp2;
        public Zelle[] Zelle; //6 Doppelzellen.
    }

    public struct Zelle
    {
        public string raw;
        public double Spannung;
        public int SoC;

    }


    
    public class Battery
    {
        public Zellmodul[] Modul;

        public List<double> GetVoltages()
        {
            List<double> voltages = new List<double>();
            foreach (var mod in Modul)
            {
                foreach (var zelle in mod.Zelle)
                {
                    voltages.Add(zelle.Spannung);
                }
            }
            return voltages;
        }

        void fiddle()
        {

            //   Module 1 bis 33: (33 = 0x70, alle anderen entsprechend dekrementieren)

            //Module_33_Information Read Data By Identifier
            //Req:
            //221870

            //Resp(header und 6 Doppelzellen)
            //62 18 70 51
            //9e 20 55 4d 30 42 2b
            //9e 20 55 72 67 79 43
            //9e 20 55 20 20 20 20
            //9e 16 55 4f 49 44 45
            //9e 16 55 00 00 00 00
            //9e 16 55 00 00 00 00

            //            7DF 03 22 18 5C
            //7ED 10 2E 62 18 5C 14 9A 10
            //7E5 30 00 00
            //7ED 21 4C 6D 09 00 00 9A 1A
            //7ED 22 4C 00 00 00 00 9A 1A
            //7ED 23 4C 00 00 00 00 9A 1A
            //7ED 24 4C 00 00 00 00 99 FC
            //7ED 25 4C 00 00 00 00 9A 10
            //7ED 26 4C 00 00 00 00 AA AA
            //7DF 03 22 18 5D
            //7ED 10 2E 62 18 5D 22 9A 1A
            //7E5 30 00 00
            //7ED 21 4C 6D 09 00 00 9A 1A
            //7ED 22 4C 00 00 00 00 9A 1A
            //7ED 23 4C 00 00 00 00 9A 24
            //7ED 24 4C 00 00 00 00 9A 10
            //7ED 25 4C 00 00 00 00 9A 10
            //7ED 26 4C 00 00 00 00 AA AA
            //7DF 03 22 18 5E
            //7ED 10 2E 62 18 5E 82 9A 2E
            //7E5 30 00 00
            //7ED 21 4C 6D 09 00 00 9A 1A
            //7ED 22 4C 00 00 00 00 9A 10
            //7ED 23 4C 00 00 00 00 9A 10
            //7ED 24 4C 00 00 00 00 9A 10
            //7ED 25 4C 00 00 00 00 9A 06
            //7ED 26 4C 00 00 00 00 AA AA

            // Hier mein altes Gefummel, sorry für den unübersichlichen Code:

            string resp = "";
            
            var did = resp.Substring(13, 2); // module index
            var trimm = (resp.Substring(16, resp.Length - 22));


            var result = trimm.Split(": ");
            var cleaned = result.Select(r => r.Substring(0, r.Length - 1));
            var all = String.Concat(cleaned);

            var mod = new Zellmodul();


            mod.Index = Convert.ToInt32(did, 16);


            mod.unknown = Convert.ToInt32(all.Substring(0, 2), 16);
            mod.Zelle = new Zelle[6];
            all = all.Substring(2, all.Length - 2).Replace(" ", "");

            for (int i = 0; i < 6; i++)
            {
                if (all.Length < 14 * 6)
                    continue;
                var rest = all.Substring(0 + (i * 14), 14);
                mod.Zelle[i].raw = rest;
                mod.Zelle[i].SoC = Convert.ToInt32(rest.Substring(4, 2), 16);
                mod.Zelle[i].Spannung = Convert.ToInt32(rest.Substring(0, 4), 16) / 10000.0;
            }

            
         

        }
    }
    
}
