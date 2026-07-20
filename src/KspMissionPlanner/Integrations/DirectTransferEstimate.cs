using System;
using System.Collections.Generic;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Integrations;

public sealed class DirectTransferEstimate
{
    public double DeltaVMetersPerSecond { get; set; }
    public double FlightTimeSeconds { get; set; }
    public IReadOnlyList<PlannedManeuver> Maneuvers { get; set; } = Array.Empty<PlannedManeuver>();
    public string Summary { get; set; } = string.Empty;
}