namespace TaycanLogger
{
  internal class FormPagePowerCalc
  {
    private Action<double> DisplaySpeedValue;
    private Action<double> PlotterAmpereAddValue;
    private Action<double> PlotterConsumptionAddValue;
    private Action<double> PlotterPowerAddValue;
    private Action<double> PlotterVoltAddValue;
    private Action<double> PlotterSpeedSoCAddValueSpeed;
    private Action<double> PlotterSpeedSoCAddValueSoC;

    public FormPagePowerCalc(Action<double> p_DisplaySpeedValue, Action<double> p_PlotterAmpereAddValue, Action<double> p_PlotterConsumptionAddValue, Action<double> p_PlotterPowerAddValue, Action<double> p_PlotterVoltAddValue, Action<double> p_PlotterSpeedSoCAddValueSpeed, Action<double> p_PlotterSpeedSoCAddValueSoC)
    {
      DisplaySpeedValue = p_DisplaySpeedValue;
      PlotterAmpereAddValue = p_PlotterAmpereAddValue;
      PlotterConsumptionAddValue = p_PlotterConsumptionAddValue;
      PlotterPowerAddValue = p_PlotterPowerAddValue;
      PlotterVoltAddValue = p_PlotterVoltAddValue;
      PlotterSpeedSoCAddValueSpeed = p_PlotterSpeedSoCAddValueSpeed;
      PlotterSpeedSoCAddValueSoC = p_PlotterSpeedSoCAddValueSoC;
    }

    //below all yours! :)

    private double m_LastVoltageValue = double.NaN;
    private DateTime m_LastVoltageTime = DateTime.MaxValue;

    private double m_LastPowerValue = double.NaN;

    private double m_LastDistanceValue = double.NaN;

    private double m_LastSpeedValue = double.NaN;
    private DateTime m_LastSpeedTime = DateTime.MaxValue;

    internal void SessionValueExecuted(string p_Name, string p_Units, double p_Value)
    {
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "Amp")
      {
        PlotterAmpereAddValue(p_Value);

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
              */

        TimeSpan l_TimeSpan = (DateTime.Now - m_LastVoltageTime);
        var EnergykWh = m_LastPowerValue * l_TimeSpan.TotalSeconds / 3_600_000;
        var DistanceKm = m_LastSpeedValue * l_TimeSpan.TotalSeconds / 3600;
        var Consumption = 100 * EnergykWh / DistanceKm;

        if (Consumption < 100 & Consumption > -100)
          PlotterConsumptionAddValue(Consumption);

        if (!double.IsNaN(m_LastVoltageValue))
        {
          PlotterPowerAddValue(m_LastVoltageValue * -p_Value);
          m_LastPowerValue = m_LastVoltageValue * -p_Value;
        }
      }

      if (!string.IsNullOrEmpty(p_Name) && p_Name == "BatV")
      {
        PlotterVoltAddValue(p_Value);
        m_LastVoltageValue = p_Value;
        m_LastVoltageTime = DateTime.Now;
      }

      if (!string.IsNullOrEmpty(p_Name) && p_Name == "Speed")
      {
        DisplaySpeedValue(p_Value);
        PlotterSpeedSoCAddValueSpeed(p_Value);
        m_LastSpeedValue = p_Value;
        m_LastSpeedTime = DateTime.Now;

      }
      if (!string.IsNullOrEmpty(p_Name) && p_Name == "SoCDiplay")
      {
        PlotterSpeedSoCAddValueSoC(p_Value);
      }


    }

  }
}