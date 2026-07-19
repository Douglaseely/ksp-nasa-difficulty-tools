using KspMathCore.Orbital;
using Xunit;

namespace KspMathCoreKhanTests;

public sealed class SurfaceHopKhanTests
{
    [Fact]
    [Trait("Category", "Khan")]
    public void GreatCircleDistance_KerbinBiomeHopChallenge()
    {
        var actual = SurfaceHopWorksheet.ComputeGreatCircleDistanceMeters(
            600000.0,
            DegreesToRadians(0.0),
            DegreesToRadians(0.0),
            DegreesToRadians(10.0),
            DegreesToRadians(35.0));

        AssertClose(379342.4911144282, actual, 1e-6);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void SurfaceHopTransferSemiMajorAxis_SuborbitalArcChallenge()
    {
        var actual = SurfaceHopWorksheet.ComputeTransferSemiMajorAxis(600000.0, 650000.0);
        AssertClose(625000.0, actual, 1e-9);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void EllipticArcTimeOfFlight_SuborbitalArcChallenge()
    {
        var actual = SurfaceHopWorksheet.ComputeEllipticArcTimeOfFlightSeconds(
            3.5316e12,
            625000.0,
            0.04,
            0.3,
            1.4);

        AssertClose(281.96306247079536, actual, 1e-6);
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * System.Math.PI / 180.0;
    }

    private static void AssertClose(double expected, double actual, double tolerance)
    {
        Assert.InRange(actual, expected - tolerance, expected + tolerance);
    }
}