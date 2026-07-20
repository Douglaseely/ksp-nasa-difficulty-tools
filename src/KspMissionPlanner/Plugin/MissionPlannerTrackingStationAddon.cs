#if KSP_RUNTIME
namespace KspMissionPlanner.Plugin;

[KSPAddon(KSPAddon.Startup.TrackingStation, false)]
public sealed class MissionPlannerTrackingStationAddon : MissionPlannerSceneAddonBase
{
    protected override bool AllowNodeExport => false;
}
#endif