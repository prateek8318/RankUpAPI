#!/usr/bin/env bash
# RankUpAPI - Run EF Core migrations for all services (against Docker SQL Server)
# Run from repo root. Requires: SQL Server container running and dotnet CLI.
set -e
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$ROOT_DIR"

# Load env if present
if [ -f "docker/.env" ]; then
  set -a
  source docker/.env 2>/dev/null || true
  set +a
fi
CONN_PREFIX="Server=localhost;User Id=sa;Password=${MSSQL_SA_PASSWORD:-RankUp@SecurePass1};TrustServerCertificate=True;"

echo "Running migrations (ensure SQL Server container is up) ..."

run_migration() {
  local name=$1
  local proj=$2
  local db=$3
  echo "  → $name"
  dotnet ef database update --project "$proj" --connection "$CONN_PREFIX;Database=$db" --no-build 2>/dev/null || \
  dotnet ef database update --project "$proj" --no-build 2>/dev/null || true
}

run_migration "UserService"       "Services/UserService/UserService.API/UserService.API.csproj"           "RankUp_UserDB"
run_migration "ExamService"       "Services/ExamService/ExamService.API/ExamService.API.csproj"           "RankUp_ExamDB"
run_migration "AdminService"      "Services/AdminService/AdminService.API/AdminService.API.csproj"         "RankUp_AdminDB"
run_migration "MasterService"     "Services/MasterService/MasterService.API/MasterService.API.csproj"       "RankUp_QualificationDB"
run_migration "QuestionService"  "Services/QuestionService/QuestionService.API/QuestionService.API.csproj" "RankUp_QuestionDB"
run_migration "QuizService"      "Services/QuizService/QuizService.API/QuizService.API.csproj"             "RankUp_QuizDB"
run_migration "SubscriptionService" "Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj" "RankUp_SubscriptionDB"
run_migration "PaymentService"   "Services/PaymentService/PaymentService.API/PaymentService.API.csproj"   "RankUp_PaymentDB"

echo "Done. If any failed, ensure DB exists and connection string in appsettings matches docker/.env."
