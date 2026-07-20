# Release Packaging and CKAN Prep

This document describes the non-math release workflow for both mods.

## What is now configured

- Deterministic release zip packaging script: `scripts/package-release.sh`
- CKAN metadata templates: `ckan/*.ckan.template`
- CI workflow that restores, builds, tests integration seams, and creates zip artifacts: `.github/workflows/ci.yml`

## Create release zips locally

Run from repository root:

```bash
bash scripts/package-release.sh 0.1.0
```

Outputs:

- `artifacts/release/0.1.0/KspMissionPlanner-0.1.0.zip`
- `artifacts/release/0.1.0/KspAscentOptimizer-0.1.0.zip`

Each zip is built in KSP-style layout:

- `GameData/KspMissionPlanner/...`
- `GameData/KspAscentOptimizer/...`

The packaging script builds the solution with `KSPBuildTools` metadata and then zips the generated project-local `GameData` folders, which means the packaged payload matches the `net481` KSP runtime build.

## Local test deployment

- `dotnet build` deploys the generated `GameData/<ModName>` folders into the KSP install specified by `KSPBT_GameRoot`.
- This repository defaults `KSPBT_GameRoot` to your current local macOS install path in `Directory.Build.props`.
- Override it per-build if needed:
   - `KSPBT_GameRoot=/path/to/KSP dotnet build`

## CKAN submission prep

1. Publish a GitHub release and upload both zip files.
2. Copy each `*.ckan.template` to a concrete `*.ckan` metadata file.
3. Fill in placeholders:
   - GitHub org/repo
   - exact mod version
   - exact release zip URL
   - license value
4. Submit the `.ckan` metadata to CKAN-meta.

## Important notes

- The `.version` files generated in each zip are for KSP-AVC compatibility.
- Keep one zip per mod identifier to keep CKAN install stanzas simple.
- Keep `KspMathCore.dll` and `KspIntegration.dll` bundled in each mod zip unless you later split those into separately distributed CKAN packages.
