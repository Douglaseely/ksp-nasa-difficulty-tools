using System;

namespace KspMathCore.Orbital;

public static class LambertWorksheet
{
    public static (double departureDeltaV, double arrivalDeltaV) SolvePatchedConicApproximation(
        double muPrimary,
        OrbitalElements departureOrbit,
        OrbitalElements arrivalOrbit,
        double timeOfFlightSeconds)
    {
        if (muPrimary <= 0 || timeOfFlightSeconds <= 0)
        {
            throw new ArgumentOutOfRangeException("muPrimary/timeOfFlightSeconds", "Inputs must be positive.");
        }

        if (departureOrbit is null || arrivalOrbit is null)
        {
            throw new ArgumentNullException(nameof(departureOrbit), "Orbits cannot be null.");
        }

        throw new NotImplementedException("Learning task: implement Lambert or an approximation pipeline.");
    }
}