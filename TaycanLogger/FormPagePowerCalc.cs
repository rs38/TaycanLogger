namespace TaycanLogger
{
    internal class FormPagePowerCalc
    {
        private Action<double> DisplaySpeedValue;
        private Action<string, string> DataListDisplayLeft;
        private Action<string, string> DataListDisplayRight;
        private Action<double> PlotterAmpereAddValue;
        private Action<double> PlotterPowerAddValue;
        private Action<double> PlotterVoltAddValue;
        private Action<double> PlotterSpeedSoCAddValueSpeed;
        private Action<double> PlotterSpeedSoCAddValueSoC;
        Consumption consumption;

        private double m_LastVoltageValue = 0;
        private DateTime m_LastVoltageTime = DateTime.MaxValue;
        private double m_LastPowerValue = 0;
        private double m_LastDistanceValue = 0;
        private double m_LastSpeedValue = 0;
        private DateTime m_LastSpeedTime = DateTime.MaxValue;
        
        public FormPagePowerCalc(Action<double> p_DisplaySpeedValue, Action<string, string> p_DataListDisplayLeft, Action<string, string> p_DataListDisplayRight, Action<double> p_PlotterAmpereAddValue, Action<double> p_PlotterConsumptionAddValue, Action<double> p_PlotterPowerAddValue, Action<double> p_PlotterVoltAddValue, Action<double> p_PlotterSpeedSoCAddValueSpeed, Action<double> p_PlotterSpeedSoCAddValueSoC)
        {
            DisplaySpeedValue = p_DisplaySpeedValue;
            DataListDisplayLeft = p_DataListDisplayLeft;
            DataListDisplayRight = p_DataListDisplayRight;
            PlotterAmpereAddValue = p_PlotterAmpereAddValue;
            PlotterPowerAddValue = p_PlotterPowerAddValue;
            PlotterVoltAddValue = p_PlotterVoltAddValue;
            PlotterSpeedSoCAddValueSpeed = p_PlotterSpeedSoCAddValueSpeed;
            PlotterSpeedSoCAddValueSoC = p_PlotterSpeedSoCAddValueSoC;
            consumption = new Consumption(p_PlotterConsumptionAddValue);
        }

        internal void SessionValueExecuted(string p_Name, string p_Units, double p_Value)
        {
            if (string.IsNullOrEmpty(p_Name))
                return;

            if (double.IsNaN(p_Value))
                return;

            if (p_Name == "Amp")
            {
                PlotterAmpereAddValue(p_Value);

                UpdateConsumptionPlotter();

                if (!double.IsNaN(m_LastVoltageValue))
                {
                    PlotterPowerAddValue(m_LastVoltageValue * -p_Value);
                    m_LastPowerValue = m_LastVoltageValue * -p_Value;
                }
            }

            if (p_Name == "BatV")
            {
                PlotterVoltAddValue(p_Value);
                m_LastVoltageValue = p_Value;
                m_LastVoltageTime = DateTime.Now;
            }

            if (p_Name == "Speed")
            {
                DisplaySpeedValue(p_Value);
                PlotterSpeedSoCAddValueSpeed(p_Value);
                m_LastSpeedValue = p_Value;
                m_LastSpeedTime = DateTime.Now;

            }
            if (p_Name == "SoCDiplay")
            {
                PlotterSpeedSoCAddValueSoC(p_Value);
            }

            if (p_Name == "SoCDiplay")
            {
                PlotterSpeedSoCAddValueSoC(p_Value);
            }

            if (p_Name == "Speed")
                DataListDisplayLeft("Speed", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "SoCDiplay")
                DataListDisplayLeft("SoCDiplay", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "AirT")
                DataListDisplayLeft("AirT", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "InsideT")
                DataListDisplayLeft("InsideT", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "CompW")
                DataListDisplayLeft("CompW", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "HeatA")
                DataListDisplayLeft("HeatA", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "BatTOut")
                DataListDisplayRight("BatTOut", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "BatTIn")
                DataListDisplayRight("BatTIn", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "BatTemp")
                DataListDisplayRight("BatTemp", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "CelSum")
                DataListDisplayRight("CelSum", $"{Math.Round(p_Value, 1)} {p_Units}");
            if (p_Name == "BatLimC")
                DataListDisplayRight("BatLimC", $"{Math.Round(p_Value, 1)} {p_Units}");
            //if (p_Name == "BatLimD")
            //  DataListDisplayRight("BatLimD", $"{Math.Round(p_Value, 1)} {p_Units}");

        }

        private void UpdateConsumptionPlotter()
        {
            /*     Power, Current and Speed usually have slightly different timestamps from OBD but time span between reads should be similar if skipcount is same.
                   I alread tried to read amp,volt AND velocity at once but this was a bit of unrealiable:

                     <command send="2218021801F40D" header="atsh7e5" skipCount="0">
                        <values>
                          <value name="Amp" conversion="((B3*65536.0)+B4*256+B5)/100-1500" units="A"/>
                          <value name="BatV" conversion="((B8*256)+B9)/10" units="V"/>
                          <value name="Speed"  conversion="B12" units="km/h"/>-->
                        </values>
                      </command>

                  we should give it a retry, result will be cleaner and more precise! 

                  Math:

                  TimeSpan = (now - LastVoltageTime) (in seconds)

                  Energy = LastPower * TimeSpan (in Ws = J)
                  EnergyKWh = Energy / 3_600_000

                  Distance = Speed * TimeSpan (km/h * s)
                  DistanceKm = Distance / 3600

                  Consumption = 100 * EnergyKWh / DistanceKm (kWh/100km)

                  --> divide by zero if not moving but consuming energy. how to track that? 
                        how to deal with infinite numbers? 
                    Clipping in Plotter

                  */

            TimeSpan l_TimeSpan = (DateTime.Now - m_LastVoltageTime);
            var EnergykWh = m_LastPowerValue * l_TimeSpan.TotalSeconds / 3_600_000;
            var DistanceKm = m_LastSpeedValue * l_TimeSpan.TotalSeconds / 3600;
            var Consumption = 100 * EnergykWh / DistanceKm;

            consumption.Add(Consumption);
        }

    }

    internal class Consumption
    {
        private List<double> consumptions = new List<double>();
        private int windowSize = 40;
        private int skipCountUpdate = 5;
        int sampleCount = 0;
        Action<double> plotterDraw;

        internal Consumption( Action<double> plot )
        {
           plotterDraw  = plot;
        }

        public void Add(double value)
        {
            consumptions.Add(value);
            sampleCount++;

            //ProcessPlotterValues();
            Plot();
        }

        private void Plot()
        {
          if (sampleCount % skipCountUpdate == 0)   
            PlotIfValid(consumptions.TakeLast(windowSize).Average());
        }

        private void PlotIfValid(double consumption)
        {
            if (double.IsNaN(consumption))
                return;
            if (consumption < -100 | double.IsNegativeInfinity(consumption))
                plotterDraw(-100.0);
            if (consumption > 100.0)
                plotterDraw(100);
            else
                plotterDraw(consumption);
        }

        void ProcessPlotterValues()
        {
           
        }
    }
}