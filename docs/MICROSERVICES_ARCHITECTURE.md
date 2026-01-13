# RankUpAPI Microservices Architecture

## Overview

This document describes the microservices architecture refactoring from a modular monolith to independent microservices using Clean Architecture and Domain-Driven Design (DDD) principles.

## Solution Structure

```
RankUpAPI/
├── Services/
│   ├── ExamService/
│   │   ├── ExamService.API/
│   │   ├── ExamService.Application/
│   │   ├── ExamService.Domain/
│   │   └── ExamService.Infrastructure/
│   ├── UserService/
│   │   ├── UserService.API/
│   │   ├── UserService.Application/
│   │   ├── UserService.Domain/
│   │   └── UserService.Infrastructure/
│   ├── QuizService/
│   │   ├── QuizService.API/
│   │   ├── QuizService.Application/
│   │   ├── QuizService.Domain/
│   │   └── QuizService.Infrastructure/
│   ├── QuestionService/
│   │   ├── QuestionService.API/
│   │   ├── QuestionService.Application/
│   │   ├── QuestionService.Domain/
│   │   └── QuestionService.Infrastructure/
│   ├── SubscriptionService/
│   │   ├── SubscriptionService.API/
│   │   ├── SubscriptionService.Application/
│   │   ├── SubscriptionService.Domain/
│   │   └── SubscriptionService.Infrastructure/
│   ├── PaymentService/
│   │   ├── PaymentService.API/
│   │   ├── PaymentService.Application/
│   │   ├── PaymentService.Domain/
│   │   └── PaymentService.Infrastructure/
│   └── AdminService/
│       ├── AdminService.API/
│       ├── AdminService.Application/
│       ├── AdminService.Domain/
│       └── AdminService.Infrastructure/
```

## Architecture Layers

### 1. Domain Layer
- **Purpose**: Core business logic and domain entities
- **Contains**:
  - Entities (domain models)
  - Value Objects
  - Domain rules and business logic
- **Dependencies**: None (pure domain logic)

### 2. Application Layer
- **Purpose**: Application use cases and business workflows
- **Contains**:
  - DTOs (service-specific)
  - Interfaces (repositories, external services)
  - Application Services (use cases)
  - Mappings (AutoMapper profiles)
- **Dependencies**: Domain layer only

### 3. Infrastructure Layer
- **Purpose**: Technical implementations
- **Contains**:
  - DbContext (EF Core)
  - Repository implementations
  - External service clients (HTTP clients)
  - Migrations
- **Dependencies**: Application and Domain layers

### 4. API Layer
- **Purpose**: HTTP endpoints and configuration
- **Contains**:
  - Controllers
  - Program.cs (DI configuration)
  - appsettings.json
- **Dependencies**: Application and Infrastructure layers

## Microservices Details

### ExamService
**Responsibility**: Manage exams and their relationships with qualifications

**Domain Entities**:
- Exam
- ExamQualification

**Database**: `ExamServiceDb`

**API Endpoints**:
- `GET /api/exams` - Get all exams
- `GET /api/exams/{id}` - Get exam by ID
- `GET /api/exams/by-qualification/{qualificationId}` - Get exams by qualification
- `POST /api/exams` - Create exam (Admin)
- `PUT /api/exams/{id}` - Update exam (Admin)
- `DELETE /api/exams/{id}` - Delete exam (Admin)
- `PATCH /api/exams/{id}/status` - Toggle exam status (Admin)

### UserService
**Responsibility**: User management and authentication

**Domain Entities**:
- User

**Value Objects**:
- OtpCode

**Database**: `UserServiceDb`

**API Endpoints**:
- `POST /api/users/auth/send-otp` - Send OTP
- `POST /api/users/auth/verify-otp` - Verify OTP and login
- `GET /api/users/auth/profile/{userId}` - Get user profile
- `PUT /api/users/auth/profile/{userId}` - Update user profile

### QuizService
**Responsibility**: Manage test series, subjects, and chapters

**Domain Entities**:
- TestSeries
- Subject
- Chapter

**Database**: `QuizServiceDb`

**Note**: Combines functionality from TestSeries, Subject, and Chapter modules

### QuestionService
**Responsibility**: Manage questions and their relationships

**Domain Entities**:
- Question
- TestSeriesQuestion (if needed)

**Database**: `QuestionServiceDb`

### SubscriptionService
**Responsibility**: Manage user subscriptions

**Domain Entities**:
- Subscription

**Database**: `SubscriptionServiceDb`

### PaymentService
**Responsibility**: Handle payment processing

**Domain Entities**:
- Payment

**Database**: `PaymentServiceDb`

### AdminService
**Responsibility**: Admin management, RBAC, and activity logging

**Domain Entities**:
- Admin
- Role
- Permission
- RolePermission
- AdminRole
- AdminSession
- AdminActivityLog

**Database**: `AdminServiceDb`

**Features**:
- Role-Based Access Control (RBAC)
- Permission management
- Admin activity logging
- Session management

## Inter-Service Communication

### HTTP Client Pattern

