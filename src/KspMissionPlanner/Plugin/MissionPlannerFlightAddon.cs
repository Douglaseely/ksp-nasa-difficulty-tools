#if KSP_RUNTIME
namespace KspMissionPlanner.Plugin;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public sealed class MissionPlannerFlightAddon : MissionPlannerSceneAddonBase
{
}
#endif