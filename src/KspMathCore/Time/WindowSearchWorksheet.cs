using System;

namespace KspMathCore.Time;

public static class WindowSearchWorksheet
{
    public static double ComputeSynodicPeriodSeconds(double period1Seconds, double period2Seconds)
    {
        if (period1Seconds <= 0 || period2Seconds <= 0)
        {
            throw new ArgumentOutOfRangeException("periods", "Orbital periods must be positive.");
        }

        throw new NotImplementedException("Learning task: implement synodic period equation.");
    }
}