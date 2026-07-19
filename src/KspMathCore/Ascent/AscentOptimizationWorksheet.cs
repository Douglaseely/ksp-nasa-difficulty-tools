using System;

namespace KspMathCore.Ascent;

public static class AscentOptimizationWorksheet
{
    public static double ComputeIdealRocketDeltaV(double initialMassKg, double finalMassKg, double effectiveExhaustVelocity)
    {
        if (initialMassKg <= 0 || finalMassKg <= 0 || effectiveExhaustVelocity <= 0)
        {
            throw new ArgumentOutOfRangeException("mass/ve", "Mass and exhaust velocity must be positive.");
        }

        if (finalMassKg >= initialMassKg)
        {
            throw new ArgumentException("Final mass must be smaller than initial mass.", nameof(finalMassKg));
        }

        throw new NotImplementedException("Learning task: implement Tsiolkovsky equation.");
    }

    public static double ComputeInstantaneousTwr(double thrustNewton, double massKg, double localGravity)
    {
        if (thrustNewton < 0 || massKg <= 0 || localGravity <= 0)
        {
            throw new ArgumentOutOfRangeException("thrust/mass/g", "Inputs out of valid range.");
        }

        throw new NotImplementedException("Learning task: implement TWR = T / (m * g).");
    }

    public static double ComputeBurnTimeSeconds(double deltaV, double initialMassKg, double thrustNewton, double ispSeconds, double g0)
    {
        if (deltaV < 0 || initialMassKg <= 0 || thrustNewton <= 0 || ispSeconds <= 0 || g0 <= 0)
        {
            throw new ArgumentOutOfRangeException("inputs", "Inputs out of valid range.");
        }

        throw new NotImplementedException("Learning task: derive burn duration from mass flow and rocket equation.");
    }
}