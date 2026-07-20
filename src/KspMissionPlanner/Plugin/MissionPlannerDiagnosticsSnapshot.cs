namespace KspMissionPlanner.Plugin;

public sealed class MissionPlannerDiagnosticsSnapshot
{
    public string CurrentSaveName { get; set; } = string.Empty;
    public double UniversalTimeSeconds { get; set; }
    public string ActiveBodyName { get; set; } = string.Empty;
    public bool WaypointCatalogAvailable { get; set; }
    public bool ManeuverNodeExportAvailable { get; set; }
}