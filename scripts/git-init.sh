#!/usr/bin/env bash
set -euo pipefail

if [ -d ".git" ]; then
  echo "Git already initialized."
  exit 0
fi

git init

echo "Git initialized. Configure remote as required by policy."
