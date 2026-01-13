# Quick Reference Guide

## OTP Value Change (बिना Code खोले)

### Step 1: Open Configuration File
- Open `appsettings.json` (production)
- या `appsettings.Development.json` (development)

### Step 2: Find OtpSettings Section
```json
{
  "OtpSettings": {
    "DefaultOtp": "1234",  // यहाँ value change करें
    "ExpirationMinutes": 5,
    "UseRandomOtp": false,
    "OtpLength": 4
  }
}
```

### Step 3: Change Value
```json
{
  "OtpSettings": {
    "DefaultOtp": "5678",  // नई value
    "ExpirationMinutes": 5,
    "UseRandomOtp": false,
    "OtpLength": 4
  }
}
```

### Step 4: Restart Application
- Application restart करें
- **Code change की जरूरत नहीं!**

## Project Structure

```
Core/                    → Core services और configuration
Microservices/          → Microservices documentation
Areas/                  → Area-based controllers
Controllers/            → Root controllers
Services/               → Business services
Models/                 → Domain models
DTOs/                   → Data Transfer Objects
Data/                   → Database context
```

## Important Files

- **OTP Configuration**: `appsettings.json` → `OtpSettings`
- **OTP Service**: `Core/Services/Implementations/OtpService.cs`
- **OTP Interface**: `Core/Services/Interfaces/IOtpService.cs`
- **OTP Settings Class**: `Core/Configuration/OtpSettings.cs`

## API Endpoints (Unchanged)

- `/api/auth/send-otp` ✅
- `/api/auth/verify-otp` ✅
- `/api/identity/auth/send-otp` ✅
- `/api/identity/auth/verify-otp` ✅
- All admin endpoints ✅
- All other endpoints ✅

**Postman collection में कोई change नहीं करना!**

## Configuration Options

### OTP Settings
- `DefaultOtp`: Default OTP value (string)
- `ExpirationMinutes`: OTP expiration time (int)
- `UseRandomOtp`: Use random OTP generation (bool)
- `OtpLength`: Length of random OTP (int)

### JWT Settings
- `Key`: JWT secret key
- `Issuer`: JWT issuer
- `Audience`: JWT audience
- `ExpireInMinutes`: Token expiration time

### Admin Credentials
- `Email`: Admin email
- `Password`: Admin password

## Benefits

✅ OTP value change बिना code खोले  
✅ Centralized configuration  
✅ Microservices ready structure  
✅ Clean architecture  
✅ No breaking changes  
✅ All endpoints work as before  
