using System;

namespace KspMathCore.Orbital;

public static class SurfaceHopWorksheet
{
    public static double ComputeGreatCircleDistanceMeters(
        double bodyRadiusMeters,
        double departureLatitudeRadians,
        double departureLongitudeRadians,
        double arrivalLatitudeRadians,
        double arrivalLongitudeRadians)
    {
        if (bodyRadiusMeters <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bodyRadiusMeters), "Body radius must be positive.");
        }

        throw new NotImplementedException("Learning task: implement the spherical great-circle distance relation.");
    }

    public static double ComputeTransferSemiMajorAxis(double periapsisRadiusMeters, double apoapsisRadiusMeters)
    {
        if (periapsisRadiusMeters <= 0 || apoapsisRadiusMeters <= 0)
        {
            throw new ArgumentOutOfRangeException("radii", "Transfer radii must be positive.");
        }

        if (apoapsisRadiusMeters < periapsisRadiusMeters)
        {
            throw new ArgumentException("Apoapsis radius must be greater than or equal to periapsis radius.", nameof(apoapsisRadiusMeters));
        }

        throw new NotImplementedException("Learning task: implement a = (rp + ra) / 2.");
    }

    public static double ComputeEllipticArcTimeOfFlightSeconds(
        double gravitationalParameter,
        double semiMajorAxisMeters,
        double eccentricity,
        double departureEccentricAnomalyRadians,
        double arrivalEccentricAnomalyRadians)
    {
        if (gravitationalParameter <= 0 || semiMajorAxisMeters <= 0)
        {
            throw new ArgumentOutOfRangeException("mu/a", "Inputs must be positive.");
        }

        if (eccentricity < 0 || eccentricity >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(eccentricity), "Elliptic eccentricity must be in [0, 1).");
        }

        throw new NotImplementedException("Learning task: implement elliptic-arc time of flight from eccentric anomalies.");
    }
}