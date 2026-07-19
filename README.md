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

## Project context for collaborators and future agents

This section captures the key context behind the repository so contributors can work effectively without re-discovering requirements.

### User challenge profile and intent

- Gameplay setup is a realism-heavy career challenge in the stock-like Kerbol system.
- Primary learning goal is to understand orbital mechanics and mission-design math deeply, not only to automate gameplay.
- Tooling should reduce repetitive trial-and-error while still preserving educational value.

### Realism mod stack and why it matters

- RealFuels:
	- Introduces realistic fuels, ullage/pressure constraints, throttle limits, and ignition limits.
	- Planning must account for minimum throttle, restart limits, and stage-level engine constraints.
- TestFlight:
	- Adds reliability and failure risk that depends on engine usage and flight environment.
	- Better planning for ignition timing and ascent conditions is required.
- FAR (Ferram Aerospace Research):
	- Replaces stock aerodynamics with more realistic aero behavior.
	- Drag and ascent shaping become major optimization factors.
- RealHeat + SystemHeat + DeadlyReentry:
	- Increase thermal risk and reentry consequences.
	- Mission plans should include conservative thermal/entry margins.
- Skyhawk Kerbalism:
	- Expands realism for science progression and mission constraints (life support, radiation, operational pressure).
	- Timing and mission duration outputs are important, not only delta-v.
- Orbital decay mod:
	- Makes low orbits less stable over long durations.
	- Parking-orbit recommendations and mission timing become more important.
- RealAntennas:
	- Increases communication realism.
	- Mission architecture should consider relay/orbit implications.
- Outer Planets Mod:
	- Expands destination set and transfer complexity.
	- Interplanetary planning and gravity assists become high-value features.
- MechJeb 2:
	- Provides autopilot/control baseline; this project extends planning and optimization around it.
- Sigma Dimensions rescale:
	- System is rescaled by 10x (with specific atmosphere/terrain scaling differences).
	- Stock intuition and many stock delta-v assumptions are no longer valid.

### Product goals

- Build two connected tools:
	- `KspMissionPlanner`: mission-program planning from start point through staged goals.
	- `KspAscentOptimizer`: stage-aware ascent/hop guidance policy support and integration handoff.
- Support mission plans that include:
	- Orbital transfers
	- Flybys
	- Landings and waypoint-targeted operations
	- Same-body biome hops/sub-orbital arcs
	- Return phases and multi-stage mission programs
- Surface rough gravity-assist alternatives per transfer leg with coarse:
	- viability window
	- estimated delta-v savings
	- estimated flight-time change

### Learning-first constraints (critical)

- Keep math implementation ownership with the user whenever possible.
- Framework, integration seams, data models, and tests can be scaffolded by agents.
- `KspMathCore` worksheet methods are intentionally left as learning tasks.
- When helping with math, agents should prioritize explanation and structure over writing final formula code directly.

### How agents should help with math problems

For any requested equation or derivation support, use this sequence:

1. Clarify what quantity is being solved for and why it matters in mission planning.
2. List assumptions and coordinate/frame conventions.
3. Present the governing equation(s) and define all symbols and units.
4. Cite source(s) from `docs/REFERENCES.md` and related lesson docs.
5. Walk through a numerical sanity-check example.
6. Let the user implement the formula in worksheet code.
7. Validate using Khan-style tests and edge-case checks.

### Current architecture expectations

- `KspIntegration` holds mod/game integration contracts and reflection-based adapters.
- `KspMissionPlanner` holds mission data models, orchestration, persistence hooks, and planning structure.
- `KspAscentOptimizer` holds ascent/hop guidance policy integration seams.
- `KspMathCore` holds worksheet-style math APIs and challenge boundaries.
- `tests/KspIntegrationTests` should remain green and verify runtime seam behavior.
- `tests/KspMathCoreKhanTests` are expected to fail until worksheet math is implemented.

### Mission persistence expectations

- Multiple mission programs should be saveable and manageable concurrently.
- File-based persistence is JSON with one mission per file.
- Recommended runtime path convention:
	- `GameData/YourMod/Missions/<save-name>/mission-<id>.json`

### UI and workflow expectations in KSP

- Planner should feel like an operations console, not just a checklist.
- Users should be able to:
	- create/select mission programs
	- define staged goals
	- generate direct plans
	- review gravity-assist options
	- auto-insert parking orbit goals around landing stages
	- save and reload plans across sessions
- See `docs/KSP_UI_USAGE_AND_IMPLEMENTATION.md` for proposed UI structure and plugin lifecycle hooks.