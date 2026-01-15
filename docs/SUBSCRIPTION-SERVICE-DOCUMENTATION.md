# SubscriptionService Documentation

## Overview

The SubscriptionService is a comprehensive microservice for managing subscription plans, payments, and user subscriptions in the RankUpAPI EdTech platform. It follows Clean Architecture principles and provides complete subscription lifecycle management.

## Architecture

### Clean Architecture Layers

- **SubscriptionService.API**: Web API layer with controllers and middleware
- **SubscriptionService.Application**: Business logic, DTOs, and service interfaces
- **SubscriptionService.Domain**: Domain entities and interfaces
- **SubscriptionService.Infrastructure**: EF Core implementation, repositories, and external services

### Database

- **Database Name**: RankUp_SubscriptionDB
- **ORM**: Entity Framework Core
- **Migration**: InitialSubscriptionSchema

## Features

### Admin Features (FR-ADM-05)

1. **Plan & Pricing Configuration**
   - Create, update, delete subscription plans
   - Plan types: Monthly, Yearly, Exam-specific
   - Configure price, validity, exam category, features
   - Manage coupons with percentage/flat discounts

2. **User Subscription Oversight**
   - View active/expired/cancelled subscriptions
   - Manual renewal and cancellation
   - Full subscription history per user

### User Features (FR-MA-04)

1. **Exam-Specific Subscription Plans**
   - Monthly and yearly plans
   - Based on exam category

2. **Payment Gateway Integration**
   - Razorpay integration
   - Support for UPI, Card, Net Banking, Wallets
   - PCI-DSS compliant tokenized flow

3. **Free Demo Access**
   - Limited quizzes without subscription
   - One-time demo access per exam category

4. **Billing & Invoicing**
   - Auto invoice generation
   - Downloadable invoices
   - Receipt email integration

5. **Auto-Renewal & Expiry**
   - Razorpay recurring payments
   - Expiry reminders (3 days, 1 day before)
   - Post-expiry renewal prompts

6. **Subscription Validation**
   - Server-side validation on premium API calls
   - Access control based on subscription status

## API Endpoints

### Admin APIs

#### Subscription Plans
- `POST /api/admin/subscription-plans/plans` - Create plan
- `PUT /api/admin/subscription-plans/plans/{id}` - Update plan
- `DELETE /api/admin/subscription-plans/plans/{id}` - Delete plan
- `GET /api/admin/subscription-plans/plans/{id}` - Get plan by ID
- `GET /api/admin/subscription-plans/plans` - Get all plans
- `GET /api/admin/subscription-plans/plans/by-exam/{examCategory}` - Get plans by exam
- `GET /api/admin/subscription-plans/plans/active` - Get active plans

#### Coupons
- `POST /api/admin/coupons` - Create coupon
- `PUT /api/admin/coupons/{id}` - Update coupon
- `PATCH /api/admin/coupons/{id}/disable` - Disable coupon
- `GET /api/admin/coupons/{id}` - Get coupon by ID
- `GET /api/admin/coupons` - Get all coupons
- `GET /api/admin/coupons/active` - Get active coupons

#### User Subscriptions
- `GET /api/admin/user-subscriptions` - Get all user subscriptions
- `GET /api/admin/user-subscriptions/user/{userId}` - Get user subscription history
- `POST /api/admin/user-subscriptions/renew` - Renew user subscription
- `POST /api/admin/user-subscriptions/cancel` - Cancel user subscription
- `GET /api/admin/user-subscriptions/active` - Get active subscriptions
- `GET /api/admin/user-subscriptions/expiring` - Get expiring subscriptions

### User APIs

#### Subscription Plans
- `GET /api/user/user-plans/by-exam/{examCategory}` - Get plans by exam category
- `GET /api/user/user-plans/active` - Get active plans
- `GET /api/user/user-plans/{id}` - Get plan by ID

#### Coupons
- `POST /api/user/user-coupons/apply` - Apply coupon
- `GET /api/user/user-coupons/active` - Get active coupons

#### Payments
- `POST /api/user/payments/create-order` - Create Razorpay order
- `POST /api/user/payments/verify` - Verify payment
- `POST /api/user/payments/refund` - Process refund
- `GET /api/user/payments/history` - Get payment history

#### Subscriptions
- `GET /api/user/subscriptions/my-subscription` - Get my subscription
- `GET /api/user/subscriptions/history` - Get subscription history
- `POST /api/user/subscriptions` - Create subscription
- `POST /api/user/subscriptions/activate` - Activate subscription
- `POST /api/user/subscriptions/cancel` - Cancel subscription

#### Invoices
- `GET /api/user/invoices/download/{subscriptionId}` - Download invoice
- `GET /api/user/invoices/history` - Get invoice history

### System APIs

#### Subscription Validation
- `POST /api/system/subscription-validation/validate` - Validate subscription
- `GET /api/system/subscription-validation/check-active/{userId}` - Check active subscription
- `GET /api/system/subscription-validation/validate-for-service` - Validate for service
- `POST /api/system/subscription-validation/check-demo-eligibility` - Check demo eligibility
- `POST /api/system/subscription-validation/log-demo-access` - Log demo access

