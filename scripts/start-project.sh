#!/bin/zsh
set -euo pipefail

cd "$(dirname "$0")/.."

SQL_CONTAINER_NAME="erpsystem-sqlserver"
SQL_HOST_PORT="${ERP_SQL_HOST_PORT:-11433}"
SQL_CONNECTION_STRING="${ERPSYSTEM_CONNECTION_STRING:-Server=localhost,${SQL_HOST_PORT};Database=ErpSystemDb;User Id=sa;Password=ErpSqlLocal2026!;TrustServerCertificate=True;MultipleActiveResultSets=True}"
API_BASE_URL="${ERP_API_BASE_URL:-http://localhost:5180}"
API_BASE_URL="${API_BASE_URL%/}"
API_LOG="${ERP_API_LOG:-/tmp/erpsystem-api.log}"
EF_LOG="${ERP_EF_LOG:-/tmp/erpsystem-dbupdate.log}"
MAX_DB_ATTEMPTS="${ERP_DB_ATTEMPTS:-30}"
MAX_API_ATTEMPTS="${ERP_API_ATTEMPTS:-45}"
SLEEP_SECONDS="${ERP_START_SLEEP_SECONDS:-2}"
API_PID=""
STARTED_API=0

cleanup() {
  if [[ "${STARTED_API}" == "1" && -n "${API_PID}" ]] && kill -0 "${API_PID}" >/dev/null 2>&1; then
    echo "Zatrzymuje API..."
    kill "${API_PID}" >/dev/null 2>&1 || true
    wait "${API_PID}" >/dev/null 2>&1 || true
  fi

  STARTED_API=0
}

on_signal() {
  cleanup
  exit 130
}

require_command() {
  local command_name="$1"
  local install_hint="$2"

  if ! command -v "${command_name}" >/dev/null 2>&1; then
    echo "Brakuje polecenia '${command_name}'. ${install_hint}"
    exit 1
  fi
}

health_ready() {
  curl --silent --fail --max-time 2 "${API_BASE_URL}/health" >/dev/null 2>&1
}

sql_port_mapping() {
  ERP_SQL_HOST_PORT="${SQL_HOST_PORT}" docker compose port sqlserver 1433 2>/dev/null || true
}

ensure_sql_port_mapping() {
  local mapping
  mapping="$(sql_port_mapping)"

  if [[ "${mapping}" == *":${SQL_HOST_PORT}" ]]; then
    return
  fi

  echo "Kontener SQL Server nie wystawia portu localhost:${SQL_HOST_PORT}."
  echo "Aktualne mapowanie: ${mapping:-brak}"
  echo "Odtwarzam kontener z konfiguracji docker-compose.yml..."
  ERP_SQL_HOST_PORT="${SQL_HOST_PORT}" docker compose up -d --force-recreate sqlserver

  mapping="$(sql_port_mapping)"
  if [[ "${mapping}" != *":${SQL_HOST_PORT}" ]]; then
    echo "SQL Server nadal nie wystawia portu ${SQL_HOST_PORT}."
    echo "Aktualne mapowanie: ${mapping:-brak}"
    echo "Sprawdz, czy port ${SQL_HOST_PORT} nie jest zajety przez inna usluge."
    exit 1
  fi
}

trap cleanup EXIT
trap on_signal INT TERM

require_command dotnet "Zainstaluj .NET SDK 9."
require_command docker "Zainstaluj Docker Desktop."
require_command curl "Zainstaluj curl albo uruchom API i desktop recznie."

if ! dotnet ef --version >/dev/null 2>&1; then
  echo "Brakuje narzedzia dotnet-ef. Zainstaluj je poleceniem:"
  echo "dotnet tool install --global dotnet-ef --version 9.*"
  exit 1
fi

if ! docker info >/dev/null 2>&1; then
  echo "Docker daemon nie dziala. Uruchom Docker Desktop i sprobuj ponownie."
  exit 1
fi

echo "Buduje rozwiazanie..."
dotnet build ErpSystem.sln --nologo

echo "Uruchamiam lokalny SQL Server..."
ERP_SQL_HOST_PORT="${SQL_HOST_PORT}" docker compose up -d sqlserver
ensure_sql_port_mapping

echo "Czekam na SQL Server i stosuje migracje..."
attempt=1
while (( attempt <= MAX_DB_ATTEMPTS )); do
  if ConnectionStrings__ErpDb="${SQL_CONNECTION_STRING}" ConnectionStrings__ErpSystemDatabase="${SQL_CONNECTION_STRING}" ERPSYSTEM_CONNECTION_STRING="${SQL_CONNECTION_STRING}" dotnet ef database update --no-build --project src/ErpSystem.Infrastructure --startup-project src/ErpSystem.Api >"${EF_LOG}" 2>&1; then
    echo "Migracje zastosowane poprawnie."
    break
  fi

  echo "Proba ${attempt}/${MAX_DB_ATTEMPTS}: baza jeszcze nie jest gotowa. Czekam ${SLEEP_SECONDS}s..."
  sleep "${SLEEP_SECONDS}"
  attempt=$(( attempt + 1 ))
done

if (( attempt > MAX_DB_ATTEMPTS )); then
  echo "Nie udalo sie zastosowac migracji."
  echo "Ostatni log z EF:"
  tail -n 120 "${EF_LOG}" || true
  echo
  echo "Sprawdz kontener ${SQL_CONTAINER_NAME}: docker compose logs sqlserver"
  exit 1
fi

if health_ready; then
  echo "API juz dziala pod ${API_BASE_URL}."
else
  echo "Uruchamiam API pod ${API_BASE_URL}..."
  ConnectionStrings__ErpDb="${SQL_CONNECTION_STRING}" ConnectionStrings__ErpSystemDatabase="${SQL_CONNECTION_STRING}" ERPSYSTEM_CONNECTION_STRING="${SQL_CONNECTION_STRING}" ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="${API_BASE_URL}" \
    dotnet run --no-build --project src/ErpSystem.Api --no-launch-profile >"${API_LOG}" 2>&1 &
  API_PID=$!
  STARTED_API=1

  attempt=1
  while (( attempt <= MAX_API_ATTEMPTS )); do
    if health_ready; then
      echo "API jest gotowe."
      break
    fi

    if ! kill -0 "${API_PID}" >/dev/null 2>&1; then
      echo "API zakonczylo dzialanie przed osiagnieciem gotowosci."
      echo "Ostatni log API:"
      tail -n 120 "${API_LOG}" || true
      exit 1
    fi

    echo "Proba ${attempt}/${MAX_API_ATTEMPTS}: API jeszcze startuje. Czekam ${SLEEP_SECONDS}s..."
    sleep "${SLEEP_SECONDS}"
    attempt=$(( attempt + 1 ))
  done

  if (( attempt > MAX_API_ATTEMPTS )); then
    echo "API nie odpowiedzialo na ${API_BASE_URL}/health."
    echo "Ostatni log API:"
    tail -n 120 "${API_LOG}" || true
    exit 1
  fi
fi

echo "Uruchamiam aplikacje desktopowa..."
echo "Po zamknieciu desktopa API uruchomione przez ten skrypt zostanie zatrzymane."
ERP_API_BASE_URL="${API_BASE_URL}/" dotnet run --no-build --project src/ErpSystem.Desktop
