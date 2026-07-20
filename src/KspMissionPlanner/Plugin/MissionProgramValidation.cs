using System.Collections.Generic;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public static class MissionProgramValidation
{
    public static IReadOnlyList<string> Validate(MissionProgram missionProgram, bool requireAtLeastOneGoal)
    {
        var messages = new List<string>();
        if (missionProgram.Stages.Count == 0)
        {
            messages.Add("Mission must contain at least one stage.");
        }

        var hasAnyGoals = false;
        foreach (var stage in missionProgram.Stages)
        {
            if (stage.Goals.Count > 0)
            {
                hasAnyGoals = true;
            }

            foreach (var goal in stage.Goals)
            {
                if (string.IsNullOrWhiteSpace(goal.TargetBody))
                {
                    messages.Add($"Goal '{goal.Name}' must specify a target body.");
                }

                if ((goal.GoalType == MissionGoalType.Orbit || goal.GoalType == MissionGoalType.ParkingOrbit) && goal.OrbitTarget is null)
                {
                    messages.Add($"Orbit goal '{goal.Name}' must include orbit target data.");
                }

                if ((goal.GoalType == MissionGoalType.Landing || goal.GoalType == MissionGoalType.SurfaceWaypoint || goal.GoalType == MissionGoalType.SurfaceHop) && goal.SurfaceTarget is null)
                {
                    messages.Add($"Surface goal '{goal.Name}' must include surface target data.");
                }

                if (goal.GoalType == MissionGoalType.SurfaceWaypoint && goal.SurfaceTarget is not null && string.IsNullOrWhiteSpace(goal.SurfaceTarget.WaypointName))
                {
                    messages.Add($"Waypoint goal '{goal.Name}' must include a waypoint name.");
                }
            }
        }

        if (requireAtLeastOneGoal && !hasAnyGoals)
        {
            messages.Add("Mission must contain at least one goal.");
        }

        return messages;
    }

    public static IReadOnlyList<string> ValidateForSaving(MissionProgram missionProgram)
    {
        return Validate(missionProgram, requireAtLeastOneGoal: false);
    }

    public static IReadOnlyList<string> ValidateForPlanning(MissionProgram missionProgram)
    {
        return Validate(missionProgram, requireAtLeastOneGoal: true);
    }
}