# TimorINSS — Local Development Setup

The system has 4 parts, each running independently via Docker (no need to install Node/.NET/SQL Server directly on the host machine):

| Service | Container name | Image | Port (host) |
|---|---|---|---|
| Database (SQL Server) | `inss-db` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 |
| Backend API (.NET Core 3.1) | `inss-backend` | `mcr.microsoft.com/dotnet/sdk:3.1` | 5000 |
| Internal Frontend / Finance Module (Angular 11) | `inss_interno` | `node:14` | 4300 |
| Enterprise Portal — external self-service (ModuloContribuicoes, Angular) | `inss_contrib` | `node:14` | 4200 |

**Why Docker for frontend/backend:** the dev machine has much newer Node/.NET versions installed than this project requires (Angular 11 needs Node ≤16, the .NET project builds with SDK 3.1) — running directly with `ng serve`/`dotnet run` on the host will fail (e.g. Node 18+ causes a `No such module: http_parser` error). The container isolates the exact runtime version needed, so you don't have to reinstall your environment every time.

Config file: **`docker-compose.yml`** at the repo root — holds the config (image/port/mount/env/network) for all 4 services.

> **Note:** if the containers `inss-db`/`inss-backend`/`inss_interno`/`inss_contrib` are already running (created manually via `docker run`, not through compose), `docker compose up` will error with "container name already in use". In that case just use `docker start <container name>` (see below) — `docker-compose.yml` is only needed when **recreating from scratch** (after `docker rm`) or on a machine that has no containers yet.

```bash
docker compose up -d
```

## Quick start (when containers already exist)

```bash
# 1. Open Docker Desktop (if not already running)
open -a Docker

# 2. Start in order: DB first (wait for healthy) → backend → frontends
docker start inss-db
# wait a few seconds until "healthy":
docker inspect --format='{{.State.Health.Status}}' inss-db

docker start inss-backend
docker start inss_interno
docker start inss_contrib
```

Check everything is up:
```bash
docker ps --filter "name=inss" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
curl -s -o /dev/null -w "finance module (interno): %{http_code}\n" http://localhost:4300/
curl -s -o /dev/null -w "enterprise portal:        %{http_code}\n" http://localhost:4200/
curl -s -o /dev/null -w "backend:                   %{http_code}\n" http://localhost:5000/
```
Open **http://localhost:4300/** for the Finance Module (internal app, Módulo Contabilidade and the rest). Open **http://localhost:4200/** for the Enterprise Portal (external self-service portal — companies/entidades submit declarations, register workers, etc.). Backend returning `404` at `/` is normal (the API has no root route) — as long as it's not "connection refused," the server is running.

## If a container was deleted — commands to recreate it from scratch

**Database:**
```bash
docker run -d \
  --name inss-db \
  --network inss-benefit-app_default \
  -p 1433:1433 \
  -e ACCEPT_EULA=Y \
  -e MSSQL_SA_PASSWORD='INSSdev@2024!' \
  -e MSSQL_PID=Developer \
  mcr.microsoft.com/mssql/server:2022-latest
```

**Backend** (requires a pre-built output — this repo ships with `bin/Release/netcoreapp3.1/...` already built, so it can run with `--no-build` right away, no rebuild needed):
```bash
docker run -d \
  --name inss-backend \
  --network inss-benefit-app_default \
  -p 5000:5000 \
  -v "$(pwd)/backend/TimorINSS-BackEnd-main:/src" \
  -w /src/TimorINSS-BackEnd \
  -e ASPNETCORE_ENVIRONMENT=Development \
  -e "ConnectionStrings__sqlserverconnection=Server=inss-db,1433;Database=TimorINSSModuloContribuicoes;User ID=sa;Password=INSSdev@2024!;TrustServerCertificate=True;Encrypt=False" \
  -e DOTNET_GENERATE_ASPNET_CERTIFICATE=false \
  -e DOTNET_RUNNING_IN_CONTAINER=true \
  -e DOTNET_USE_POLLING_FILE_WATCHER=true \
  -e NUGET_XMLDOC_MODE=skip \
  mcr.microsoft.com/dotnet/sdk:3.1 \
  bash -c "dotnet run -c Release --no-build --no-launch-profile"
```

**Internal Frontend / Finance Module:**
```bash
docker run -d \
  --name inss_interno \
  -p 4300:4300 \
  -v "$(pwd)/financial/TimorINSS-MInterno:/app" \
  -w /app \
  node:14 \
  bash -c "npm install --legacy-peer-deps 2>&1 | tail -5 && npx ng serve --host 0.0.0.0 --port 4300 --disable-host-check"
```

**Enterprise Portal (external self-service — ModuloContribuicoes):**
```bash
docker run -d \
  --name inss_contrib \
  -p 4200:4200 \
  -v "$(pwd)/contribution/ModuloContribuicoes:/app" \
  -w /app \
  node:14 \
  bash -c "npm install --legacy-peer-deps 2>&1 | tail -5 && npx ng serve --host 0.0.0.0 --port 4200 --disable-host-check"
```

Both frontend containers take a few minutes on first run for `npm install`. Watch the logs until you see `Compiled successfully`:
```bash
docker logs -f inss_interno
docker logs -f inss_contrib
```

> `$(pwd)` assumes you're standing in the repo root (`inss_contrib_finance`). If running from elsewhere, use the absolute path to the repo instead.

## Important note — mount path

Containers **must be mounted to the current repo directory** (`inss_contrib_finance/...`), not an old differently-named directory (`benefit_niss/...` — the repo's name before the local rename). If a container starts and then exits immediately with an error like:
- Frontend: `Cannot find module`, or a blank page that can't load `src/`
- Backend: `Couldn't find a project to run. Ensure a project exists in /src/TimorINSS-BackEnd`

→ this means the mount is pointing at the wrong directory (usually because Docker auto-creates an empty directory when the mount source doesn't exist). Fix: `docker rm <container>` then re-run the correct `docker run` command above with `-v` pointing at the real repo directory.

## Stopping when done

```bash
docker stop inss_interno inss_contrib inss-backend inss-db
```
(Use `stop`, not `rm` — this keeps the container around so next time you only need `docker start`, not `npm install`/recreate from scratch.)
