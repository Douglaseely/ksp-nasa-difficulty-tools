#if KSP_RUNTIME
namespace KspMissionPlanner.Plugin;

[KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
public sealed class MissionPlannerSpaceCenterAddon : MissionPlannerSceneAddonBase
{
    protected override bool AllowNodeExport => false;
}
#endif