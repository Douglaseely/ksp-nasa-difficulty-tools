# Ascent Optimizer Spec (v0.1)

## Goal

Compute and apply a stage-aware ascent policy that improves over static acceleration caps.

## Input

- Craft stage data
- Stage min throttle and ignition behavior (RealFuels)
- Aerodynamic estimates (FAR)

## Output

- Recommended ascent acceleration envelope over time/altitude
- Suggested MechJeb PVG settings
- Estimated ascent delta-v usage split into ideal/gravity/drag components

## Design notes

- Keep MechJeb integration behind `IMechJebAdapter`.
- Keep RealFuels and FAR coupling optional and interface-based.
- Start with deterministic offline optimization before live closed-loop control.