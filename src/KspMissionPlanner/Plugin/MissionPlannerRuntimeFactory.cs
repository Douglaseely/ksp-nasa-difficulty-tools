#if KSP_RUNTIME
using System.IO;
using KspIntegration.Mission;
using KspMissionPlanner.Planning;

namespace KspMissionPlanner.Plugin;

public static class MissionPlannerRuntimeFactory
{
    public static MissionPlannerController CreateController(bool allowNodeExport)
    {
        var gameContext = new StockKspGameContext();
        var waypointCatalog = new StockWaypointCatalog();
        IManeuverNodeWriter? maneuverNodeWriter = allowNodeExport ? new StockManeuverNodeWriter() : null;
        var missionStore = new FileMissionProgramStore(BuildMissionStoreDirectory(gameContext.CurrentSaveName));
        var bootstrap = new MissionPlannerBootstrap(gameContext, waypointCatalog, missionStore: missionStore);
        return new MissionPlannerController(bootstrap, gameContext, waypointCatalog.IsAvailable, maneuverNodeWriter);
    }

    private static string BuildMissionStoreDirectory(string currentSaveName)
    {
        var saveName = string.IsNullOrWhiteSpace(currentSaveName) ? "default-save" : currentSaveName;
        return Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "KspMissionPlanner", "Missions", saveName);
    }
}
#endif