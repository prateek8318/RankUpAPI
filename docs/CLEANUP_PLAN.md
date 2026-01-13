# Cleanup Plan - Removing Shared Dependencies

## Overview
After migrating to microservices, the following shared dependencies need to be removed as they violate microservices architecture principles.

## Folders to Remove

### 1. Models/ (Shared Domain Entities)
**Status**: ✅ Remove
**Reason**: Each microservice now has its own domain entities
**Replacement**: Entities are now in `Services/{ServiceName}/{ServiceName}.Domain/Entities/`

### 2. Repositories/ (Shared Repositories)
**Status**: ✅ Remove
**Reason**: Each microservice has its own repositories
**Replacement**: Repositories are now in `Services/{ServiceName}/{ServiceName}.Infrastructure/Repositories/`

### 3. Services/ (Shared Business Services)
**Status**: ✅ Remove
**Reason**: Each microservice has its own application services
**Replacement**: Services are now in `Services/{ServiceName}/{ServiceName}.Application/Services/`

### 4. Data/ (Shared DbContext)
**Status**: ✅ Remove
**Reason**: Each microservice has its own DbContext
**Replacement**: DbContexts are now in `Services/{ServiceName}/{ServiceName}.Infrastructure/Data/`

### 5. DTOs/ (Shared DTOs)
**Status**: ✅ Remove
**Reason**: Each microservice has its own DTOs
**Replacement**: DTOs are now in `Services/{ServiceName}/{ServiceName}.Application/DTOs/`

### 6. Areas/ (Old Monolith Structure)
**Status**: ✅ Remove
**Reason**: Functionality migrated to microservices
**Replacement**: Controllers are now in `Services/{ServiceName}/{ServiceName}.API/Controllers/`

### 7. Controllers/ (Root Controllers)
**Status**: ✅ Remove
**Reason**: Controllers moved to microservices
**Replacement**: Controllers are now in respective microservices

### 8. Mappings/ (Shared AutoMapper Profiles)
**Status**: ✅ Remove
**Reason**: Each microservice has its own mappings
**Replacement**: Mappings are now in `Services/{ServiceName}/{ServiceName}.Application/Mappings/`

### 9. Core/ (Shared Core Services)
**Status**: ⚠️ Review First
**Reason**: May contain OTP service that's now in UserService
**Action**: Check if anything is still needed

### 10. Migrations/ (Old Monolith Migrations)
**Status**: ⚠️ Archive, Don't Delete
**Reason**: May be needed for data migration
**Action**: Move to `Archive/Migrations/` folder

## Files to Update/Remove

### Program.cs
**Status**: ⚠️ Review
**Action**: This is the old monolith entry point. Consider:
- Removing it if not needed
- Or converting it to an API Gateway
- Or keeping it as a reference

### OTPLoginAPI.csproj
**Status**: ⚠️ Review
**Action**: This is the old monolith project file. Consider removing or archiving.

## Migration Status

All functionality has been migrated to microservices:
- ✅ ExamService - Complete
- ✅ UserService - Complete (includes OTP)
- ✅ AdminService - Complete
- ✅ QuizService - Complete
- ✅ QuestionService - Complete
- ✅ SubscriptionService - Complete
- ✅ PaymentService - Complete

## Execution Order

1. ✅ Verify all microservices are working
2. ✅ Create this cleanup plan
3. ⏳ Archive old migrations
4. ⏳ Remove shared folders
5. ⏳ Update/remove Program.cs
6. ⏳ Clean up project file references

## Notes

- Keep documentation files (README, MD files)
- Keep wwwroot if it contains static assets
- Archive old code before deletion for reference
