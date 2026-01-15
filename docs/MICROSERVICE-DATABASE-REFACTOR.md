# Microservice Database Refactoring

## Overview

This document describes the refactoring of the RankUp microservices solution from a single shared database to separate dedicated databases for each microservice.

## Architecture Changes

### Before Refactoring
- All microservices shared a single SQL Server database (`RankUpDB`)
- Single connection string used across all services
- Mixed entity tables in one database

### After Refactoring
- Each microservice has its own dedicated SQL Server database
- Separate connection strings for each service
- Independent EF Core migration history per service
- Complete data isolation between services

## Database Structure

The following databases are now created:

| Service | Database Name | DbContext | Connection String |
|---------|---------------|-----------|-------------------|
| AdminService | `RankUp_AdminDB` | `AdminDbContext` | `AdminServiceConnection` |
| UserService | `RankUp_UserDB` | `UserDbContext` | `UserServiceConnection` |
| ExamService | `RankUp_ExamDB` | `ExamDbContext` | `ExamServiceConnection` |
| QuestionService | `RankUp_QuestionDB` | `QuestionDbContext` | `QuestionServiceConnection` |
| QuizService | `RankUp_QuizDB` | `QuizDbContext` | `QuizServiceConnection` |
| SubscriptionService | `RankUp_SubscriptionDB` | `SubscriptionDbContext` | `SubscriptionServiceConnection` |
| PaymentService | `RankUp_PaymentDB` | `PaymentDbContext` | `PaymentServiceConnection` |

## Configuration Changes

### Connection Strings (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=ABHIJEET;Database=RankUpDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "AdminServiceConnection": "Server=ABHIJEET;Database=RankUp_AdminDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "UserServiceConnection": "Server=ABHIJEET;Database=RankUp_UserDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "ExamServiceConnection": "Server=ABHIJEET;Database=RankUp_ExamDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "QuestionServiceConnection": "Server=ABHIJEET;Database=RankUp_QuestionDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "QuizServiceConnection": "Server=ABHIJEET;Database=RankUp_QuizDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "SubscriptionServiceConnection": "Server=ABHIJEET;Database=RankUp_SubscriptionDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "PaymentServiceConnection": "Server=ABHIJEET;Database=RankUp_PaymentDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Service Configuration Updates

Each service's `Program.cs` has been updated to use its dedicated connection string:

```csharp
builder.Services.AddDbContext<ServiceDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ServiceNameConnection");
    options.UseSqlServer(connectionString, sqlServerOptions =>
    {
        sqlServerOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
        sqlServerOptions.CommandTimeout(60);
        sqlServerOptions.MigrationsAssembly(typeof(ServiceDbContext).Assembly.GetName().Name);
    });
});
```

## Migration Management

### Generating Migrations

Run the provided PowerShell script to generate migrations for all services:

```powershell
.\Generate-Migrations.ps1
```

Or run individually for each service:

```powershell
# AdminService
cd Services/AdminService/AdminService.API
dotnet ef migrations add InitialCreate --project ../AdminService.Infrastructure --context AdminDbContext --startup-project . --output-dir Migrations

# UserService
cd Services/UserService/UserService.API
dotnet ef migrations add InitialCreate --project ../UserService.Infrastructure --context UserDbContext --startup-project . --output-dir Migrations

# ... repeat for other services
```

### Applying Migrations

Run the provided PowerShell script to apply all migrations:

```powershell
.\Update-Databases.ps1
```

Or run individually for each service:

```powershell
# AdminService
cd Services/AdminService/AdminService.API
dotnet ef database update --project ../AdminService.Infrastructure --context AdminDbContext

# UserService
cd Services/UserService/UserService.API
dotnet ef database update --project ../UserService.Infrastructure --context UserDbContext

# ... repeat for other services
```

## Benefits of This Refactoring

### 1. **Data Isolation**
- Each microservice has complete control over its data
- No risk of accidental data modification between services
- Clear ownership boundaries

### 2. **Independent Scaling**
- Databases can be scaled independently based on service load
- Performance tuning can be done per service
- Backup/restore strategies can be service-specific

### 3. **Deployment Independence**
- Services can be deployed without affecting other services' databases
- Database schema changes are isolated to individual services
- Rollback scenarios are simplified

### 4. **Security**
- Access can be granted at database level per service
- Different security policies can be applied per service
- Audit trails are service-specific

### 5. **Development Efficiency**
- Teams can work independently without database conflicts
- Schema changes don't require coordination with other teams
- Testing can be done with isolated datasets

## Inter-Service Communication

Since data is now isolated, services must communicate via:

1. **REST APIs** - For synchronous communication
2. **Message Queues** - For asynchronous communication
3. **Shared Events** - For eventual consistency

## Monitoring and Maintenance

### Database Monitoring
- Monitor each database separately
- Set up alerts per service database
- Track performance metrics individually

### Backup Strategy
- Implement separate backup schedules per service
- Consider different retention policies based on service importance
- Test restore procedures for each database

## Migration Considerations

### Existing Data
If migrating from a single database:

1. **Export Data**: Extract data from the original shared database
2. **Transform**: Map data to the new service-specific schemas
3. **Load**: Import data into the appropriate service databases
4. **Validate**: Ensure data integrity across all services

### Downtime Planning
- Plan for service downtime during migration
- Consider a blue-green deployment approach
- Have rollback procedures ready

## Best Practices

### Development
1. Always use service-specific connection strings in development
2. Generate migrations at the service level, not solution level
3. Test database creation and migration in CI/CD pipelines

### Production
1. Use different connection strings for development/staging/production
2. Implement proper connection pooling per service
3. Monitor database performance and set up appropriate alerts

### Security
1. Use principle of least privilege for database access
2. Implement different credentials for each service
3. Regularly rotate database credentials

## Troubleshooting

### Common Issues

1. **Connection String Errors**
   - Verify connection string names match exactly
   - Check that appsettings.json is properly configured

2. **Migration Failures**
   - Ensure EF Core tools are installed
   - Verify project references are correct
   - Check that DbContext classes are properly configured

3. **Database Creation Issues**
   - Verify SQL Server is running and accessible
   - Check that the user has permissions to create databases
   - Ensure connection string server name is correct

### Debugging Steps

1. Check service logs for database connection errors
2. Verify connection strings in configuration
3. Test database connectivity using SQL Server Management Studio
4. Run migrations manually to see detailed error messages

## Future Enhancements

1. **Database per Tenant**: Consider multi-tenancy within services
2. **Read Replicas**: Implement read replicas for high-traffic services
3. **CQRS**: Separate read/write databases for complex services
4. **Event Sourcing**: Consider event sourcing for audit-heavy services
