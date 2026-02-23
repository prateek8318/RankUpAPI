# RankUpAPI – Docker

Run the full RankUpAPI stack (Gateway + SQL Server + all microservices) with Docker Compose.

## Prerequisites

- **Docker Desktop** (Windows/Mac) or **Docker Engine** + **Docker Compose** (Linux)
- Optional: **Git Bash** or **WSL** on Windows to run `scripts/*.sh`

## Quick start

From the **repository root**:

```bash
# 1. Copy env (edit docker/.env if you need different passwords/ports)
cp docker/.env.example docker/.env

# 2. Start everything (builds images and starts containers)
docker compose -f docker/docker-compose.yml --env-file docker/.env up -d --build

# 3. Create databases and run migrations (after SQL Server is up, ~30–60 seconds)
# Windows (PowerShell):
cd scripts; bash update-db.sh; cd ..

# Or run migrations manually per service (see below)
```

- **Gateway (API entry point):** http://localhost:5087  
- **SQL Server:** localhost:1433 (user `sa`, password from `docker/.env`)

## Compose files

| File | Purpose |
|------|--------|
| `docker/docker-compose.yml` | Full stack: SQL Server + Gateway + all microservices |
| `docker/docker-compose.override.yml` | Local overrides (optional) |
| `docker/.env` | Env vars (copy from `docker/.env.example`) |

## Commands (from repo root)

```bash
# Start stack
docker compose -f docker/docker-compose.yml --env-file docker/.env up -d --build

# View logs
docker compose -f docker/docker-compose.yml logs -f

# Stop
docker compose -f docker/docker-compose.yml down

# Stop and remove volumes (resets SQL Server data)
docker compose -f docker/docker-compose.yml down -v
```

## Databases and migrations

SQL Server in Docker starts without databases. Options:

1. **Let APIs create/migrate**  
   Many services run EF Core migrations on startup. Start the stack, wait ~1–2 minutes, then call the Gateway; some services will create/update their DB on first use.

2. **Scripts (if you have Bash)**  
   - `scripts/update-db.sh` – creates DBs (if the SQL container has `sqlcmd`) and runs migrations.  
   - `scripts/migrate.sh` – runs `dotnet ef database update` for each service (requires .NET SDK and connection to Docker SQL).

3. **Manual**  
   Connect to `localhost:1433` with SSMS or Azure Data Studio and create databases, then run migrations per service (see each service’s `appsettings` for DB names).

## Environment variables (docker/.env)

| Variable | Default | Description |
|----------|---------|-------------|
| `MSSQL_SA_PASSWORD` | `RankUp@SecurePass1` | SQL Server `sa` password |
| `MSSQL_PORT` | `1433` | Host port for SQL Server |
| `GATEWAY_PORT` | `5087` | Host port for the API Gateway |

## Gateway and Ocelot

- **Local/dev:** Gateway uses `ocelot.json` (localhost + various ports).
- **Docker:** Gateway uses `ocelot.docker.json` (service names + port 80), loaded when `OCELOT_CONFIG=ocelot.docker.json` (set in docker-compose).

## Service layout in Docker

- Each API runs inside its container on **port 80**.
- Only the **Gateway** is published on the host (default **5087**).
- All services use the **rankup-network** and resolve each other by **service name** (e.g. `userservice`, `examservice`).

## Troubleshooting

- **APIs fail to connect to SQL:** Wait 30–60 s after `docker compose up` for SQL Server to be ready. Ensure `ConnectionStrings` in compose use hostname `sqlserver` and the same password as in `docker/.env`.
- **Gateway returns 502/503:** Downstream services may still be starting; wait a bit and retry.
- **.NET 10 (Gateway):** Gateway uses .NET 10; ensure Docker can pull `mcr.microsoft.com/dotnet/sdk:10.0` and `aspnet:10.0`. If not available in your environment, change `Services/GatewayAPI/Dockerfile` to use `8.0` and align `GatewayAPI.csproj` with `net8.0`.
