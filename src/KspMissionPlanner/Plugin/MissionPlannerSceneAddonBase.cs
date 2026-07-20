#if KSP_RUNTIME
using System;
using KSP.UI.Screens;
using UnityEngine;

namespace KspMissionPlanner.Plugin;

public abstract class MissionPlannerSceneAddonBase : MonoBehaviour
{
    private static bool _instanceActive;

    private ApplicationLauncherButton? _toolbarButton;
    private ToolbarControllerBridge? _toolbarControllerBridge;
    private Rect _windowRect = new Rect(180f, 120f, 720f, 520f);
    private bool _isVisible;

    private MissionPlannerController? _controller;
    private MissionPlannerViewState? _viewState;
    private MissionPlannerWindow? _window;
    private MissionMapOverlayRenderer? _mapOverlay;

    protected virtual bool AllowNodeExport => HighLogic.LoadedSceneIsFlight;
    protected virtual string WindowTitle => "Mission Planner";

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

        _controller = MissionPlannerRuntimeFactory.CreateController(AllowNodeExport);
        _viewState = _controller.CreateInitialViewState();
        _window = new MissionPlannerWindow(_controller);
        _mapOverlay = new MissionMapOverlayRenderer();
        RegisterToolbarButton();
        GameEvents.onGUIApplicationLauncherReady.Add(RegisterToolbarButton);
    }

    public void OnDestroy()
    {
        if (_controller is not null && _viewState?.CurrentMissionProgram is not null && _viewState.IsDirty)
        {
            try
            {
                _viewState.CurrentMissionProgram = _controller.SaveMissionProgram(_viewState.CurrentMissionProgram);
                _viewState.IsDirty = false;
            }
            catch
            {
            }
        }

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

        if (_mapOverlay is not null && _viewState is not null)
        {
            _mapOverlay.Draw(_viewState);
        }

        if (!_isVisible || _window is null || _viewState is null)
        {
            return;
        }

        _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, DrawWindowContents, WindowTitle);
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
            ApplicationLauncher.AppScenes.ALWAYS,
            "KspNasaDifficultyTools",
            "MissionPlanner",
            MissionPlannerToolbarIconFactory.Create());

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
            ApplicationLauncher.AppScenes.ALWAYS,
                MissionPlannerToolbarIconFactory.Create());
    }

    private void OnToolbarEnabled()
    {
        _isVisible = true;
    }

    private void OnToolbarDisabled()
    {
        _isVisible = false;
    }

    private static Texture2D CreateToolbarTexture()
    {
        var texture = new Texture2D(38, 38, TextureFormat.ARGB32, false);
        var fill = new Color(0.14f, 0.42f, 0.72f, 1f);
        var pixels = new Color[38 * 38];
        for (var index = 0; index < pixels.Length; index++)
        {
            pixels[index] = fill;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}
#endif