# KSP NASA Difficulty Tools

This repository contains two connected Kerbal Space Program mod projects:

- `KspMissionPlanner`: in-game mission planning and reverse-mission design.
- `KspAscentOptimizer`: ascent profile optimization that can integrate with MechJeb, RealFuels, and FAR.

It also contains a shared library:

- `KspMathCore`: orbital mechanics and ascent math worksheets that are intentionally left for you to implement.

## Learning-first design

The architecture is designed so you write the math yourself:

- Framework and integration seams are scaffolded.
- Calculation methods throw `NotImplementedException` with a learning hint.
- Lesson docs are math-only: equations, variable definitions, modeling assumptions, and sources.
- Every principle listed in the lesson docs is paired with a source so you can trace it back to a textbook, NASA reference, or foundational paper.
- No worked code examples are included in the lesson docs.

## Repository layout

- `src/KspMathCore`: reusable math APIs and worksheet stubs.
- `src/KspMissionPlanner`: mission planner models and plugin bootstrap seam.
- `src/KspAscentOptimizer`: ascent optimizer models and integration adapters.
- `docs`: implementation roadmap, equation list, and references.

## Current planning scope

- Reverse-planned orbital missions such as flybys, return trajectories, and target orbits.
- Same-body surface transfers using waypoint targets and sub-orbital hop planning.
- Flight-controller handoff architecture so the mission planner can eventually feed ascent and hop guidance requests into the execution layer.

## Quick start

1. `cd /Users/douglas.seely/Desktop/ksp-nasa-difficulty-tools`
2. `dotnet restore`
3. `dotnet build`

## Test lanes

- `dotnet test tests/KspIntegrationTests/KspIntegrationTests.csproj`
	- Validates the KSP/mod integration seam, reflection adapters, and bootstrap wiring.
- `dotnet test tests/KspMathCoreKhanTests/KspMathCoreKhanTests.csproj`
	- Fixed-answer Khan-style checks for your worksheet math. These are expected to fail until you implement the worksheet methods.

## Mission persistence

- Mission programs can be stored via `FileMissionProgramStore` as one JSON file per mission.
- Recommended KSP runtime location: `GameData/YourMod/Missions/<save-name>/`.

## UI planning

- See `docs/KSP_UI_USAGE_AND_IMPLEMENTATION.md` for in-game workflow, tab layout, lifecycle hooks, and implementation milestones.

## Next implementation steps

1. Implement equations in `KspMathCore` worksheet files.
2. Work through the sourced lesson documents and hand-check each equation against one mission scenario.
3. Add KSP/Unity references and plugin entry points for each mod.
4. Wire game-state readers and waypoint readers into the adapter interfaces.
5. Add unit tests for every math function before wiring runtime behavior.