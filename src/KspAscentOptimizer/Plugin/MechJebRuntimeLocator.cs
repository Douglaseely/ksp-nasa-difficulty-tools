#if KSP_RUNTIME
using System.Linq;
using KspIntegration.Flight;

namespace KspAscentOptimizer.Plugin;

public static class MechJebRuntimeLocator
{
    public static IMechJebAdapter? TryCreateAdapter()
    {
        var vessel = FlightGlobals.ActiveVessel;
        if (vessel is null)
        {
            return null;
        }

        foreach (var part in vessel.parts)
        {
            var mechJebModule = part.Modules
                .Cast<PartModule>()
                .FirstOrDefault(module =>
                    module.GetType().Name.Contains("MechJebCore") ||
                    (module.GetType().FullName?.Contains("MechJeb") ?? false));

            if (mechJebModule is not null)
            {
                return new ReflectionMechJebAdapter(mechJebModule);
            }
        }

        return null;
    }
}
#endif