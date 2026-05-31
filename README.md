# ErpSystem

Szkielet rozwiązania ERP z podziałem na warstwy:

- `ErpSystem.Domain` - encje i reguły domenowe
- `ErpSystem.Application` - przypadki użycia i logika aplikacyjna
- `ErpSystem.Infrastructure` - dostęp do danych i integracje
- `ErpSystem.Api` - backend HTTP
- `ErpSystem.Desktop` - klient desktopowy w Avalonia
- `ErpSystem.Shared` - kontrakty i współdzielone typy
- `ErpSystem.Tests` - testy jednostkowe i integracyjne

Repozytorium zostało wyczyszczone ze starego MAUI i przygotowane od nowa pod Avalonię.

## Lokalny start

Desktop loguje się wyłącznie przez API, a API korzysta z SQL Servera.

Najprostszy start całego projektu:

- `./run.sh`

To samo robi alias zgodny ze starszą instrukcją:

- `./scripts/start-all-local.sh`

Skrypt:

- buduje rozwiązanie,
- uruchamia lokalny kontener SQL Server,
- czeka aż baza będzie gotowa,
- wykonuje migracje EF,
- uruchamia API na `http://localhost:5180`,
- uruchamia aplikację desktopową i kieruje ją na lokalne API.

Wymagania lokalne:

- .NET SDK 9
- Docker Desktop
- narzędzie `dotnet-ef` (`dotnet tool install --global dotnet-ef --version 9.*`)

Ręczny start nadal jest możliwy:

1. Uruchom lokalny SQL Server:
   `./scripts/start-local-sql.sh`
2. Zastosuj migracje:
   `dotnet ef database update --project src/ErpSystem.Infrastructure --startup-project src/ErpSystem.Api`
3. Uruchom API:
   `./scripts/start-local-api.sh`
4. Uruchom desktop:
   `dotnet run --project src/ErpSystem.Desktop`

## Rider

Da się to wygodnie odpalać z Ridera.

Najprostszy układ:

1. Dodaj run configuration typu `Shell Script`
   Script path: `$ProjectFileDir$/run.sh`
   Interpreter: `/bin/zsh`
   Working directory: `$ProjectFileDir$`
2. Uruchom tę konfigurację.

Jeśli chcesz, możesz też w Riderze zrobić `Compound configuration`, która uruchomi:

- backend przez `scripts/start-local-sql.sh` i `scripts/start-local-api.sh`
- desktop `ErpSystem.Desktop`

Domyślne dane logowania do ERP:

- login: `admin`
- hasło: ``

Lokalny SQL Server:

- host: `localhost,11433`
- login: `sa`
- hasło: ``

## ERP modules

Startowy modułowy rdzeń:

- System
- Contractors
- Warehouse
- Domestic transport
- International transport
- HR
- Finance / Costs
- Documents
- Reports
- Administration

Katalogi modułów zostały przygotowane w:

- `src/ErpSystem.Domain/Modules/*`
- `src/ErpSystem.Application/Modules/*`
- `src/ErpSystem.Infrastructure/Modules/*`
- `src/ErpSystem.Api/Features/*`
- `src/ErpSystem.Desktop/Views/*`
- `src/ErpSystem.Desktop/ViewModels/*`

## Shared foundation

Pierwszy wspólny rdzeń systemu obejmuje:

- users
- roles
- permissions
- permission-driven menu
- audit log
- dictionary items
- attachments
- document history
- document statuses
- document number sequences

Przykładowe endpointy fundamentu:

- `GET /features/system/foundation/users`
- `GET /features/system/foundation/users/{userId}/menu`
- `GET /features/system/foundation/permissions`
- `GET /features/system/foundation/audit`
- `GET /features/system/foundation/sequences`
- `POST /features/system/foundation/sequences/{key}/next`
