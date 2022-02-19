using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaycanLogger
{
   
        //todo
        //add SoH
        // "broadcast" ??
        // parse multi-frame data stolpert dabei
        // FileLogger auslagern
        // 0x2A continous
        // further ideas: reset ECUs, aktivate passenger screen

        /*
                	
                     { "181C","Cool Inlet"},
                     { "181D","Cool Outlet"},
                     { "1E10","T Batt Ave"},
                   { "1E2D","SocCell min"},
                     { "1E2C","SocCell max"},
                     { "08D2","SoC Anzeige"},
                     {"475F", "PTC current"},
                     {"2608","Outside Temp"},
                     {"2613","Inside Temp"},

                	{"27D6","Kompressor kW"},
                     {"0530","Dashboard"},
                { "1E2D2","SocCell min #"},
                     { "1E2C2","SocCell max #"},
                 { "192A","Reqrmt_BMS_CoolLvl"},
                     { "192C","StateBattCoolPump"}, 
                     { "192B","CoolPumpPwrSetPoint"}

         Strecke Read Data By Identifier ;22 05 20;10547
                 RSP0002;19:37:08.822;LL_DashBoardUDS BV_DashBoardUDS EV_DashBoardLGEPO513_004;Strecke Read Data By Identifier ;62 05 20 00 37 b1 00 a6 4f 00;10547

                 714 03 22 05 30 
                 77E 
                 RSP0002;19:35:18.655;LL_DashBoardUDS BV_DashBoardUDS EV_DashBoardLGEPO513_004;Berechnete, angezeigte Werte Read Data By Identifier ;22 05 30;9982
                 62 05 30 00 22 c1 24 ce 25 a1 80 00 01 dd d0 01 80 00 00 00 05 32 78 1b d8 80 00 46 00 18 00 00 00 00 01 60 23 82 7f 84 c9 a6 00 dc 00 00 00 00 3d b8 01 5d 4c 45 44 38 59 52 4c 45 00 00 00;9981
                */
      
   
        
        //("22F19E"); //VariantIdentificationAndSelection
        

       
            //write("22 F446"); //"Außentemperatur"; ID: F446 x-40
            //write("22 0432"); //"Historiendaten 35 Charge_Power_Limit_Time_TEMP ID: 0432 32bit werte x/10 in Sec.

            //write("22 1E32"); // Total accumulated charged and discharge
            //	charger_coolant_temperature "15E2" Value: 45 
            //17 FE 00 8B Gateway?

        

        /*
                {0, "ATE0"},	// Echo off
                 "ATAT0"}, //	Adaptive timing off
                { 0, "ATSTFF"}, //	Set timeout to ff x 4ms
                { 0, "ATAL"}, //	Allow long messages ( > 7 Bytes)
                { 0, "ATH1"}, //	Additional headers on
                { 0, "ATS0"}, //	Printing of spaces off
                { 0, "ATL0"}, //	Linefeeds off
                { 0, "ATCSM0"}, //	Silent monitoring off*/

    }

   


