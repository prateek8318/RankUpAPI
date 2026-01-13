# Identity Microservice

This microservice handles:
- User authentication
- OTP generation and verification
- JWT token management
- User profile management

## Controllers
- `Areas/Identity/Controllers/AuthController.cs` - User authentication endpoints
- `Controllers/AuthController.cs` - Legacy auth endpoints

## Services
- `Core/Services/IOtpService` - OTP management
- `Services/UserService` - User management
