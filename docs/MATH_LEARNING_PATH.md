# Math Learning Path

This file is intentionally math-only. It contains equations, modeling assumptions, symbol definitions, and sources. It does not contain implementation code.

## Stage 1: Two-body orbit foundations

### Circular-orbit speed

- Purpose: speed required for a circular orbit of radius `r` around a body with gravitational parameter `mu`.
- Equation: `v_c = sqrt(mu / r)`
- Symbols:
	- `v_c`: circular speed
	- `mu`: standard gravitational parameter, `G M`
	- `r`: orbital radius from body center
- Source:
	- Bate, Mueller, White, *Fundamentals of Astrodynamics*, Dover, Chapter 2.
	- Curtis, *Orbital Mechanics for Engineering Students*, Chapter 2.

### Vis-viva equation

- Purpose: speed at any point on a Keplerian orbit.
- Equation: `v = sqrt(mu * (2 / r - 1 / a))`
- Symbols:
	- `v`: orbital speed at radius `r`
	- `r`: instantaneous distance from the attracting body's center
	- `a`: semi-major axis
	- `mu`: standard gravitational parameter
- Source:
	- Bate, Mueller, White, Chapter 2.
	- Curtis, Chapter 3.
	- Vallado, *Fundamentals of Astrodynamics and Applications*, Section 2.

### Hohmann transfer

- Purpose: minimum-energy two-impulse transfer between coplanar circular orbits.
- Equations:
	- `a_t = (r_1 + r_2) / 2`
	- `delta_v_1 = sqrt(mu / r_1) * (sqrt((2 r_2) / (r_1 + r_2)) - 1)`
	- `delta_v_2 = sqrt(mu / r_2) * (1 - sqrt((2 r_1) / (r_1 + r_2)))`
	- `t_t = pi * sqrt(a_t^3 / mu)`
- Symbols:
	- `r_1`, `r_2`: initial and target circular-orbit radii
	- `a_t`: transfer-orbit semi-major axis
	- `delta_v_1`, `delta_v_2`: transfer impulses
	- `t_t`: half-period transfer time
- Source:
	- Walter Hohmann, *Die Erreichbarkeit der Himmelskorper*, 1925.
	- Bate, Mueller, White, Chapter 3.
	- Curtis, Chapter 6.

## Stage 2: Timing and transfer windows

### Mean motion

- Purpose: average angular rate of a Keplerian orbit.
- Equation: `n = sqrt(mu / a^3)`
- Symbols:
	- `n`: mean motion in radians per second
	- `mu`: standard gravitational parameter
	- `a`: semi-major axis
- Source:
	- Bate, Mueller, White, Chapter 2.
	- Curtis, Chapter 3.

### Synodic period

- Purpose: repeat interval between similar phase alignments of two bodies in approximately circular orbits.
- Equation: `T_syn = 1 / |1 / T_1 - 1 / T_2|`
- Symbols:
	- `T_syn`: synodic period
	- `T_1`, `T_2`: orbital periods of the two bodies
- Source:
	- Curtis, Chapter 8.
	- Vallado, interplanetary transfer timing discussion.

### Phase angle for Hohmann-style departure

- Purpose: required departure geometry for a transfer that arrives when the target reaches the intercept point.
- Equation:
	- `gamma_req = pi - n_target * t_t` for the standard outer-target circular approximation
- Symbols:
	- `gamma_req`: required phase angle at departure
	- `n_target`: target mean motion
	- `t_t`: transfer time
- Source:
	- Bate, Mueller, White, Chapter 8.
	- Curtis, Chapter 8.

### Lambert boundary-value framing

- Purpose: transfer between two position vectors in a specified time of flight.
- Problem statement:
	- Given `r_1`, `r_2`, `mu`, and `Delta t`, solve for a conic arc and endpoint velocities `v_1`, `v_2`.
- Source:
	- J. H. Lambert, *Insigniores Orbitae Cometarum Proprietates*, 1761.
	- R. H. Battin, *An Introduction to the Mathematics and Methods of Astrodynamics*, Lambert chapter.
	- R. H. Gooding, “A Procedure for the Solution of Lambert’s Orbital Boundary-Value Problem,” *Celestial Mechanics and Dynamical Astronomy*, 1990.

## Stage 3: Burn sizing and execution

### Tsiolkovsky rocket equation

- Purpose: ideal impulsive delta-v from propellant mass ratio.
- Equation: `delta_v = v_e * ln(m_0 / m_f)`
- Alternate form: `delta_v = I_sp * g_0 * ln(m_0 / m_f)`
- Symbols:
	- `delta_v`: ideal velocity increment
	- `v_e`: effective exhaust velocity
	- `I_sp`: specific impulse
	- `g_0`: standard gravity
	- `m_0`, `m_f`: initial and final mass
