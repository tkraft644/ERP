#!/bin/zsh
set -euo pipefail

cd "$(dirname "$0")/.."
dotnet run --project src/ErpSystem.Api --launch-profile http