Services communicate via HTTP using Typed HttpClient with Polly resilience policies.

**Example: AdminService calling UserService**

```csharp
// In AdminService.Application/Interfaces/IUserServiceClient.cs
public interface IUserServiceClient
{
    Task<UserDto?> GetUserByIdAsync(int userId);
}

// In AdminService.Application/Clients/UserServiceClient.cs
public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var response = await _retryPolicy.ExecuteAsync(async () =>
            await _httpClient.GetAsync($"/api/users/{userId}"));
        
        return await response.Content.ReadFromJsonAsync<UserDto>();
    }
}

// In AdminService.API/Program.cs
builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5002"); // UserService URL
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### Service Discovery

For production, consider:
- API Gateway (Ocelot, YARP)
- Service discovery (Consul, Eureka)
- Load balancing

## Database Isolation

Each service has:
- **Own connection string** in appsettings.json
- **Own DbContext** in Infrastructure layer
- **Own migrations** folder
- **Independent schema**

**Connection String Pattern**:
```json
{
  "ConnectionStrings": {
    "ExamServiceConnection": "Server=...;Database=ExamServiceDb;...",
    "UserServiceConnection": "Server=...;Database=UserServiceDb;...",
    "AdminServiceConnection": "Server=...;Database=AdminServiceDb;..."
  }
}
```

## Authentication & Authorization

### JWT Tokens
- Each service can validate JWT tokens independently
- Shared JWT secret/key across services (or use JWT validation service)
- AdminService manages admin authentication and RBAC

### Authorization
- AdminService provides RBAC
- Other services validate JWT and check roles/permissions via AdminService API

## Migration Strategy

### Phase 1: Create Microservices Structure ✅
- [x] Create solution structure
- [x] Create ExamService (complete)
- [x] Create UserService (complete)
- [x] Create AdminService (structure)

### Phase 2: Complete Remaining Services
- [ ] Create QuizService
- [ ] Create QuestionService
- [ ] Create SubscriptionService
- [ ] Create PaymentService

### Phase 3: Migrate Data
- [ ] Create migration scripts
- [ ] Data migration from monolith to microservices
- [ ] Verify data integrity

### Phase 4: Update Clients
- [ ] Update frontend to call new service endpoints
- [ ] Update API Gateway configuration
- [ ] Update documentation

### Phase 5: Decommission Monolith
- [ ] Remove old Areas/ structure
- [ ] Remove shared Models, Repositories, Services
- [ ] Archive old codebase

## Project Reference Diagram

```
ExamService.API
    ├── ExamService.Application
    │       └── ExamService.Domain
    └── ExamService.Infrastructure
            ├── ExamService.Application
            └── ExamService.Domain

UserService.API
    ├── UserService.Application
    │       └── UserService.Domain
    └── UserService.Infrastructure
            ├── UserService.Application
            └── UserService.Domain

AdminService.API
    ├── AdminService.Application
    │       ├── AdminService.Domain
    │       └── IUserServiceClient (HTTP client interface)
    └── AdminService.Infrastructure
            ├── AdminService.Application
            └── AdminService.Domain
```

## Clean Architecture Compliance

✅ **Dependency Rule**: Dependencies point inward
- Domain: No dependencies
- Application: Depends on Domain only
- Infrastructure: Depends on Application and Domain
- API: Depends on Application and Infrastructure

✅ **Separation of Concerns**: Each layer has a single responsibility

✅ **Independence**: Each service can be deployed independently

✅ **Testability**: Each layer can be tested in isolation

## Validation Checklist

### Independent Deployment
- [x] Each service has its own Program.cs
- [x] Each service has its own appsettings.json
- [x] Each service can run on different ports
- [x] No shared binaries between services

### Database Isolation
- [x] Each service has its own DbContext
- [x] Each service has its own connection string
- [x] Each service has its own migrations
- [x] No cross-service database access

### Code Isolation
- [x] No shared Models between services
- [x] No shared Repositories between services
- [x] No shared Services between services
- [x] DTOs are service-specific

### Inter-Service Communication
- [x] Services communicate via HTTP only
- [x] No direct database access between services
- [x] Resilience patterns (Polly) implemented
- [x] Proper error handling

## Next Steps

1. **Complete Remaining Services**: Create QuizService, QuestionService, SubscriptionService, PaymentService following the same pattern
2. **Add API Gateway**: Implement Ocelot or YARP for routing
3. **Add Service Discovery**: For dynamic service location
4. **Add Distributed Tracing**: For monitoring across services
5. **Add Event Bus**: For async communication (RabbitMQ, Azure Service Bus)
6. **Containerization**: Docker containers for each service
7. **Orchestration**: Kubernetes or Docker Compose for local development

## Example: Complete Service Structure

See `Services/ExamService/` for a complete reference implementation.

## Notes

- **No Shared Code**: Each service is completely independent
- **API Contracts**: Services expose well-defined HTTP APIs
- **Versioning**: Consider API versioning for backward compatibility
- **Documentation**: Each service should have OpenAPI/Swagger documentation
- **Testing**: Unit tests for Domain/Application, Integration tests for API
