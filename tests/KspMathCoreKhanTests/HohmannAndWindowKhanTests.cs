using KspMathCore.Orbital;
using KspMathCore.Time;
using Xunit;

namespace KspMathCoreKhanTests;

public sealed class HohmannAndWindowKhanTests
{
    [Fact]
    [Trait("Category", "Khan")]
    public void CircularOrbitVelocity_KerbinLowOrbitChallenge()
    {
        var actual = HohmannTransferWorksheet.ComputeCircularOrbitVelocity(3.5316e12, 680000.0);
        AssertClose(2278.931638238564, actual, 1e-6);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void TransferSemiMajorAxis_KerbinOrbitRaiseChallenge()
    {
        var actual = HohmannTransferWorksheet.ComputeTransferSemiMajorAxis(680000.0, 1200000.0);
        AssertClose(940000.0, actual, 1e-9);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void HohmannDeltaV_KerbinOrbitRaiseChallenge()
    {
        var actual = HohmannTransferWorksheet.ComputeHohmannDeltaV(3.5316e12, 680000.0, 1200000.0);
        AssertClose(295.9542906782231, actual.deltaV1, 1e-6);
        AssertClose(256.4153882733161, actual.deltaV2, 1e-6);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void SynodicPeriod_InnerOuterOrbitChallenge()
    {
        var actual = WindowSearchWorksheet.ComputeSynodicPeriodSeconds(21600.0, 138984.38);
        AssertClose(25574.63444454876, actual, 1e-6);
    }

    private static void AssertClose(double expected, double actual, double tolerance)
    {
        Assert.InRange(actual, expected - tolerance, expected + tolerance);
    }
}