#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
VERSION="${1:-0.1.0}"
CONFIGURATION="${CONFIGURATION:-Release}"

# KSP-AVC expects numeric version fields, so strip optional pre-release suffix.
VERSION_CORE="${VERSION%%-*}"
IFS='.' read -r VERSION_MAJOR VERSION_MINOR VERSION_PATCH <<< "$VERSION_CORE"
VERSION_MAJOR="${VERSION_MAJOR:-0}"
VERSION_MINOR="${VERSION_MINOR:-0}"
VERSION_PATCH="${VERSION_PATCH:-0}"

RELEASE_DIR="$ROOT_DIR/artifacts/release/$VERSION"
STAGING_DIR="$RELEASE_DIR/staging"

rm -rf "$RELEASE_DIR"
mkdir -p "$STAGING_DIR"

pushd "$ROOT_DIR" >/dev/null

dotnet restore

dotnet build "$ROOT_DIR/KspNasaDifficultyTools.sln" -c "$CONFIGURATION" /p:Version="$VERSION" /p:KSPBT_ModVersion="$VERSION"

package_mod() {
  local mod_id="$1"
  local project_name="$2"

  local mod_stage="$STAGING_DIR/$mod_id"
  local mod_source="$ROOT_DIR/src/$project_name/GameData/$mod_id"

  mkdir -p "$mod_stage/GameData"
  cp -R "$mod_source" "$mod_stage/GameData/"

  pushd "$mod_stage" >/dev/null
  zip -r "$RELEASE_DIR/$mod_id-$VERSION.zip" "GameData" >/dev/null
  popd >/dev/null
}

package_mod "KspMissionPlanner" "KspMissionPlanner"
package_mod "KspAscentOptimizer" "KspAscentOptimizer"

echo "Release artifacts written to: $RELEASE_DIR"
ls -1 "$RELEASE_DIR"/*.zip

popd >/dev/null
