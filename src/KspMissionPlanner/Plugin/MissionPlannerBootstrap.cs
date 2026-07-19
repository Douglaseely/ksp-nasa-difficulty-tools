using KspIntegration.Mission;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerBootstrap
{
    private readonly IKspGameContext _gameContext;
    private readonly IWaypointCatalog? _waypointCatalog;

    public MissionPlannerBootstrap(IKspGameContext gameContext, IWaypointCatalog? waypointCatalog = null)
    {
        _gameContext = gameContext;
        _waypointCatalog = waypointCatalog;
    }

    public MissionPlan BuildReversePlan(MissionObjective objective)
    {
        // This is the integration seam where KSP state is read and your math modules are called.
        _ = _gameContext.UniversalTimeSeconds;

        if (objective.ObjectiveType == MissionObjectiveType.SurfaceWaypoint &&
            objective.SurfaceTarget is not null &&
            _waypointCatalog is not null)
        {
            _waypointCatalog.TryGetWaypoint(
                objective.SurfaceTarget.BodyName,
                objective.SurfaceTarget.WaypointName,
                out _);
        }

        return new MissionPlan { Objective = objective };
    }
}