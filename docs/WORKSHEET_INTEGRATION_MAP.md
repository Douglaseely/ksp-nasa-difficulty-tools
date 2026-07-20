# Worksheet Integration Map

This document shows where the new mod flow layer expects math implementation.

## Mission Planner path

UI-facing entry point:

- `KspMissionPlanner.Plugin.MissionPlannerController`

Planning orchestration:

- `KspMissionPlanner.Plugin.MissionPlannerBootstrap`

Worksheet-backed helper file for you to implement:

- `src/KspMissionPlanner/Integrations/MissionPlannerMath.cs`

Methods to fill in:

- `WorksheetMissionMathService.EstimateOrbitalTransfer(...)`
  - expected worksheet dependencies:
    - `HohmannTransferWorksheet`
    - `WindowSearchWorksheet`
    - later `LambertWorksheet`
- `WorksheetMissionMathService.EstimateTerminalApproach(...)`
  - use when mission legs end at landing or waypoint goals
- `WorksheetMissionMathService.EstimateSurfaceHop(...)`
  - expected worksheet dependency:
    - `SurfaceHopWorksheet`

Current flow:

1. UI calls `MissionPlannerController.GeneratePlan(...)`
2. controller validates mission structure
3. bootstrap captures game state and waypoint availability
4. bootstrap calls `IMissionMathService.EstimateTransfer(...)` for each leg
5. bootstrap returns `MissionPlan` with legs, direct-transfer estimates, assist candidates, and planned maneuvers

## Ascent Optimizer path

UI-facing entry point:

- `KspAscentOptimizer.Plugin.AscentOptimizerController`

Guidance orchestration:

- `KspAscentOptimizer.Plugin.AscentOptimizerBootstrap`

Worksheet-backed helper file for you to implement:

- `src/KspAscentOptimizer/Integrations/AscentGuidanceMath.cs`

Methods to fill in:

- `WorksheetAscentGuidanceMathService.BuildAscentToOrbitPolicy(...)`
  - expected worksheet dependency:
    - `AscentOptimizationWorksheet`
- `WorksheetAscentGuidanceMathService.BuildSurfaceHopPolicy(...)`
  - expected worksheet dependencies:
    - `SurfaceHopWorksheet`
    - `AscentOptimizationWorksheet`

Current flow:

1. UI calls `AscentOptimizerController.PreviewGuidance(...)` or `EngageGuidance(...)`
2. bootstrap captures real-fuels stage data, FAR aero data, and MechJeb capability flags
3. bootstrap calls `IAscentGuidanceMathService.BuildGuidancePolicy(...)`
4. returned policy is either previewed in UI or applied to MechJeb limits/autopilot

## What is intentionally still missing

- Unity/KSP concrete window drawing code
- KSP addon attributes and toolbar registration
- actual worksheet math inside the helper methods above

Those pieces are isolated now so you can work the math without changing the controller/bootstrap flow.