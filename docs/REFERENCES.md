# References

This file is a source map for the lesson material. Each mathematical principle named in `MATH_LEARNING_PATH.md` is tied to at least one source here.

## Core astrodynamics texts

- Bate, Roger R.; Mueller, Donald D.; White, Jerry E. *Fundamentals of Astrodynamics*. Dover Publications.
	- Use for: circular velocity, vis-viva, orbital geometry, Hohmann transfer, anomaly relations, time of flight.
- Curtis, Howard D. *Orbital Mechanics for Engineering Students*. Butterworth-Heinemann.
	- Use for: two-body orbit derivations, transfer timing, phase angles, interplanetary windows, Lambert framing.
- Vallado, David A. *Fundamentals of Astrodynamics and Applications*. Microcosm Press.
	- Use for: orbit-state representations, transfer timing, coordinate systems, practical astrodynamics conventions.
- Battin, Richard H. *An Introduction to the Mathematics and Methods of Astrodynamics*. AIAA.
	- Use for: deeper treatment of Lambert solvers and boundary-value methods.

## Foundational and historical sources

- Hohmann, Walter. *Die Erreichbarkeit der Himmelskorper*. 1925.
	- Use for: original minimum-energy transfer concept now called the Hohmann transfer.
- Tsiolkovsky, Konstantin E. “The Exploration of Cosmic Space by Means of Reaction Devices.” 1903.
	- Use for: original rocket-equation formulation.
- Lambert, Johann Heinrich. *Insigniores Orbitae Cometarum Proprietates*. 1761.
	- Use for: original statement of the transfer-time boundary-value problem now called Lambert’s problem.
- Gooding, R. H. “A Procedure for the Solution of Lambert’s Orbital Boundary-Value Problem.” *Celestial Mechanics and Dynamical Astronomy*, 1990.
	- Use for: practical modern Lambert-solver reference.

## Propulsion and launch-vehicle references

- Sutton, George P.; Biblarz, Oscar. *Rocket Propulsion Elements*. Wiley.
	- Use for: rocket equation, mass flow rate, finite burn estimation, launch-vehicle performance.
- Humble, Ronald W.; Henry, Gary N.; Larson, Wiley J. *Space Propulsion Analysis and Design*. McGraw-Hill.
	- Use for: ascent performance breakdown, gravity and drag loss framing.
- NASA Glenn Research Center propulsion pages.
	- Use for: `delta_v = I_sp g_0 ln(m_0 / m_f)`, `mdot = T / (I_sp g_0)`, and propulsion variable definitions.
	- Starting points:
		- https://www1.grc.nasa.gov/beginners-guide-to-aeronautics/ideal-rocket-equation/
		- https://www1.grc.nasa.gov/beginners-guide-to-aeronautics/specific-impulse/

## Aerodynamics and flight loads

- Anderson, John D. *Fundamentals of Aerodynamics*. McGraw-Hill.
	- Use for: dynamic pressure and aerodynamic loading context.
- NASA Glenn Research Center aerodynamic force references.
	- Use for: dynamic pressure definition and the relation between `q`, density, and speed.
	- Starting point:
		- https://www1.grc.nasa.gov/beginners-guide-to-aeronautics/dynamic-pressure-2/

## Surface geometry and navigation

- Bowditch, Nathaniel. *The American Practical Navigator*.
	- Use for: great-circle distance and spherical navigation formulas.
- Vallado, David A. *Fundamentals of Astrodynamics and Applications*.
	- Use for: coordinate-system conventions and body-fixed to inertial reasoning.

## KSP integration references

- FAR source and documentation.
	- Use for: estimating how the mod exposes aerodynamic quantities and drag behavior.
- MechJeb 2 source and documentation.
	- Use for: PVG settings, ascent-control extension points, and autopilot handoff.
- RealFuels source and documentation.
	- Use for: minimum throttle, ignition limits, ullage constraints, and engine configuration data.
- Waypoints Manager source and documentation.
	- Use for: surface target lookup, waypoint metadata, and body/biome destination integration.