# RankUp API - Project Structure

## Overview
This project follows a microservices-oriented architecture with proper folder structure and centralized configuration.

## Folder Structure

```
RankUpAPI/
├── Core/                          # Core/shared functionality
│   ├── Configuration/            # Configuration classes
│   │   └── OtpSettings.cs        # OTP configuration (can be changed in appsettings.json)
│   └── Services/                 # Core services
│       ├── Interfaces/           # Service interfaces
│       │   └── IOtpService.cs   # OTP service interface
│       └── Implementations/      # Service implementations
│           └── OtpService.cs     # OTP service implementation
│
├── Microservices/                # Microservices documentation
│   ├── Identity/                 # Identity microservice
│   ├── Admin/                    # Admin microservice
│   ├── Content/                  # Content microservice
│   └── Users/                    # Users microservice
│
├── Areas/                        # Area-based organization
│   ├── Admin/                    # Admin area
│   │   ├── Controllers/          # Admin controllers
│   │   ├── Models/               # Admin models
│   │   └── Services/             # Admin services
│   ├── Identity/                 # Identity area
│   │   ├── Controllers/          # Identity controllers
│   │   └── Models/               # Identity models
│   └── Users/                    # Users area
│       └── Controllers/          # User controllers
│
├── Controllers/                  # Root-level controllers
│   ├── AuthController.cs         # Legacy auth controller
│   ├── ExamsController.cs        # Exams controller
│   ├── HomeContentController.cs  # Home content controller
│   └── QualificationsController.cs # Qualifications controller
│
├── Services/                     # Business services
│   ├── Interfaces/               # Service interfaces
│   └── [Service implementations]
│
├── Models/                       # Domain models
├── DTOs/                         # Data Transfer Objects
├── Data/                         # Data access layer
│   └── ApplicationDbContext.cs  # EF Core DbContext
├── Mappings/                     # AutoMapper profiles
├── Migrations/                   # EF Core migrations
└── wwwroot/                      # Static files
```

## Configuration

### OTP Configuration
OTP settings are now centralized in `appsettings.json`:

```json
{
  "OtpSettings": {
    "DefaultOtp": "1234",           // Change this value without code changes
    "ExpirationMinutes": 5,         // OTP expiration time
    "UseRandomOtp": false,          // Set to true for random OTP generation
    "OtpLength": 4                  // Length of random OTP
  }
}
```

**To change OTP value:**
1. Open `appsettings.json` or `appsettings.Development.json`
2. Change the `DefaultOtp` value in `OtpSettings` section
3. Restart the application
4. No code changes required!

### JWT Configuration
JWT settings are in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "your-256-bit-secret-key-must-be-at-least-32-characters-long",
    "Issuer": "https://localhost:7001",
    "Audience": "https://localhost:7001",
    "ExpireInMinutes": 60
  }
}
```

### Admin Credentials
Admin credentials are in `appsettings.json`:

```json
{
  "AdminCredentials": {
    "Email": "admin@rankup.com",
    "Password": "Admin@123"
  }
}
```

## Microservices Structure

### Identity Microservice
- Handles user authentication
- OTP generation and verification
- JWT token management
- User profile management

### Admin Microservice
- Admin authentication
- Content management (Subjects, Chapters, Questions, TestSeries)
- Home content management

### Content Microservice
- Qualifications management
- Exams management
- Home content for users

### Users Microservice
- User profile management
- User operations

## Key Features

1. **Centralized Configuration**: All static values (OTP, JWT, Admin credentials) are in `appsettings.json`
2. **Microservices Architecture**: Organized into logical microservices
3. **Dependency Injection**: All services registered in `Program.cs`
4. **Clean Architecture**: Separation of concerns with proper folder structure

## API Endpoints

All endpoints remain the same as before. No breaking changes to Postman collection.

- `/api/auth/*` - User authentication
- `/api/identity/auth/*` - Identity authentication
- `/api/admin/*` - Admin operations
- `/api/qualifications/*` - Qualifications
- `/api/exams/*` - Exams
- `/api/homecontent/*` - Home content

## Development

1. Update OTP value in `appsettings.json` without code changes
2. All services are registered in `Program.cs`
3. Use dependency injection for all services
4. Follow microservices structure for new features
