# Subscription Service API Documentation

## Overview
This document provides comprehensive API endpoints for subscription and payment management for both users and administrators.

## Base URLs
- User APIs: `/api/user/`
- Admin APIs: `/api/admin/`

## Authentication
All endpoints require JWT token authentication. Admin endpoints require `Admin` or `SuperAdmin` role.

---

## User Endpoints

### Subscription Management

#### Get Current User's Active Subscription
```http
GET /api/user/subscriptions/my-subscription
Authorization: Bearer {token}
```

**Response:**
```json
{
  "id": 1,
  "userId": 123,
  "planId": 2,
  "planName": "Premium Plan",
  "startDate": "2024-01-01T00:00:00Z",
  "endDate": "2024-02-01T00:00:00Z",
  "status": "Active",
  "autoRenew": true,
  "remainingDays": 15
}
```

#### Get Subscription History
```http
GET /api/user/subscriptions/history
Authorization: Bearer {token}
```

**Response:**
```json
{
  "currentSubscription": {
    "id": 1,
    "planName": "Premium Plan",
    "status": "Active"
  },
  "pastSubscriptions": [
    {
      "id": 2,
      "planName": "Basic Plan",
      "startDate": "2023-12-01T00:00:00Z",
      "endDate": "2024-01-01T00:00:00Z",
      "status": "Expired"
    }
  ],
  "totalSubscriptions": 2
}
```

#### Create New Subscription
```http
POST /api/user/subscriptions
Authorization: Bearer {token}
Content-Type: application/json

{
  "planId": 2,
  "paymentMethod": "Razorpay"
}
```

#### Activate Subscription (After Payment)
```http
POST /api/user/subscriptions/activate
Authorization: Bearer {token}
Content-Type: application/json

{
  "razorpayOrderId": "order_123",
  "razorpayPaymentId": "pay_123",
  "razorpaySignature": "signature_123",
  "planId": 2
}
```

#### Cancel Subscription
```http
POST /api/user/subscriptions/cancel
Authorization: Bearer {token}
Content-Type: application/json

{
  "subscriptionId": 1,
  "cancellationReason": "No longer needed"
}
```

### Payment Management

#### Create Razorpay Order
```http
POST /api/user/payments/create-order
Authorization: Bearer {token}
Content-Type: application/json

{
  "planId": 2,
  "currency": "INR"
}
```

**Response:**
```json
{
  "id": "order_123",
  "entity": "order",
  "amount": 99900,
  "currency": "INR",
  "receipt": "receipt_123",
  "status": "created"
}
```

#### Verify Payment
```http
POST /api/user/payments/verify
Authorization: Bearer {token}
Content-Type: application/json

{
  "razorpayOrderId": "order_123",
  "razorpayPaymentId": "pay_123",
  "razorpaySignature": "signature_123"
}
```

**Response:**
```json
{
  "success": true,
  "subscriptionId": 1,
  "message": "Payment verified and subscription activated"
}
```

#### Process Refund
```http
POST /api/user/payments/refund
Authorization: Bearer {token}
Content-Type: application/json

{
  "paymentId": "pay_123",
  "amount": 99900,
  "reason": "Requested by customer"
}
```

#### Get Payment History
```http
GET /api/user/payments/history
Authorization: Bearer {token}
```

**Response:**
```json
[
  {
    "id": 1,
    "transactionId": "txn_123",
    "amount": 999.00,
    "currency": "INR",
    "status": "Successful",
    "paymentMethod": "Razorpay",
    "createdAt": "2024-01-01T10:00:00Z",
    "completedAt": "2024-01-01T10:05:00Z"
  }
]
```

---

## Admin Endpoints

### Subscription Management

#### Get All User Subscriptions
```http
GET /api/admin/user-subscriptions
Authorization: Bearer {admin-token}
```

#### Get Paginated User Subscriptions
```http
GET /api/admin/admin-subscription/user-subscriptions?pageNumber=1&pageSize=20&userId=123&status=Active
Authorization: Bearer {admin-token}
```

#### Get User Subscription by ID
```http
GET /api/admin/admin-subscription/user-subscriptions/{id}
Authorization: Bearer {admin-token}
```

#### Get User Subscription History
```http
GET /api/admin/user-subscriptions/user/{userId}
Authorization: Bearer {admin-token}
```

