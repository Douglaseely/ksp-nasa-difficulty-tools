#if KSP_RUNTIME
using UnityEngine;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerWindowState
{
    public string ExpandedBodyGoalId { get; set; } = string.Empty;
    public string NewMissionName { get; set; } = "New Mission";
    public string StatusMessage { get; set; } = string.Empty;
    public Vector2 MissionScrollPosition { get; set; }
    public Vector2 EditorScrollPosition { get; set; }
    public Vector2 PlanningScrollPosition { get; set; }
    public string ParkingPeriapsisText { get; set; } = "15000";
    public string ParkingApoapsisText { get; set; } = "18000";
    public string ParkingInclinationText { get; set; } = "0";
}
#endif