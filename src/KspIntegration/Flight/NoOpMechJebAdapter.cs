using KspIntegration.Models;

namespace KspIntegration.Flight;

public sealed class NoOpMechJebAdapter : IMechJebAdapter
{
    public MechJebAscentSettingsSnapshot CaptureAscentSettings()
    {
        return new MechJebAscentSettingsSnapshot();
    }

    public void SetAscentLimitQ(double maxDynamicPressure)
    {
        _ = maxDynamicPressure;
    }

    public void SetAscentMaxAcceleration(double maxAccelerationMetersPerSecondSquared)
    {
        _ = maxAccelerationMetersPerSecondSquared;
    }

    public void EngageAscentAutopilot()
    {
    }

    public bool SupportsSurfaceHopGuidance()
    {
        return false;
    }
}