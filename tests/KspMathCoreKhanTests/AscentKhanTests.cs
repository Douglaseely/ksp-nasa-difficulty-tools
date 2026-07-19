using KspMathCore.Ascent;
using Xunit;

namespace KspMathCoreKhanTests;

public sealed class AscentKhanTests
{
    [Fact]
    [Trait("Category", "Khan")]
    public void IdealRocketDeltaV_TransferStageChallenge()
    {
        var actual = AscentOptimizationWorksheet.ComputeIdealRocketDeltaV(12000.0, 5000.0, 310.0 * 9.80665);
        AssertClose(2661.4788028832027, actual, 1e-6);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void InstantaneousTwr_BoosterStageChallenge()
    {
        var actual = AscentOptimizationWorksheet.ComputeInstantaneousTwr(200000.0, 12000.0, 9.81);
        AssertClose(1.6989466530750934, actual, 1e-9);
    }

    [Fact]
    [Trait("Category", "Khan")]
    public void BurnTime_FiniteBurnChallenge()
    {
        var actual = AscentOptimizationWorksheet.ComputeBurnTimeSeconds(950.0, 12000.0, 200000.0, 310.0, 9.80665);
        AssertClose(48.9534559715229, actual, 1e-6);
    }

    private static void AssertClose(double expected, double actual, double tolerance)
    {
        Assert.InRange(actual, expected - tolerance, expected + tolerance);
    }
}