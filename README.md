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
- Docs explain what equations to implement and why.

## Repository layout

- `src/KspMathCore`: reusable math APIs and worksheet stubs.
- `src/KspMissionPlanner`: mission planner models and plugin bootstrap seam.
- `src/KspAscentOptimizer`: ascent optimizer models and integration adapters.
- `docs`: implementation roadmap, equation list, and references.

## Quick start

1. `cd /Users/douglas.seely/Desktop/ksp-nasa-difficulty-tools`
2. `dotnet restore`
3. `dotnet build`

## Next implementation steps

1. Implement equations in `KspMathCore` worksheet files.
2. Add KSP/Unity references and plugin entry points for each mod.
3. Wire game-state readers into the adapter interfaces.
4. Add unit tests for every math function before wiring runtime behavior.