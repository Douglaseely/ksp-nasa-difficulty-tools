#if KSP_RUNTIME
using System;
using System.Collections.Generic;
using System.Reflection;
using KspMissionPlanner.Planning;
using UnityEngine;

namespace KspMissionPlanner.Plugin;

public sealed class MissionMapOverlayRenderer
{
    private readonly Texture2D _lineTexture = CreateLineTexture();

    public void Draw(MissionPlannerViewState viewState)
    {
        if (!MapView.MapIsEnabled || viewState.CurrentMissionProgram is null || viewState.CurrentPlan is null)
        {
            return;
        }

        var camera = ResolveMapCamera();
        if (camera is null)
        {
            return;
        }

        var allGoals = FlattenGoals(viewState.CurrentMissionProgram.Stages);
        if (allGoals.Count == 0)
        {
            return;
        }

        var goalById = new Dictionary<string, MissionGoal>(StringComparer.OrdinalIgnoreCase);
        var stageByGoalId = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var stageIndex = 0; stageIndex < viewState.CurrentMissionProgram.Stages.Count; stageIndex++)
        {
            foreach (var goal in viewState.CurrentMissionProgram.Stages[stageIndex].Goals)
            {
                goalById[goal.GoalId] = goal;
                stageByGoalId[goal.GoalId] = stageIndex;
            }
        }

        var overlayRect = new Rect(20f, 80f, 360f, 220f);
        GUILayout.BeginArea(overlayRect, "Mission Map Preview", HighLogic.Skin.window);
        GUILayout.Label($"Active mission: {viewState.CurrentMissionProgram.Name}");
        GUILayout.Label($"Legs: {viewState.CurrentPlan.Legs.Count}  Maneuvers: {viewState.CurrentPlan.Maneuvers.Count}");
        GUILayout.EndArea();

        for (var legIndex = 0; legIndex < viewState.CurrentPlan.Legs.Count; legIndex++)
        {
            var leg = viewState.CurrentPlan.Legs[legIndex];
            if (!goalById.TryGetValue(leg.FromGoalId, out var fromGoal) || !goalById.TryGetValue(leg.ToGoalId, out var toGoal))
            {
                continue;
            }

            var fromBody = FlightGlobals.GetBodyByName(fromGoal.TargetBody);
            var toBody = FlightGlobals.GetBodyByName(toGoal.TargetBody);
            if (fromBody is null || toBody is null)
            {
                continue;
            }

            var stageIndex = stageByGoalId.TryGetValue(toGoal.GoalId, out var mappedStage) ? mappedStage : 0;
            var stageColor = GetStageColor(stageIndex);

            var fromPoint = GetScreenPoint(camera, fromBody.position);
            var toPoint = GetScreenPoint(camera, toBody.position);
            DrawLine(fromPoint, toPoint, stageColor, 2.5f);
            DrawLabel(toPoint + new Vector2(8f, -8f), toGoal.Name, stageColor);

            if (toGoal.OrbitTarget is not null)
            {
                var orbitRadius = Mathf.Clamp((float)(toGoal.OrbitTarget.ApoapsisAltitudeMeters / 50000.0), 10f, 48f);
                DrawCircle(toPoint, orbitRadius, stageColor, 1.25f, 26);
            }

            if (viewState.CurrentPlan.Maneuvers.Count > legIndex)
            {
                var markerColor = GetManeuverColor(legIndex);
                var marker = Vector2.Lerp(fromPoint, toPoint, 0.5f);
                DrawMarker(marker, markerColor);
            }
        }
    }

    private static List<MissionGoal> FlattenGoals(IReadOnlyList<MissionStage> stages)
    {
        var goals = new List<MissionGoal>();
        foreach (var stage in stages)
        {
            foreach (var goal in stage.Goals)
            {
                goals.Add(goal);
            }
        }

        return goals;
    }

    private static Vector2 GetScreenPoint(Camera camera, Vector3d worldPosition)
    {
        var scaled = ScaledSpace.LocalToScaledSpace(worldPosition);
        var point = camera.WorldToScreenPoint(scaled);
        return new Vector2(point.x, Screen.height - point.y);
    }

    private static Camera? ResolveMapCamera()
    {
        var mapCamera = MapView.MapCamera;
        if (mapCamera is null)
        {
            return null;
        }

        var type = mapCamera.GetType();
        foreach (var memberName in new[] { "camera", "cam", "scaledSpaceCamera", "MapCamera", "Camera" })
        {
            var property = type.GetProperty(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (property?.GetValue(mapCamera, null) is Camera cameraFromProperty)
            {
                return cameraFromProperty;
            }

            var field = type.GetField(memberName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field?.GetValue(mapCamera) is Camera cameraFromField)
            {
                return cameraFromField;
            }
        }

        return Camera.main;
    }

    private void DrawLine(Vector2 start, Vector2 end, Color color, float width)
    {
        var angle = Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg;
        var length = Vector2.Distance(start, end);
        var previousColor = GUI.color;
        GUI.color = color;
        GUIUtility.RotateAroundPivot(angle, start);
        GUI.DrawTexture(new Rect(start.x, start.y - width * 0.5f, length, width), _lineTexture);
        GUIUtility.RotateAroundPivot(-angle, start);
        GUI.color = previousColor;
    }

    private void DrawCircle(Vector2 center, float radius, Color color, float width, int steps)
    {
        var previous = center + new Vector2(radius, 0f);
        for (var step = 1; step <= steps; step++)
        {
            var t = (float)step / steps;
            var angle = t * Mathf.PI * 2f;
            var next = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            DrawLine(previous, next, color, width);
            previous = next;
        }
    }

    private void DrawMarker(Vector2 center, Color color)
    {
        const float size = 6f;
        var previous = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(new Rect(center.x - size * 0.5f, center.y - size * 0.5f, size, size), _lineTexture);
        GUI.color = previous;
    }

    private static void DrawLabel(Vector2 position, string text, Color color)
    {
        var style = new GUIStyle(HighLogic.Skin.label)
        {
            normal = { textColor = color },
            fontStyle = FontStyle.Bold,
        };
        GUI.Label(new Rect(position.x, position.y, 260f, 22f), text, style);
    }

    private static Color GetStageColor(int stageIndex)
    {
        Color[] palette =
        {
            new Color(0.94f, 0.28f, 0.26f, 1f),
            new Color(0.18f, 0.78f, 0.43f, 1f),
            new Color(0.23f, 0.56f, 0.95f, 1f),
            new Color(0.95f, 0.76f, 0.22f, 1f),
            new Color(0.88f, 0.42f, 0.94f, 1f),
            new Color(0.20f, 0.84f, 0.84f, 1f),
        };
        return palette[Mathf.Abs(stageIndex) % palette.Length];
    }

    private static Color GetManeuverColor(int maneuverIndex)
    {
        Color[] palette =
        {
            new Color(1f, 1f, 1f, 1f),
            new Color(1f, 0.86f, 0.36f, 1f),
            new Color(1f, 0.48f, 0.48f, 1f),
            new Color(0.55f, 0.90f, 1f, 1f),
        };
        return palette[Mathf.Abs(maneuverIndex) % palette.Length];
    }

    private static Texture2D CreateLineTexture()
    {
        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        return texture;
    }
}
#endif