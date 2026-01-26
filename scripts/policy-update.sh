#!/usr/bin/env bash
set -euo pipefail

VERSION_CONTROL="${1:-}"
TESTING="${2:-}"
DOCUMENTATION="${3:-}"
LANGUAGE="${4:-}"
FRAMEWORKS="${5:-}"
AUTONOMY="${6:-}"

if [ ! -f "governance.config.json" ]; then
  echo "Missing governance.config.json"
  exit 1
fi

VERSION_CONTROL="$VERSION_CONTROL" TESTING="$TESTING" DOCUMENTATION="$DOCUMENTATION" LANGUAGE="$LANGUAGE" FRAMEWORKS="$FRAMEWORKS" AUTONOMY="$AUTONOMY" python - <<'PY'
import json
import os

version_control = os.environ.get("VERSION_CONTROL", "")
testing = os.environ.get("TESTING", "")
documentation = os.environ.get("DOCUMENTATION", "")
language = os.environ.get("LANGUAGE", "")
frameworks = os.environ.get("FRAMEWORKS", "")
autonomy = os.environ.get("AUTONOMY", "")

allowed_version_control = {"git-local", "git-remote", "git-remote-ci"}
allowed_testing = {"full", "baseline"}
allowed_documentation = {"inline", "comments-only", "generate"}
allowed_autonomy = {"feature", "milestone", "fully-autonomous"}

with open("governance.config.json", "r", encoding="utf-8") as f:
  policy = json.load(f)

if version_control:
  if version_control not in allowed_version_control:
    raise SystemExit("Invalid VersionControl")
  policy["versionControl"] = version_control
  policy["remoteRequired"] = version_control != "git-local"

if testing:
  if testing not in allowed_testing:
    raise SystemExit("Invalid Testing")
  policy["testing"] = testing

if documentation:
  if documentation not in allowed_documentation:
    raise SystemExit("Invalid Documentation")
  policy["documentation"] = documentation

if language:
  policy["language"]["primary"] = language

if frameworks:
  if frameworks == "None":
    policy["language"]["frameworks"] = []
  else:
    policy["language"]["frameworks"] = [f.strip() for f in frameworks.split(",") if f.strip()]

if autonomy:
  if autonomy not in allowed_autonomy:
    raise SystemExit("Invalid Autonomy")
  policy["autonomy"] = autonomy

with open("governance.config.json", "w", encoding="utf-8") as f:
  json.dump(policy, f, indent=2)

print("Updated governance policy at governance.config.json")
PY
