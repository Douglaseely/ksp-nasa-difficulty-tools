using System;
using System.Collections.Generic;
using KspIntegration.Models;

namespace KspIntegration.Flight;

public sealed class NoOpRealFuelsAdapter : IRealFuelsAdapter
{
    public IReadOnlyList<StageSnapshot> CaptureStages()
    {
        return Array.Empty<StageSnapshot>();
    }

    public double GetCurrentStageMinimumThrottle01()
    {
        return 0.0;
    }
}