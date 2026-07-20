using System;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Integrations;

public sealed class WorksheetMissionMathService : IMissionMathService
{
    public DirectTransferEstimate EstimateTransfer(MissionGoal fromGoal, MissionGoal toGoal, MissionPlanningContext context)
    {
        if (fromGoal is null)
        {
            throw new ArgumentNullException(nameof(fromGoal));
        }

        if (toGoal is null)
        {
            throw new ArgumentNullException(nameof(toGoal));
        }

        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        return toGoal.GoalType switch
        {
            MissionGoalType.Orbit => EstimateOrbitalTransfer(fromGoal, toGoal, context),
            MissionGoalType.ParkingOrbit => EstimateOrbitalTransfer(fromGoal, toGoal, context),
            MissionGoalType.Flyby => EstimateOrbitalTransfer(fromGoal, toGoal, context),
            MissionGoalType.Landing => EstimateTerminalApproach(fromGoal, toGoal, context),
            MissionGoalType.SurfaceWaypoint => EstimateTerminalApproach(fromGoal, toGoal, context),
            MissionGoalType.SurfaceHop => EstimateSurfaceHop(fromGoal, toGoal, context),
            _ => throw new NotSupportedException($"No mission-math route is defined for goal type '{toGoal.GoalType}'."),
        };
    }

    public DirectTransferEstimate EstimateOrbitalTransfer(MissionGoal fromGoal, MissionGoal toGoal, MissionPlanningContext context)
    {
        _ = fromGoal;
        _ = toGoal;
        _ = context;
        throw new NotImplementedException("Implement orbital transfer estimation with HohmannTransferWorksheet, WindowSearchWorksheet, and later LambertWorksheet.");
    }

    public DirectTransferEstimate EstimateTerminalApproach(MissionGoal fromGoal, MissionGoal toGoal, MissionPlanningContext context)
    {
        _ = fromGoal;
        _ = toGoal;
        _ = context;
        throw new NotImplementedException("Implement terminal approach and landing estimation for planner legs.");
    }

    public DirectTransferEstimate EstimateSurfaceHop(MissionGoal fromGoal, MissionGoal toGoal, MissionPlanningContext context)
    {
        _ = fromGoal;
        _ = toGoal;
        _ = context;
        throw new NotImplementedException("Implement same-body waypoint hop estimation with SurfaceHopWorksheet.");
    }
}