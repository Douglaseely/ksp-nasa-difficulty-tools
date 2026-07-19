# KSP Mission Planner UI: Usage and Implementation

This document defines how the mod should feel in game, what the user flow looks like, and how to implement the UI in KSP 1.

## In-game usage conversation

The player interaction loop should feel like this:

1. Open Mission Planner from toolbar.
2. Choose an existing mission program or create a new one.
3. Set mission start point.
4. Add stage goals in order.
5. Click "Plan" to compute direct transfer legs.
6. Review gravity-assist options that appear per transfer leg.
7. Click "Auto add parking orbits" for landing goals if needed.
8. Save mission program.
9. Optionally export planned maneuvers to maneuver nodes when unlocked.

## Core windows and tabs

Use one main window with tabs to keep state stable:

- Missions tab:
  - Mission list
  - Create, duplicate, delete mission
  - Save indicator
- Stages tab:
  - Stage list
  - Goal editor per stage
  - Reorder goals
- Planning tab:
  - Planned transfer legs
  - Delta-v and time estimates
  - Gravity-assist candidate cards with rough savings and time change
- Actions tab:
  - Auto parking orbit insertion toggles (before/after landing)
  - Regenerate plan
  - Export nodes
- Diagnostics tab:
  - Current vessel/body/time context
  - Integration adapter availability (MechJeb/FAR/RealFuels/Waypoint)

## KSP 1 implementation approach

## UI technology

- Use Unity IMGUI (`OnGUI`) for first implementation speed.
- Later migrate to a custom skin/theme, still IMGUI-based, to stay compatible with KSP 1 mod ecosystem.

## Plugin lifecycle hooks

- Use `KSPAddon(KSPAddon.Startup.Flight, false)` for flight scene planner.
- Optionally add separate addon for tracking station or map view.
- Register toolbar button using `ApplicationLauncher`.
- Keep planner state singleton scoped to save name.

## Scene integration

- Flight scene:
  - Full planning and node export
- Tracking station:
  - Read-only mission viewing and scheduling
- Space center:
  - Mission program management only

## View-model layering

Keep UI thin; no orbital math in UI code.

- UI layer:
  - Window state, tab selection, text input, validation messaging
- Application layer:
  - `MissionPlannerBootstrap` orchestration
  - Save/load via `FileMissionProgramStore`
  - Gravity assist scouting via `IGravityAssistScout`
- Domain layer:
  - Mission model objects and math worksheets

## Suggested classes

- `MissionPlannerAddon`:
  - Owns toolbar button and scene lifecycle
- `MissionPlannerWindow`:
  - Draws IMGUI window and tabs
- `MissionPlannerViewState`:
  - Current selected mission/stage/goal, dirty state, expanded sections
- `MissionPlannerController`:
  - Calls bootstrap for save/load/plan/augment actions

## Data and save strategy

- Serialize each mission program to one JSON file:
  - `GameData/YourMod/Missions/<save-name>/mission-<id>.json`
- Keep autosave cadence:
  - save on explicit button click
  - save on scene change if dirty
  - optional timed autosave every N seconds in flight

## Gravity-assist UI behavior

- For each transfer leg show:
  - Direct transfer baseline card
  - Zero or more assist cards
- Assist card fields:
  - Assist body
  - Window start/end
  - Estimated delta-v savings
  - Estimated flight-time change
  - Confidence note (coarse estimate)
- Do not auto-select assist by default.

## Parking orbit auto-insert UX

- Show action as explicit button near landing goals:
  - `Add pre-landing parking orbit`
  - `Add post-landing parking orbit`
  - `Add both`
- Display generated altitudes and inclination before apply.

## Error handling and validation

- Validation rules:
  - Mission must have at least one stage and one goal.
  - Waypoint goals must resolve in waypoint catalog.
  - Orbit goals must include body and altitudes.
- UI should show non-blocking warning badges for unresolved integrations.

## Initial milestone breakdown

1. Toolbar button + empty planner window.
2. Mission list with create/save/delete wired to file store.
3. Stage and goal editor.
4. Plan generation table with direct transfer placeholders.
5. Gravity-assist candidate cards per leg.
6. Parking-orbit auto insertion controls.
7. Node export action when maneuver-node writer is available.