# Math Learning Path

Implement these in order so each step builds intuition for the next.

## Stage 1: Core orbit math

- Circular velocity: `v = sqrt(mu / r)`
- Vis-viva: `v = sqrt(mu * (2 / r - 1 / a))`
- Hohmann transfer: two impulses + transfer time

## Stage 2: Timing and windows

- Mean motion: `n = sqrt(mu / a^3)`
- Synodic period: `T_syn = 1 / |1/T1 - 1/T2|`
- Phase angle for transfer opportunity

## Stage 3: Burn execution

- Tsiolkovsky equation: `delta_v = v_e * ln(m0 / mf)`
- Mass flow: `mdot = thrust / v_e`
- Burn duration from consumed propellant and `mdot`

## Stage 4: Ascent optimization intuition

- Gravity losses and drag losses as additive penalties
- Dynamic pressure: `q = 0.5 * rho * v^2`
- TWR envelope constraints with min/max throttle

## Practical rule

For each equation:

1. Write the derivation in comments or docs.
2. Implement code.
3. Validate with a known worked example.
4. Add a unit test that checks both nominal and edge-case inputs.