#### Health Check
- `GET /api/system/health` - Health check
- `GET /api/system/health/database` - Database connectivity check

## Database Schema

### Tables

1. **SubscriptionPlans**
   - Subscription plan configurations
   - Features stored as comma-separated values
   - Support for monthly, yearly, and exam-specific plans

2. **Coupons**
   - Discount coupons with percentage or flat amounts
   - Usage limits and validity periods
   - Many-to-many relationship with plans

3. **UserSubscriptions**
   - User subscription records
   - Payment integration with Razorpay
   - Auto-renewal support

4. **PaymentTransactions**
   - Payment transaction details
   - Razorpay integration data
   - Refund tracking

5. **Invoices**
   - Invoice generation and management
   - PDF generation support
   - Email integration ready

6. **DemoAccessLogs**
   - Demo access tracking
   - One-time demo enforcement
   - Usage analytics

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "SubscriptionServiceConnection": "Server=localhost;Database=RankUp_SubscriptionDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "Razorpay": {
    "KeyId": "your_razorpay_key_id",
    "KeySecret": "your_razorpay_key_secret",
    "BaseUrl": "https://api.razorpay.com/v1"
  },
  "Jwt": {
    "Key": "your_jwt_secret_key",
    "Issuer": "RankUpAPI",
    "Audience": "RankUpAPI.Users"
  }
}
```

## Sample API Requests

### Create Subscription Plan (Admin)

```json
POST /api/admin/subscription-plans/plans
{
  "name": "SSC MTS Test Papers",
  "description": "Complete test series for SSC MTS exam",
  "type": 1,
  "price": 999.00,
  "validityDays": 90,
  "examCategory": "SSC MTS",
  "features": [
    "50+ Mock Tests",
    "Detailed Analysis",
    "Offline Access",
    "Ad-free Experience"
  ],
  "imageUrl": "https://example.com/ssc-mts.jpg",
  "isPopular": true,
  "sortOrder": 1
}
```

### Create Razorpay Order (User)

```json
POST /api/user/payments/create-order
{
  "planId": 1,
  "userId": 123,
  "couponId": 5,
  "currency": "INR",
  "receipt": "receipt_123456"
}
```

### Verify Payment (User)

```json
POST /api/user/payments/verify
{
  "razorpayOrderId": "order_1234567890",
  "razorpayPaymentId": "pay_1234567890",
  "razorpaySignature": "generated_signature",
  "userId": 123
}
```

### Validate Subscription (System)

```json
POST /api/system/subscription-validation/validate
{
  "userId": 123,
  "examCategory": "SSC MTS"
}
```

## Integration Guide

### For Other Microservices

To validate user subscriptions in other microservices:

```csharp
// Add to your service's Program.cs
builder.Services.AddHttpClient<SubscriptionValidationService>();

// Use in your controllers
[HttpGet("premium-content")]
public async Task<IActionResult> GetPremiumContent([FromServices] SubscriptionValidationService validationService)
{
    var userId = GetUserIdFromToken();
    var isValid = await validationService.ValidateSubscriptionForServiceAsync(userId, "SSC MTS");
    
    if (!isValid)
        return Unauthorized("Active subscription required");
    
    // Return premium content
}
```

### Frontend Integration

1. **Payment Flow**:
   - Create Razorpay order
   - Initialize Razorpay with order details
   - Handle payment success callback
   - Verify payment and activate subscription

2. **Subscription Validation**:
   - Call validation endpoint before premium content
   - Handle expired subscriptions with renewal prompts
   - Display subscription status in UI

## Security

- JWT authentication required for all endpoints
- Role-based access control (Admin/User)
- Razorpay signature verification for payments
- SQL injection prevention through EF Core
- CORS configuration for cross-origin requests

## Monitoring & Logging

- Structured logging with Microsoft.Extensions.Logging
- Health check endpoints for monitoring
- Database connectivity checks
- Payment transaction logging

## Deployment

### Database Migration

```bash
dotnet ef database update --project SubscriptionService.Infrastructure --startup-project SubscriptionService.API --context SubscriptionDbContext
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ConnectionStrings__SubscriptionServiceConnection`: Database connection string
- `Razorpay__KeyId`: Razorpay API key
- `Razorpay__KeySecret`: Razorpay secret key

## Testing

### Unit Tests

- Domain entities and services
- Application service logic
- Repository implementations
- DTO mappings

### Integration Tests

- API endpoints
- Database operations
- Razorpay integration (mocked)
- JWT authentication

## Troubleshooting

### Common Issues

1. **Payment Verification Failed**
   - Check Razorpay credentials
   - Verify signature calculation
   - Ensure order ID matches

2. **Database Connection Issues**
   - Verify connection string
   - Check SQL Server availability
   - Run database migrations

3. **Subscription Validation Fails**
   - Check subscription status
   - Verify expiry dates
   - Ensure user ID is correct

## Future Enhancements

- Webhook support for Razorpay events
- Subscription analytics dashboard
- Advanced coupon rules (first-time user, referral)
- Multi-currency support
- Subscription pause/resume functionality
- Family/Group subscription plans

## Support

For issues and questions:
- Check application logs
- Review API documentation
- Contact development team
- Monitor health check endpoints
