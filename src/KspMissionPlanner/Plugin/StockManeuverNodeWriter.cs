#if KSP_RUNTIME
using System;
using KspIntegration.Mission;
using KspIntegration.Models;
using UnityEngine;

namespace KspMissionPlanner.Plugin;

public sealed class StockManeuverNodeWriter : IManeuverNodeWriter
{
    public void AddNode(ManeuverNodeRequest nodeRequest)
    {
        if (nodeRequest is null)
        {
            throw new ArgumentNullException(nameof(nodeRequest));
        }

        var vessel = FlightGlobals.ActiveVessel;
        if (vessel?.patchedConicSolver is null)
        {
            throw new InvalidOperationException("No active vessel with a patched conic solver is available.");
        }

        var node = vessel.patchedConicSolver.AddManeuverNode(nodeRequest.UniversalTimeSeconds);
        if (node is null)
        {
            throw new InvalidOperationException("Failed to create maneuver node.");
        }

        node.DeltaV = new Vector3d(
            nodeRequest.RadialMetersPerSecond,
            nodeRequest.NormalMetersPerSecond,
            nodeRequest.ProgradeMetersPerSecond);
    }
}
#endif