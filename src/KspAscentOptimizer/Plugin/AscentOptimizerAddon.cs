#if KSP_RUNTIME
using KSP.UI.Screens;
using UnityEngine;

namespace KspAscentOptimizer.Plugin;

[KSPAddon(KSPAddon.Startup.Flight, false)]
public class AscentOptimizerAddon : MonoBehaviour
{
    private static bool _instanceActive;

    private ApplicationLauncherButton? _toolbarButton;
    private ToolbarControllerBridge? _toolbarControllerBridge;
    private Rect _windowRect = new Rect(240f, 140f, 540f, 420f);
    private bool _isVisible;
    private AscentOptimizerController? _controller;
    private AscentOptimizerViewState? _viewState;
    private AscentOptimizerWindow? _window;

    public void Awake()
    {
        if (_instanceActive)
        {
            Destroy(this);
            return;
        }

        _instanceActive = true;
    }

    public void Start()
    {
        if (_controller is not null)
        {
            return;
        }

        _controller = AscentOptimizerRuntimeFactory.CreateController();
        _viewState = _controller.CreateInitialViewState();
        _window = new AscentOptimizerWindow(_controller);
        RegisterToolbarButton();
        GameEvents.onGUIApplicationLauncherReady.Add(RegisterToolbarButton);
    }

    public void OnDestroy()
    {
        GameEvents.onGUIApplicationLauncherReady.Remove(RegisterToolbarButton);
        if (_toolbarControllerBridge is not null)
        {
            _toolbarControllerBridge.Unregister();
            _toolbarControllerBridge = null;
        }

        if (_toolbarButton is not null && ApplicationLauncher.Instance is not null)
        {
            ApplicationLauncher.Instance.RemoveModApplication(_toolbarButton);
            _toolbarButton = null;
        }

        _instanceActive = false;
    }

    public void OnGUI()
    {
        GUI.skin = HighLogic.Skin;

        if (!_isVisible || _window is null || _viewState is null)
        {
            return;
        }

        _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, DrawWindowContents, "Ascent Optimizer");
    }

    private void DrawWindowContents(int windowId)
    {
        _window!.Draw(_viewState!);
        GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
    }

    private void RegisterToolbarButton()
    {
        if (_toolbarControllerBridge is not null || _toolbarButton is not null)
        {
            return;
        }

        _toolbarControllerBridge = ToolbarControllerBridge.TryRegister(
            this,
            OnToolbarEnabled,
            OnToolbarDisabled,
            ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION,
            "KspNasaDifficultyTools",
            "AscentOptimizer",
            AscentOptimizerToolbarIconFactory.Create());

        if (_toolbarControllerBridge is not null)
        {
            return;
        }

        if (!ApplicationLauncher.Ready)
        {
            return;
        }

        _toolbarButton = ApplicationLauncher.Instance.AddModApplication(
            OnToolbarEnabled,
            OnToolbarDisabled,
            null,
            null,
            null,
            null,
            ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW | ApplicationLauncher.AppScenes.TRACKSTATION,
                AscentOptimizerToolbarIconFactory.Create());
    }

    private void OnToolbarEnabled()
    {
        _isVisible = true;
    }

    private void OnToolbarDisabled()
    {
        _isVisible = false;
    }

}
#endif