#if KSP_RUNTIME
using UnityEngine;

namespace KspAscentOptimizer.Plugin;

public sealed class AscentOptimizerWindowState
{
    public string StatusMessage { get; set; } = string.Empty;
    public string TargetOrbitAltitudeText { get; set; } = "100000";
    public Vector2 ScrollPosition { get; set; }
}
#endif