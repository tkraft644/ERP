#!/bin/zsh
set -euo pipefail

cd "$(dirname "$0")/.."
exec ./scripts/start-project.sh