#### Get Active Subscriptions
```http
GET /api/admin/user-subscriptions/active
Authorization: Bearer {admin-token}
```

#### Get Expiring Subscriptions
```http
GET /api/admin/user-subscriptions/expiring?daysBeforeExpiry=7
Authorization: Bearer {admin-token}
```

#### Cancel User Subscription (Admin)
```http
POST /api/admin/admin-subscription/user-subscriptions/cancel
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "subscriptionId": 1,
  "cancellationReason": "Admin cancellation"
}
```

#### Extend User Subscription
```http
POST /api/admin/admin-subscription/user-subscriptions/extend
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "subscriptionId": 1,
  "days": 30
}
```

### Subscription Plan Management

#### Get All Plans (Paginated)
```http
GET /api/admin/admin-subscription/plans?pageNumber=1&pageSize=20&isActive=true
Authorization: Bearer {admin-token}
```

#### Create Subscription Plan
```http
POST /api/admin/admin-subscription/plans
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "Premium Plan",
  "description": "Premium subscription with all features",
  "price": 999.00,
  "duration": 30,
  "features": ["Feature 1", "Feature 2"],
  "isActive": true,
  "isPopular": true,
  "isRecommended": false
}
```

#### Get Plan by ID
```http
GET /api/admin/admin-subscription/plans/{id}
Authorization: Bearer {admin-token}
```

#### Update Subscription Plan
```http
PUT /api/admin/admin-subscription/plans/{id}
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "name": "Updated Premium Plan",
  "price": 1199.00,
  "features": ["Feature 1", "Feature 2", "Feature 3"]
}
```

#### Delete Subscription Plan (Soft Delete)
```http
DELETE /api/admin/admin-subscription/plans/{id}
Authorization: Bearer {admin-token}
```

#### Toggle Plan Status
```http
PUT /api/admin/admin-subscription/plans/toggle-status
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "planId": 1,
  "isActive": false
}
```

#### Update Plan Popularity
```http
PUT /api/admin/admin-subscription/plans/update-popularity
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "planId": 1,
  "isPopular": true,
  "isRecommended": true
}
```

#### Bulk Update Plans
```http
PUT /api/admin/admin-subscription/plans/bulk-update
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "planIds": [1, 2, 3],
  "updates": {
    "isActive": true,
    "price": 999.00
  }
}
```

### Payment Management

#### Get Payment Statistics
```http
GET /api/admin/payments/statistics
Authorization: Bearer {admin-token}
```

**Response:**
```json
{
  "totalPayments": 150,
  "totalRevenue": 149850.00,
  "todayRevenue": 999.00,
  "thisMonthRevenue": 29970.00,
  "successfulPayments": 145,
  "failedPayments": 3,
  "pendingPayments": 2,
  "averageTransactionAmount": 999.00,
  "uniquePayingUsers": 120,
  "paymentsByMethod": [
    {
      "paymentMethod": "Razorpay",
      "count": 140,
      "amount": 139860.00,
      "percentage": 93.33
    }
  ],
  "dailyRevenue": [
    {
      "date": "2024-01-01",
      "revenue": 999.00,
      "transactionCount": 1
    }
  ]
}
```

#### Get All Payments (Paginated)
```http
GET /api/admin/payments?pageNumber=1&pageSize=20&status=Successful&userId=123
Authorization: Bearer {admin-token}
```

#### Get Payment by ID
```http
GET /api/admin/payments/{id}
Authorization: Bearer {admin-token}
```

#### Get Payments by User ID
```http
GET /api/admin/payments/user/{userId}?pageNumber=1&pageSize=20
Authorization: Bearer {admin-token}
```

#### Process Refund (Admin)
```http
POST /api/admin/payments/refund
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "paymentId": "pay_123",
  "amount": 99900,
  "reason": "Service issue"
}
```

#### Update Payment Status
```http
PUT /api/admin/payments/update-status
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "paymentId": 1,
  "status": "Successful",
  "notes": "Manually verified",
  "updatedBy": "admin@example.com"
}
```

#### Get Failed Payments
```http
GET /api/admin/payments/failed?pageNumber=1&pageSize=20
Authorization: Bearer {admin-token}
```

