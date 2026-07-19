namespace KspAscentOptimizer.Integrations;

public interface IMechJebAdapter
{
    void SetAscentLimitQ(double maxDynamicPressure);
    void SetAscentMaxAcceleration(double maxAccelerationMetersPerSecondSquared);
    void EngageAscentAutopilot();
    bool SupportsSurfaceHopGuidance();
}

public interface IRealFuelsAdapter
{
    double GetCurrentStageMinimumThrottle01();
}

public interface IFarAdapter
{
    double EstimateDragLossesMetersPerSecond();
}