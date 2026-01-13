# Microservices Migration Guide

## Current Status

### ✅ Completed Services

1. **ExamService** - Fully implemented
   - Domain layer with Exam and ExamQualification entities
   - Application layer with DTOs, services, and interfaces
   - Infrastructure layer with DbContext and repositories
   - API layer with controllers and configuration

2. **UserService** - Fully implemented
   - Domain layer with User entity and OtpCode value object
   - Application layer with authentication services
   - Infrastructure layer with UserDbContext
   - API layer with auth controllers

3. **AdminService** - Structure created (needs completion)
   - Domain layer with RBAC entities (Admin, Role, Permission, etc.)
   - Application layer structure created
   - Infrastructure and API layers need implementation

### ⏳ Remaining Services

4. **QuizService** - Needs creation
   - Should combine TestSeries, Subject, Chapter functionality
   - Domain: TestSeries, Subject, Chapter entities
   - Migrate from Areas/Admin/TestSeries, Areas/Admin/Subject, Areas/Admin/Chapter

5. **QuestionService** - Needs creation
   - Domain: Question entity
   - Migrate from Areas/Admin/Question

6. **SubscriptionService** - Needs creation
   - Domain: Subscription entity
   - Migrate from Models/Subscription

7. **PaymentService** - Needs creation
   - Domain: Payment entity
   - Migrate from Models/Payment

## Step-by-Step Migration Process

### For Each Remaining Service:

#### Step 1: Create Domain Layer
```bash
# Create project structure
mkdir -p Services/{ServiceName}/{ServiceName}.Domain/Entities
mkdir -p Services/{ServiceName}/{ServiceName}.Domain/ValueObjects

# Create .csproj
# Copy pattern from ExamService.Domain/ExamService.Domain.csproj

# Create BaseEntity.cs
# Copy from ExamService.Domain/Entities/BaseEntity.cs

# Create domain entities
# Move from Models/ to Domain/Entities/
# Remove dependencies on other services' entities
```

#### Step 2: Create Application Layer
```bash
mkdir -p Services/{ServiceName}/{ServiceName}.Application/DTOs
mkdir -p Services/{ServiceName}/{ServiceName}.Application/Interfaces
mkdir -p Services/{ServiceName}/{ServiceName}.Application/Services
mkdir -p Services/{ServiceName}/{ServiceName}.Application/Mappings

# Create DTOs
# Move from Areas/Admin/{Entity}/DTOs/
# Make service-specific (no shared DTOs)

# Create interfaces
# Move repository interfaces from Repositories/Interfaces/
# Create service interfaces

# Create services
# Move from Areas/Admin/{Entity}/Services/
# Remove dependencies on shared repositories
# Use only service-specific repositories

# Create AutoMapper profiles
```

#### Step 3: Create Infrastructure Layer
```bash
mkdir -p Services/{ServiceName}/{ServiceName}.Infrastructure/Data
mkdir -p Services/{ServiceName}/{ServiceName}.Infrastructure/Repositories

# Create DbContext
# Copy pattern from ExamService.Infrastructure/Data/ExamDbContext.cs
# Include only service-specific entities

# Create repositories
# Move from Repositories/Implementations/
# Implement service-specific repository interfaces
# Use service-specific DbContext

# Configure migrations
# Update connection string in appsettings.json
```

#### Step 4: Create API Layer
```bash
mkdir -p Services/{ServiceName}/{ServiceName}.API/Controllers

# Create controllers
# Move from Areas/Admin/{Entity}/Controllers/
# Update routes to /api/{resource}
# Remove Area attributes
# Update dependencies to use Application layer only

# Create Program.cs
# Copy pattern from ExamService.API/Program.cs
# Update service registrations
# Configure DbContext with service-specific connection string

# Create appsettings.json
# Add service-specific connection string
# Configure JWT if needed
```

#### Step 5: Update Solution File
```xml
<!-- Add project references to RankUpAPI.sln -->
```

#### Step 6: Remove Old Code
```bash
# After verification, remove:
# - Areas/Admin/{Entity}/
# - Models/{Entity}.cs (if service-specific)
# - Repositories related to entity
# - Shared services related to entity
```

