using System.Collections.Generic;
using KspIntegration.Models;

namespace KspIntegration.Flight;

public interface IMechJebAdapter
{
    MechJebAscentSettingsSnapshot CaptureAscentSettings();
    void SetAscentLimitQ(double maxDynamicPressure);
    void SetAscentMaxAcceleration(double maxAccelerationMetersPerSecondSquared);
    void EngageAscentAutopilot();
    bool SupportsSurfaceHopGuidance();
}

public interface IRealFuelsAdapter
{
    IReadOnlyList<StageSnapshot> CaptureStages();
    double GetCurrentStageMinimumThrottle01();
}

public interface IFarAdapter
{
    AerodynamicSnapshot CaptureAerodynamics();
    double EstimateDragLossesMetersPerSecond();
}