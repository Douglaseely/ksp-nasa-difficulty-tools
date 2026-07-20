using System;
using System.Collections.Generic;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerViewState
{
    public MissionPlannerTab SelectedTab { get; set; } = MissionPlannerTab.Missions;
    public string SelectedMissionId { get; set; } = string.Empty;
    public string ActiveMissionId { get; set; } = string.Empty;
    public bool IsDirty { get; set; }
    public MissionProgram? CurrentMissionProgram { get; set; }
    public MissionPlan? CurrentPlan { get; set; }
    public IReadOnlyList<string> ValidationMessages { get; set; } = Array.Empty<string>();
    public MissionPlannerDiagnosticsSnapshot Diagnostics { get; set; } = new MissionPlannerDiagnosticsSnapshot();
}