using KspIntegration.Models;

namespace KspIntegration.Flight;

public sealed class NoOpFarAdapter : IFarAdapter
{
    public AerodynamicSnapshot CaptureAerodynamics()
    {
        return new AerodynamicSnapshot();
    }

    public double EstimateDragLossesMetersPerSecond()
    {
        return 0.0;
    }
}