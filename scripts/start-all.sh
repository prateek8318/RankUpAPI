#!/usr/bin/env bash
# RankUpAPI - Start all services with Docker Compose
# Run from repo root: ./scripts/start-all.sh
set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$ROOT_DIR"

if [ ! -f "docker/.env" ]; then
  echo "Creating docker/.env from docker/.env.example ..."
  cp docker/.env.example docker/.env
fi

echo "Starting RankUpAPI stack (Gateway + SQL Server + all microservices) ..."
docker compose -f docker/docker-compose.yml --env-file docker/.env up -d --build

echo ""
echo "Waiting for SQL Server to be ready (~30s). Then create DBs: run docker/init-dbs.sql in SSMS/Azure Data Studio, or use scripts/update-db.sh if sqlcmd is available."
sleep 30
echo ""
echo "Gateway: http://localhost:${GATEWAY_PORT:-5087}"
echo "SQL Server: localhost:${MSSQL_PORT:-1433} (sa / password from docker/.env)"
echo ""
echo "To view logs: docker compose -f docker/docker-compose.yml logs -f"
echo "To stop: docker compose -f docker/docker-compose.yml down"
