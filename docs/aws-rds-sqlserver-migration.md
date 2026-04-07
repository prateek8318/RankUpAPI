# RankUpAPI AWS RDS SQL Server Migration

This repo uses one SQL Server database per service. The Docker setup is now parameterized so the same compose file can target either the local `sqlserver` container or an AWS RDS SQL Server endpoint.

## Database map

| Service | Connection string key | Database name | Schema source |
| --- | --- | --- | --- |
| UserService | `UserServiceConnection` | `RankUp_UserDB` | `database/RankUp_UserDB_Script.sql` |
| ExamService | `ExamServiceConnection` | `RankUp_ExamDB` | application-specific SQL/stored procedures |
| AdminService | `AdminServiceConnection` | `RankUp_AdminDB` | `database/RankUp_AdminDB_Script.sql` |
| MasterService | `MasterServiceConnection` | `RankUp_MasterDB` | `database/RankUp_MasterDB_Script.sql` |
| QuestionService | `QuestionServiceConnection` | `RankUp_QuestionDB` | EF Core migrations in `Services/QuestionService/QuestionService.Infrastructure/Migrations` |
| QuizService | `QuizServiceConnection` | `RankUp_QuizDB` | EF Core migrations in `Services/QuizService/QuizService.Infrastructure/Migrations` |
| SubscriptionService | `SubscriptionServiceConnection` | `RankUp_SubscriptionDB` | service startup migration/manual SQL review required |
| PaymentService | `PaymentServiceConnection` | `RankUp_PaymentDB` | EF Core migrations in `Services/PaymentService/PaymentService.Infrastructure/Migrations` |
| HomeDashboardService | `HomeDashboardServiceConnection` | `RankUp_HomeDashboardDB` | EF Core migrations in `Services/HomeDashboardService/HomeDashboardService.Infrastructure/Migrations` |
| TestService | `DefaultConnection` | `RankUpAPI_TestService` | EF Core migrations in `Services/TestService/TestService.Infrastructure/Migrations` |

## Important project findings

1. Most services do not auto-create schema on startup. A successful connection only means the database is reachable.
2. `docker/init-dbs.sql` previously missed `RankUp_MasterDB`, `RankUp_HomeDashboardDB`, and the TestService database.
3. `TestService` uses `DefaultConnection`, not a named `TestServiceConnection`.
4. The repo already contains full SQL bootstrap scripts for `User`, `Admin`, and `Master`.

## Recommended production flow

1. Create the RDS instance and allow port `1433` from your EC2/private network.
2. Set `.env.production` or your deployment environment to:

```env
DB_SERVER=rankup-db.xxxxxx.ap-south-1.rds.amazonaws.com,1433
DB_USER=admin
DB_PASSWORD=StrongPassword@123
USERSERVICE_DB_NAME=RankUp_UserDB
EXAMSERVICE_DB_NAME=RankUp_ExamDB
ADMINSERVICE_DB_NAME=RankUp_AdminDB
MASTERSERVICE_DB_NAME=RankUp_MasterDB
QUESTIONSERVICE_DB_NAME=RankUp_QuestionDB
QUIZSERVICE_DB_NAME=RankUp_QuizDB
SUBSCRIPTIONSERVICE_DB_NAME=RankUp_SubscriptionDB
PAYMENTSERVICE_DB_NAME=RankUp_PaymentDB
HOMEDASHBOARDSERVICE_DB_NAME=RankUp_HomeDashboardDB
TESTSERVICE_DB_NAME=RankUpAPI_TestService
```

3. Run `docker/init-dbs.sql` against RDS first so every service database exists.
4. Apply schema in this order:
   - Run `database/RankUp_MasterDB_Script.sql`
   - Run `database/RankUp_UserDB_Script.sql`
   - Run `database/RankUp_AdminDB_Script.sql`
   - Apply EF migrations for `QuestionService`, `QuizService`, `PaymentService`, `HomeDashboardService`, and `TestService`
   - Review `SubscriptionService` and `ExamService` schema source before go-live if they are not already present in the target server
5. Point Docker services to RDS by starting compose with the RDS env file.
6. Verify each service with logs and a simple health/API request.

## Running the existing bootstrap SQL

Use SSMS/Azure Data Studio connected to RDS and run:

- `docker/init-dbs.sql`
- `database/RankUp_MasterDB_Script.sql`
- `database/RankUp_UserDB_Script.sql`
- `database/RankUp_AdminDB_Script.sql`

## Notes for missing/partial schemas

- `ExamService` currently has stored procedure files under `Services/ExamService/ExamService.Infrastructure/StoredProcedures`, but this repo does not expose one consolidated full database creation script alongside the other three databases.
- `SubscriptionService` has `Services/SubscriptionService/Scripts/SubscriptionService_StoredProcedures.sql`, but you should confirm whether table creation is handled elsewhere before cutover.
- If your live Docker SQL Server already has the complete schema/data, the most reliable migration path is to export each database from the running source and import into RDS instead of rebuilding part of the schema by hand.