- Source:
	- K. E. Tsiolkovsky, “The Exploration of Cosmic Space by Means of Reaction Devices,” 1903.
	- NASA Glenn Research Center, rocket equation reference pages.
	- Sutton and Biblarz, *Rocket Propulsion Elements*, performance chapters.

### Mass flow rate

- Purpose: convert thrust and exhaust velocity into propellant consumption rate.
- Equation: `mdot = T / v_e = T / (I_sp * g_0)`
- Symbols:
	- `mdot`: propellant mass flow rate
	- `T`: thrust
	- `v_e`: effective exhaust velocity
	- `I_sp`: specific impulse
	- `g_0`: standard gravity
- Source:
	- NASA Glenn Research Center, propulsion basics.
	- Sutton and Biblarz, propulsion fundamentals.

### Finite burn duration

- Purpose: estimate burn duration from thrust level and propellant expenditure.
- Equations:
	- `m_f = m_0 / exp(delta_v / v_e)`
	- `t_b = (m_0 - m_f) / mdot`
- Symbols:
	- `t_b`: burn duration
	- `m_0`, `m_f`: initial and final mass for the burn
	- `mdot`: mass flow rate
- Source:
	- Bate, Mueller, White, propulsion application sections.
	- Sutton and Biblarz, finite-burn performance discussion.

## Stage 4: Ascent and flight-constraint quantities

### Thrust-to-weight ratio

- Purpose: instantaneous acceleration authority relative to local gravity.
- Equation: `TWR = T / (m g)`
- Symbols:
	- `T`: thrust
	- `m`: vehicle mass
	- `g`: local gravitational acceleration
- Source:
	- Newtonian force balance.
	- Sutton and Biblarz, launch-vehicle performance basics.

### Dynamic pressure

- Purpose: aerodynamic loading proxy for ascent and entry.
- Equation: `q = 0.5 * rho * v^2`
- Symbols:
	- `q`: dynamic pressure
	- `rho`: atmospheric density
	- `v`: vehicle speed relative to the air mass
- Source:
	- NASA Glenn Research Center, dynamic pressure and aerodynamic force references.
	- Anderson, *Fundamentals of Aerodynamics*, introductory compressible/incompressible flow treatment.

### Loss accounting model

- Purpose: partition ascent delta-v into ideal orbital requirement, gravity losses, drag losses, and steering losses.
- Approximate bookkeeping identity:
	- `delta_v_required ~= delta_v_orbital + delta_v_gravity + delta_v_drag + delta_v_steering`
- Source:
	- Sutton and Biblarz, launch analysis chapters.
	- Humble, Henry, Larson, *Space Propulsion Analysis and Design*, ascent-performance discussions.

## Stage 5: Same-body surface hops and waypoint missions

### Great-circle surface distance

- Purpose: surface distance between two lat/lon points on the same approximately spherical body.
- Equations:
	- `Delta sigma = arccos(sin(phi_1) sin(phi_2) + cos(phi_1) cos(phi_2) cos(Delta lambda))`
	- `s = R * Delta sigma`
- Symbols:
	- `phi_1`, `phi_2`: latitudes
	- `Delta lambda`: longitude difference
	- `Delta sigma`: central angle
	- `R`: body radius
	- `s`: arc distance along the surface
- Source:
	- Bowditch, *The American Practical Navigator*, great-circle navigation sections.
	- Vallado, coordinate geometry background sections.

### Sub-orbital hop as a two-body ellipse

- Purpose: model a hop between two surface points by selecting an elliptic arc that departs and re-enters the same body.
- Useful quantities:
	- `r_p = R + h_departure`
	- `r_a = R + h_apoapsis`
	- `a = (r_p + r_a) / 2`
	- `e = (r_a - r_p) / (r_a + r_p)`
	- endpoint speed from vis-viva at the chosen departure and arrival radii
- Source:
	- Bate, Mueller, White, ellipse geometry and vis-viva.
	- Curtis, time-of-flight and anomaly relations.

### Time of flight on an elliptic arc

- Purpose: compute coast time between departure and re-entry points on the chosen transfer ellipse.
- Equations:
	- `M = E - e sin(E)`
	- `t = M / n`, with `n = sqrt(mu / a^3)`
- Symbols:
	- `E`: eccentric anomaly
	- `M`: mean anomaly
	- `e`: eccentricity
	- `n`: mean motion
	- `a`: semi-major axis
- Source:
	- Bate, Mueller, White, Chapter 4.
	- Curtis, Chapters 3 and 4.