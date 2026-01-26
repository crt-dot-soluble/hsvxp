#!/usr/bin/env bash
set -euo pipefail

MODE="${1:-defaults}"
VERSION_CONTROL="${2:-git-remote-ci}"
TESTING="${3:-full}"
DOCUMENTATION="${4:-inline}"
LANGUAGE="${5:-unspecified}"
FRAMEWORKS="${6:-}"
AUTONOMY="${7:-feature}"

if [ "$MODE" != "customize" ]; then
  VERSION_CONTROL="git-remote-ci"
  TESTING="full"
  DOCUMENTATION="inline"
  LANGUAGE="unspecified"
  FRAMEWORKS=""
  AUTONOMY="feature"
fi

if [ "$FRAMEWORKS" = "None" ]; then
  FRAMEWORKS=""
fi

if [ -f "${REPO_ROOT}/README.md" ]; then
  if grep -q "AI AGENTS MUST IGNORE THIS FILE" "${REPO_ROOT}/README.md"; then
    rm -f "${REPO_ROOT}/README.md"
    echo "Removed default README.md"
  fi
fi

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

MODE="$MODE" VERSION_CONTROL="$VERSION_CONTROL" TESTING="$TESTING" DOCUMENTATION="$DOCUMENTATION" LANGUAGE="$LANGUAGE" FRAMEWORKS="$FRAMEWORKS" AUTONOMY="$AUTONOMY" REPO_ROOT="$REPO_ROOT" python - <<'PY'
import json
import os
import datetime

mode = os.environ.get("MODE")
version_control = os.environ.get("VERSION_CONTROL")
testing = os.environ.get("TESTING")
documentation = os.environ.get("DOCUMENTATION")
language = os.environ.get("LANGUAGE")
frameworks = os.environ.get("FRAMEWORKS", "")
autonomy = os.environ.get("AUTONOMY")

policy = {
  "version": "1.0.0",
  "policyGeneratedBy": "bootstrap",
  "bootstrap": {
    "mode": mode,
    "skipped": mode != "customize",
    "timestamp": datetime.date.today().isoformat()
  },
  "versionControl": version_control,
  "testing": testing,
  "documentation": documentation,
  "language": {
    "primary": language,
    "frameworks": [f.strip() for f in frameworks.split(",") if f.strip()]
  },
  "autonomy": autonomy,
  "phases": {"required": True, "list": [0,1,2,3,4,5]},
  "ciCdEnforced": True,
  "remoteRequired": version_control != "git-local"
}

repo_root = os.environ.get("REPO_ROOT") or os.getcwd()
with open(os.path.join(repo_root, "governance.config.json"), "w", encoding="utf-8") as f:
  json.dump(policy, f, indent=2)

print("Wrote governance policy to governance.config.json")
PY
