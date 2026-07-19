# Ascent Optimizer Spec (v0.1)

## Goal

Compute and apply a stage-aware flight policy that improves over static acceleration caps.

## Input

- Craft stage data
- Stage min throttle and ignition behavior (RealFuels)
- Aerodynamic estimates (FAR)
- Optional mission-planner guidance request for either ascent-to-orbit or same-body surface hop execution

## Output

- Recommended ascent acceleration envelope over time/altitude
- Suggested MechJeb PVG settings
- Estimated ascent delta-v usage split into ideal/gravity/drag components
- Flight-guidance request format that can also represent waypoint-to-waypoint sub-orbital hops

## Design notes

- Keep MechJeb integration behind `IMechJebAdapter`.
- Keep RealFuels and FAR coupling optional and interface-based.
- Start with deterministic offline optimization before live closed-loop control.
- Keep mission-planner handoff data separate from low-level control so the optimizer can evolve into a reusable execution layer for both orbital ascents and sub-orbital hops.

## Added mission mode: Surface-hop execution

- Accept a planner-produced target containing departure body, destination waypoint, and desired hop envelope.
- Reuse stage throttling, TWR, and drag-estimation logic to estimate whether a hop is flyable.
- Support a later handoff path to MechJeb when possible, while keeping room for custom control logic if MechJeb has no suitable same-body hop mode.