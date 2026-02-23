#!/usr/bin/env bash
# RankUpAPI - Update databases (create DBs if missing, then run migrations)
# Run from repo root. SQL Server container must be running.
# If docker exec sqlcmd fails (not in image), create DBs manually: run docker/init-dbs.sql in SSMS/Azure Data Studio.
set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$ROOT_DIR"

if [ -f "docker/.env" ]; then
  set -a
  source docker/.env 2>/dev/null || true
  set +a
fi
PASS="${MSSQL_SA_PASSWORD:-RankUp@SecurePass1}"

echo "Creating databases on SQL Server (localhost) if not exist ..."
if docker exec rankup-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$PASS" -C -Q "
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_UserDB') CREATE DATABASE RankUp_UserDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_ExamDB') CREATE DATABASE RankUp_ExamDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_AdminDB') CREATE DATABASE RankUp_AdminDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_QualificationDB') CREATE DATABASE RankUp_QualificationDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_QuestionDB') CREATE DATABASE RankUp_QuestionDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_QuizDB') CREATE DATABASE RankUp_QuizDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_SubscriptionDB') CREATE DATABASE RankUp_SubscriptionDB;
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'RankUp_PaymentDB') CREATE DATABASE RankUp_PaymentDB;
" 2>/dev/null; then
  echo "Databases created or already exist."
else
  echo "Note: sqlcmd not available in container. Create DBs manually by running docker/init-dbs.sql in SSMS or Azure Data Studio (connect to localhost,1433)."
fi

echo "Running migrations ..."
"$SCRIPT_DIR/migrate.sh"

echo "Update complete."