## Code Migration Examples

### Example 1: Moving Exam Entity

**Before (Shared Model)**:
```csharp
// Models/Exam.cs
public class Exam : BaseEntity
{
    public string Name { get; set; }
    // ...
}
```

**After (Service-Specific Domain)**:
```csharp
// ExamService.Domain/Entities/Exam.cs
namespace ExamService.Domain.Entities
{
    public class Exam : BaseEntity
    {
        public string Name { get; set; }
        // ...
    }
}
```

### Example 2: Moving Repository

**Before (Shared Repository)**:
```csharp
// Repositories/Implementations/ExamRepository.cs
public class ExamRepository : IExamRepository
{
    private readonly ApplicationDbContext _context; // Shared context
    // ...
}
```

**After (Service-Specific Repository)**:
```csharp
// ExamService.Infrastructure/Repositories/ExamRepository.cs
public class ExamRepository : IExamRepository
{
    private readonly ExamDbContext _context; // Service-specific context
    // ...
}
```

### Example 3: Moving Service

**Before (Area-based Service)**:
```csharp
// Areas/Admin/Exam/Services/Implementations/ExamService.cs
public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IQualificationRepository _qualificationRepository; // Cross-service dependency
    // ...
}
```

**After (Service-Specific with HTTP Client)**:
```csharp
// ExamService.Application/Services/ExamService.cs
public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    // Qualification data fetched via HTTP if needed, or stored as ID only
    // ...
}
```

## Inter-Service Communication Patterns

### Pattern 1: Store Foreign Key Only
```csharp
// ExamService.Domain/Entities/ExamQualification.cs
public class ExamQualification : BaseEntity
{
    public int ExamId { get; set; }
    public int QualificationId { get; set; } // Store ID only, not navigation
}
```

### Pattern 2: HTTP Client for Cross-Service Data
```csharp
// AdminService.Application/Interfaces/IUserServiceClient.cs
public interface IUserServiceClient
{
    Task<UserDto?> GetUserByIdAsync(int userId);
}

// AdminService.Application/Clients/UserServiceClient.cs
public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    // Implementation with Polly retry policy
}
```

### Pattern 3: Event-Driven (Future Enhancement)
```csharp
// For async operations, consider event bus
public interface IEventBus
{
    Task PublishAsync<T>(T eventData) where T : class;
}
```

## Database Migration Strategy

### Option 1: Fresh Start (Recommended for Development)
1. Create new databases for each service
2. Run migrations
3. Seed initial data if needed

### Option 2: Data Migration (For Production)
1. Export data from monolith database
2. Transform data to service-specific schemas
3. Import into service databases
4. Verify data integrity

## Testing Strategy

### Unit Tests
- Domain layer: Test business rules
- Application layer: Test use cases with mocked repositories

### Integration Tests
- Test API endpoints
- Test database operations
- Test inter-service communication

### End-to-End Tests
- Test complete workflows across services

## Deployment Checklist

For each service:
- [ ] Database created and migrated
- [ ] Connection string configured
- [ ] JWT authentication configured (if needed)
- [ ] CORS configured
- [ ] Health check endpoint added
- [ ] Logging configured
- [ ] Swagger/OpenAPI documentation
- [ ] Environment-specific appsettings

## Common Issues and Solutions

### Issue 1: Circular Dependencies
**Solution**: Use HTTP clients or events, never direct references

### Issue 2: Shared Entities
**Solution**: Duplicate entities in each service's domain, or use DTOs for communication

### Issue 3: Transaction Management
**Solution**: Use distributed transactions (Saga pattern) or eventual consistency

### Issue 4: Data Consistency
**Solution**: Implement idempotency, use event sourcing, or accept eventual consistency

## Next Steps

1. Complete AdminService implementation
2. Create QuizService, QuestionService, SubscriptionService, PaymentService
3. Set up API Gateway (Ocelot or YARP)
4. Implement service discovery
5. Add distributed tracing
6. Containerize services
7. Set up CI/CD pipelines

## Resources

- Clean Architecture: https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html
- DDD: https://martinfowler.com/bliki/DomainDrivenDesign.html
- Microservices Patterns: https://microservices.io/patterns/
