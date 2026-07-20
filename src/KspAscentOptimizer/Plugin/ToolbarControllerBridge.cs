#if KSP_RUNTIME
using System;
using System.Reflection;
using KSP.UI.Screens;
using UnityEngine;

namespace KspAscentOptimizer.Plugin;

internal sealed class ToolbarControllerBridge
{
    private readonly Component _toolbarComponent;
    private readonly MethodInfo? _destroyMethod;

    private ToolbarControllerBridge(Component toolbarComponent, MethodInfo? destroyMethod)
    {
        _toolbarComponent = toolbarComponent;
        _destroyMethod = destroyMethod;
    }

    public static ToolbarControllerBridge? TryRegister(
        MonoBehaviour host,
        Action onEnable,
        Action onDisable,
        ApplicationLauncher.AppScenes scenes,
        string @namespace,
        string toolbarId,
        Texture2D icon)
    {
        try
        {
            var toolbarType = Type.GetType("ToolbarControl_NS.ToolbarControl, ToolbarController");
            if (toolbarType is null)
            {
                return null;
            }

            var component = host.gameObject.AddComponent(toolbarType);
            var addMethod = ResolveAddMethod(toolbarType);
            if (addMethod is null)
            {
                UnityEngine.Object.Destroy(component);
                return null;
            }

            var arguments = BuildArguments(addMethod, onEnable, onDisable, scenes, @namespace, toolbarId, icon);
            addMethod.Invoke(component, arguments);

            var destroyMethod = toolbarType.GetMethod("OnDestroy", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return new ToolbarControllerBridge(component, destroyMethod);
        }
        catch
        {
            return null;
        }
    }

    public void Unregister()
    {
        try
        {
            _destroyMethod?.Invoke(_toolbarComponent, null);
        }
        catch
        {
        }

        UnityEngine.Object.Destroy(_toolbarComponent);
    }

    private static MethodInfo? ResolveAddMethod(Type toolbarType)
    {
        foreach (var method in toolbarType.GetMethods(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!string.Equals(method.Name, "AddToAllToolbars", StringComparison.Ordinal))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length >= 6)
            {
                return method;
            }
        }

        return null;
    }

    private static object?[] BuildArguments(
        MethodInfo method,
        Action onEnable,
        Action onDisable,
        ApplicationLauncher.AppScenes scenes,
        string @namespace,
        string toolbarId,
        Texture2D icon)
    {
        var parameters = method.GetParameters();
        var args = new object?[parameters.Length];
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameterType = parameters[index].ParameterType;
            if (parameterType == typeof(Action))
            {
                args[index] = index == 0 ? onEnable : onDisable;
                continue;
            }

            if (parameterType == typeof(ApplicationLauncher.AppScenes))
            {
                args[index] = scenes;
                continue;
            }

            if (parameterType == typeof(string))
            {
                if (index == 3)
                {
                    args[index] = @namespace;
                }
                else if (index == 4)
                {
                    args[index] = toolbarId;
                }
                else
                {
                    args[index] = "KspAscentOptimizer/Icons/ascent-toolbar";
                }

                continue;
            }

            if (typeof(Texture).IsAssignableFrom(parameterType))
            {
                args[index] = icon;
                continue;
            }

            args[index] = parameterType.IsValueType ? Activator.CreateInstance(parameterType) : null;
        }

        return args;
    }
}
#endif