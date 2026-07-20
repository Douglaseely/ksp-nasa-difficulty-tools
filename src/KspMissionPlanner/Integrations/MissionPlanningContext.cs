using KspIntegration.Models;

namespace KspMissionPlanner.Integrations;

public sealed class MissionPlanningContext
{
    public string CurrentSaveName { get; set; } = string.Empty;
    public double UniversalTimeSeconds { get; set; }
    public GameStateSnapshot GameState { get; set; } = new GameStateSnapshot();
}