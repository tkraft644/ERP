#!/bin/zsh
set -euo pipefail

cd "$(dirname "$0")/.."
SQL_HOST_PORT="${ERP_SQL_HOST_PORT:-11433}"
ERP_SQL_HOST_PORT="${SQL_HOST_PORT}" docker compose up -d sqlserver

echo "SQL Server startuje na localhost:${SQL_HOST_PORT}"
echo "Login: sa"
echo "Haslo: ErpSqlLocal2026!"
