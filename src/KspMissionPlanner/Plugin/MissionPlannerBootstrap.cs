using KspMissionPlanner.Integrations;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerBootstrap
{
    private readonly IKspGameContext _gameContext;

    public MissionPlannerBootstrap(IKspGameContext gameContext)
    {
        _gameContext = gameContext;
    }

    public MissionPlan BuildReversePlan(MissionObjective objective)
    {
        // This is the integration seam where KSP state is read and your math modules are called.
        _ = _gameContext.UniversalTimeSeconds;
        return new MissionPlan { Objective = objective };
    }
}