using System;

namespace KspMathCore.Orbital;

public static class HohmannTransferWorksheet
{
    public static double ComputeCircularOrbitVelocity(double mu, double radius)
    {
        if (mu <= 0 || radius <= 0)
        {
            throw new ArgumentOutOfRangeException("mu/radius", "mu and radius must be positive.");
        }

        throw new NotImplementedException("Learning task: implement v = sqrt(mu / r).");
    }

    public static double ComputeTransferSemiMajorAxis(double r1, double r2)
    {
        if (r1 <= 0 || r2 <= 0)
        {
            throw new ArgumentOutOfRangeException("r1/r2", "Orbit radii must be positive.");
        }

        throw new NotImplementedException("Learning task: implement a_t = (r1 + r2) / 2.");
    }

    public static (double deltaV1, double deltaV2) ComputeHohmannDeltaV(double mu, double r1, double r2)
    {
        if (mu <= 0 || r1 <= 0 || r2 <= 0)
        {
            throw new ArgumentOutOfRangeException("mu/r1/r2", "mu and orbit radii must be positive.");
        }

        throw new NotImplementedException("Learning task: implement two-impulse Hohmann transfer delta-v equations.");
    }
}