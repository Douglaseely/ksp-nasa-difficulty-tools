# Mission Planner Spec (v0.1)

## Goal

Plan missions backward from a target end-state and produce a timed maneuver sequence.

The planner must support both orbital missions and same-body surface transfers so a mission can move cleanly between launch, orbit, transfer, landing, and biome-hop phases.

## Input

- Target end-state (orbit, flyby, landing region, waypoint, or same-body hop destination)
- Vehicle performance (mass, thrust, Isp, ignition limits)
- Save-state snapshot (current UT, body positions, active vessel orbit)
- Optional waypoint catalog from the installed waypoint manager mod

## Output

- Ordered maneuver list
- Ordered mission legs with maneuver/coast/surface-hop semantics
- Burn durations and separation/coast times
- Launch window candidates (porkchop-style sampling)
- Surface transfer candidates with estimated range, coast time, and required departure/arrival conditions

## Design notes

- Keep solver independent from UI.
- Keep KSP save readers behind `IKspGameContext` adapters.
- Keep waypoint lookup behind a dedicated adapter so the planner can target named waypoints without depending directly on a specific waypoint mod assembly.
- Add optional writer integration for maneuver nodes after unlock.
- Model a mission as a sequence of legs so the same plan can include ascent, transfer, landing, and surface-hop segments.
- Allow the planner to emit execution-facing requests that the flight-control layer can consume later.

## First milestone

Kerbin orbit -> Mun flyby -> Kerbin return orbit planner with fixed assumptions.

## Added mission mode: Same-body waypoint and biome hops

- Example use cases:
	- Woomerang launch site -> Kerbin north-pole science hop
	- Mun landing site -> nearby biome hop -> ascent to rendezvous orbit
- Required planning data:
	- Departure and arrival latitude/longitude
	- Body radius and gravitational parameter
	- Allowed apoapsis window for the hop
	- Atmospheric model availability for drag-risk estimation
- Planning outputs:
	- Surface distance between waypoints
	- Candidate sub-orbital arc geometry
	- Estimated hop delta-v, coast time, and entry conditions
	- Execution handoff for the flight-control mod