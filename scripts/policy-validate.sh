#!/usr/bin/env bash
set -euo pipefail

if [ ! -f "governance.config.json" ]; then
  echo "Missing governance.config.json"
  exit 1
fi

python - <<'PY'
import json
required = [
  "version", "policyGeneratedBy", "bootstrap", "versionControl", "testing",
  "documentation", "language", "autonomy", "phases", "ciCdEnforced", "remoteRequired"
]
with open("governance.config.json", "r", encoding="utf-8") as f:
  data = json.load(f)
missing = [k for k in required if k not in data]
if missing:
  raise SystemExit(f"Missing policy keys: {missing}")
print("Policy validated")
PY
