# Mission Planner Spec (v0.1)

## Goal

Plan missions backward from a target end-state and produce a timed maneuver sequence.

## Input

- Target end-state (orbit, flyby, or landing region)
- Vehicle performance (mass, thrust, Isp, ignition limits)
- Save-state snapshot (current UT, body positions, active vessel orbit)

## Output

- Ordered maneuver list
- Burn durations and separation/coast times
- Launch window candidates (porkchop-style sampling)

## Design notes

- Keep solver independent from UI.
- Keep KSP save readers behind `IKspGameContext` adapters.
- Add optional writer integration for maneuver nodes after unlock.

## First milestone

Kerbin orbit -> Mun flyby -> Kerbin return orbit planner with fixed assumptions.