#### Get Pending Payments
```http
GET /api/admin/payments/pending?pageNumber=1&pageSize=20
Authorization: Bearer {admin-token}
```

#### Get Successful Payments
```http
GET /api/admin/payments/successful?pageNumber=1&pageSize=20
Authorization: Bearer {admin-token}
```

#### Get Refunded Payments
```http
GET /api/admin/payments/refunded?pageNumber=1&pageSize=20
Authorization: Bearer {admin-token}
```

#### Get Payments by Date Range
```http
GET /api/admin/payments/date-range?startDate=2024-01-01&endDate=2024-01-31&pageNumber=1&pageSize=20
Authorization: Bearer {admin-token}
```

#### Export Payments to CSV
```http
POST /api/admin/payments/export
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "status": "Successful",
  "createdDateFrom": "2024-01-01",
  "createdDateTo": "2024-01-31"
}
```

#### Get Payment Analytics
```http
GET /api/admin/payments/analytics?days=30
Authorization: Bearer {admin-token}
```

**Response:**
```json
{
  "totalRevenue": 29970.00,
  "totalTransactions": 30,
  "averageTransactionValue": 999.00,
  "dailyRevenue": [
    {
      "date": "2024-01-01",
      "revenue": 999.00,
      "transactionCount": 1
    }
  ],
  "paymentMethods": [
    {
      "paymentMethod": "Razorpay",
      "count": 28,
      "amount": 27972.00,
      "percentage": 93.33
    }
  ],
  "paymentStatuses": [
    {
      "status": "Successful",
      "count": 28,
      "amount": 27972.00,
      "percentage": 93.33
    }
  ]
}
```

---

## Error Responses

### Standard Error Format
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input data",
    "details": [
      {
        "field": "planId",
        "message": "Plan ID is required"
      }
    ]
  }
}
```

### Common Error Codes
- `UNAUTHORIZED` - Invalid or missing authentication token
- `FORBIDDEN` - Insufficient permissions
- `NOT_FOUND` - Resource not found
- `VALIDATION_ERROR` - Input validation failed
- `INTERNAL_ERROR` - Server error

---

## Data Models

### PaymentStatus Enum
```json
{
  "Pending": 0,
  "Successful": 1,
  "Failed": 2,
  "Refunded": 3,
  "Cancelled": 4
}
```

### PaymentMethod Enum
```json
{
  "Razorpay": 0,
  "Stripe": 1,
  "PayPal": 2,
  "BankTransfer": 3
}
```

### SubscriptionStatus Enum
```json
{
  "Active": 0,
  "Expired": 1,
  "Cancelled": 2,
  "Suspended": 3
}
```

---

## Rate Limiting
- User endpoints: 100 requests per minute
- Admin endpoints: 200 requests per minute

## Webhooks
### Payment Status Webhook
```http
POST /webhook/payment-status
Content-Type: application/json

{
  "event": "payment.completed",
  "data": {
    "paymentId": "pay_123",
    "status": "Successful",
    "amount": 99900,
    "userId": 123
  }
}
```

### Subscription Expiry Webhook
```http
POST /webhook/subscription-expiry
Content-Type: application/json

{
  "event": "subscription.expired",
  "data": {
    "subscriptionId": 1,
    "userId": 123,
    "planId": 2,
    "expiredAt": "2024-02-01T00:00:00Z"
  }
}
```

---

## Testing Examples

### Postman Collection Variables
```json
{
  "baseUrl": "https://api.example.com",
  "userToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "adminToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "userId": "123",
  "planId": "2"
}
```

### cURL Examples

#### Create Subscription
```bash
curl -X POST "https://api.example.com/api/user/subscriptions" \
  -H "Authorization: Bearer {{userToken}}" \
  -H "Content-Type: application/json" \
  -d '{
    "planId": {{planId}},
    "paymentMethod": "Razorpay"
  }'
```

#### Get Payment Statistics (Admin)
```bash
curl -X GET "https://api.example.com/api/admin/payments/statistics" \
  -H "Authorization: Bearer {{adminToken}}"
```

---

## Notes
- All dates are in UTC format (ISO 8601)
- Amounts are in the smallest currency unit (e.g., paise for INR)
- Pagination starts from page 1
- Maximum page size is 100
- All endpoints return JSON responses
- Webhook URLs should be configured in the admin panel
