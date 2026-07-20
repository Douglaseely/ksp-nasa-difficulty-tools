# CKAN Metadata Staging

This folder is a staging area for your release metadata before you submit to CKAN.

## Files

- `KspMissionPlanner.ckan.template`
- `KspAscentOptimizer.ckan.template`

Each template includes placeholders you must replace:

- `YOUR_GITHUB_ORG`
- `YOUR_REPO`
- `{{ version }}`
- `{{ github_release_zip_url_for_mission_planner }}`
- `{{ github_release_zip_url_for_ascent_optimizer }}`

## Publishing flow summary

1. Create release zips with `scripts/package-release.sh`.
2. Publish a GitHub release and upload both zip files.
3. Fill out template values and save final `.ckan` files.
4. Submit those metadata files to the CKAN-meta repository.

## Notes

- Keep one zip per mod identifier.
- Keep the zip root as `GameData/...`.
- Update the `license` field to the correct SPDX-style identifier before submitting.
