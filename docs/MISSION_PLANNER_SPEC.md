# Mission Planner Spec (v0.1)

## Goal

Plan missions backward from a target end-state and produce a timed maneuver sequence.

The planner must support both orbital missions and same-body surface transfers so a mission can move cleanly between launch, orbit, transfer, landing, and biome-hop phases.

The planner must also support interplanetary gravity-assist pre-screening and mission-program management where multiple missions can be saved concurrently.

## Input

- Target end-state (orbit, flyby, landing region, waypoint, or same-body hop destination)
- Mission program definition: starting point plus an ordered list of stage goals
- Vehicle performance (mass, thrust, Isp, ignition limits)
- Save-state snapshot (current UT, body positions, active vessel orbit)
- Optional waypoint catalog from the installed waypoint manager mod

## Output

- Ordered maneuver list
- Ordered mission legs with maneuver/coast/surface-hop semantics
- Burn durations and separation/coast times
- Launch window candidates (porkchop-style sampling)
- Surface transfer candidates with estimated range, coast time, and required departure/arrival conditions
- Gravity-assist candidates for each transfer leg with rough estimated delta-v savings and flight-time change
- Persisted mission program records so multiple plans can be saved and revisited

## Design notes

- Keep solver independent from UI.
- Keep KSP save readers behind `IKspGameContext` adapters.
- Keep waypoint lookup behind a dedicated adapter so the planner can target named waypoints without depending directly on a specific waypoint mod assembly.
- Add optional writer integration for maneuver nodes after unlock.
- Model a mission as a sequence of legs so the same plan can include ascent, transfer, landing, and surface-hop segments.
- Allow the planner to emit execution-facing requests that the flight-control layer can consume later.
- Mission data model should be mission-program oriented:
	- Start point
	- Stage list
	- Goal list per stage
- Transfer planning should produce a direct-transfer baseline and then add gravity-assist candidates as optional alternatives.
- Gravity-assist discovery can start as coarse viability screening rather than global optimization.
- Planner should expose an auto-add parking orbit action around landing goals when pre/post parking orbits are missing.

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

## Added mission mode: Multi-stage mission programs

- A mission is represented as:
	- One start point
	- One or more stages
	- One or more goals per stage
- Example:
	- Stage 1: depart Kerbin and capture at Mun
	- Stage 2: land at target site
	- Stage 3: ascend to Mun orbit
	- Stage 4: return to Kerbin orbit
- System requirements:
	- Multiple mission programs saved at once
	- Ability to reload/edit/delete individual mission programs

## Added planning feature: Gravity-assist options

- For each transfer between two goals:
	- Compute direct transfer baseline (initially placeholder until math implementation)
	- Query assist candidates that are potentially viable in the current game epoch
	- Present each candidate with rough:
		- Estimated delta-v savings
		- Estimated flight-time change
		- Encounter window
- Early phase does not require globally optimal assist chains.

## Added quality-of-life: Auto parking orbit insertion

- Trigger condition:
	- User selects a landing/waypoint goal and no adjacent parking orbit goal is present before/after it for that body.
- Action:
	- Insert default parking-orbit goals before descent and/or after ascent.
- Purpose:
	- Reduce manual stage setup for complete mission plans that include descent + ascent around landed operations.