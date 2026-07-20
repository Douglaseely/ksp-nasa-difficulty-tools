namespace KspMissionPlanner.Plugin;

using KspMissionPlanner.Planning;

public static class MissionPlannerSessionState
{
    public static string ActiveMissionId { get; set; } = string.Empty;
    public static MissionProgram? WorkingMissionProgram { get; set; }
}