# Subscription API Endpoints Summary

## USER ENDPOINTS

### 1. Get Current Subscription
**Endpoint:** `GET /api/user/subscriptions/my-subscription`
**Auth:** Required
**Request Body:** None
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

### 2. Get Subscription History
**Endpoint:** `GET /api/user/subscriptions/history`
**Auth:** Required
**Request Body:** None
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

### 3. Create Subscription
**Endpoint:** `POST /api/user/subscriptions`
**Auth:** Required
**Request Body:**
```json
{
  "planId": 2,
  "paymentMethod": "Razorpay"
}
```
**Response:** Created subscription details

### 4. Activate Subscription
**Endpoint:** `POST /api/user/subscriptions/activate`
**Auth:** Required
**Request Body:**
```json
{
  "razorpayOrderId": "order_123",
  "razorpayPaymentId": "pay_123",
  "razorpaySignature": "signature_123",
  "planId": 2
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

### 5. Cancel Subscription
**Endpoint:** `POST /api/user/subscriptions/cancel`
**Auth:** Required
**Request Body:**
```json
{
  "subscriptionId": 1,
  "cancellationReason": "No longer needed"
}
```
**Response:** `true/false`

### 6. Create Razorpay Order
**Endpoint:** `POST /api/user/payments/create-order`
**Auth:** Required
**Request Body:**
```json
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

### 7. Verify Payment
**Endpoint:** `POST /api/user/payments/verify`
**Auth:** Required
**Request Body:**
```json
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

### 8. Get Payment History
**Endpoint:** `GET /api/user/payments/history`
**Auth:** Required
**Request Body:** None
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

## ADMIN ENDPOINTS

### SUBSCRIPTION MANAGEMENT

### 1. Get All User Subscriptions
**Endpoint:** `GET /api/admin/user-subscriptions`
**Auth:** Admin Required
**Request Body:** None
**Response:** Array of user subscriptions

### 2. Get Paginated User Subscriptions
**Endpoint:** `GET /api/admin/admin-subscription/user-subscriptions`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20&userId=123&status=Active
```
**Response:** Paginated subscriptions list

### 3. Get User Subscription by ID
**Endpoint:** `GET /api/admin/admin-subscription/user-subscriptions/{id}`
**Auth:** Admin Required
**Request Body:** None
**Response:** Single subscription details

### 4. Get User Subscription History
**Endpoint:** `GET /api/admin/user-subscriptions/user/{userId}`
**Auth:** Admin Required
**Request Body:** None
**Response:** User subscription history

### 5. Get Active Subscriptions
**Endpoint:** `GET /api/admin/user-subscriptions/active`
**Auth:** Admin Required
**Request Body:** None
**Response:** Active subscriptions list

### 6. Get Expiring Subscriptions
**Endpoint:** `GET /api/admin/user-subscriptions/expiring?daysBeforeExpiry=7`
**Auth:** Admin Required
**Request Body:** None
**Response:** Expiring subscriptions list

### 7. Cancel User Subscription (Admin)
**Endpoint:** `POST /api/admin/admin-subscription/user-subscriptions/cancel`
**Auth:** Admin Required
**Request Body:**
```json
{
  "subscriptionId": 1,
  "cancellationReason": "Admin cancellation"
}
```
**Response:** No content (204)

### 8. Extend User Subscription
**Endpoint:** `POST /api/admin/admin-subscription/user-subscriptions/extend`
**Auth:** Admin Required
**Request Body:**
```json
{
  "subscriptionId": 1,
  "days": 30
}
```
**Response:** Updated subscription details

### PLAN MANAGEMENT

### 9. Get All Plans (Paginated)
**Endpoint:** `GET /api/admin/admin-subscription/plans`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20&isActive=true
```
**Response:** Paginated plans list

### 10. Create Subscription Plan
**Endpoint:** `POST /api/admin/admin-subscription/plans`
**Auth:** Admin Required
**Request Body:**
```json
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
**Response:** Created plan details

### 11. Get Plan by ID
**Endpoint:** `GET /api/admin/admin-subscription/plans/{id}`
**Auth:** Admin Required
**Request Body:** None
**Response:** Single plan details

### 12. Update Subscription Plan
**Endpoint:** `PUT /api/admin/admin-subscription/plans/{id}`
**Auth:** Admin Required
**Request Body:**
```json
{
  "name": "Updated Premium Plan",
  "price": 1199.00,
  "features": ["Feature 1", "Feature 2", "Feature 3"]
}
```
**Response:** Updated plan details

### 13. Delete Subscription Plan
**Endpoint:** `DELETE /api/admin/admin-subscription/plans/{id}`
**Auth:** Admin Required
**Request Body:** None
**Response:** No content (204)

### 14. Toggle Plan Status
**Endpoint:** `PUT /api/admin/admin-subscription/plans/toggle-status`
**Auth:** Admin Required
**Request Body:**
```json
{
  "planId": 1,
  "isActive": false
}
```
**Response:** No content (204)

### 15. Update Plan Popularity
**Endpoint:** `PUT /api/admin/admin-subscription/plans/update-popularity`
**Auth:** Admin Required
**Request Body:**
```json
{
  "planId": 1,
  "isPopular": true,
  "isRecommended": true
}
```
**Response:** No content (204)

### 16. Bulk Update Plans
**Endpoint:** `PUT /api/admin/admin-subscription/plans/bulk-update`
**Auth:** Admin Required
**Request Body:**
```json
{
  "planIds": [1, 2, 3],
  "updates": {
    "isActive": true,
    "price": 999.00
  }
}
```
**Response:** No content (204)

### PAYMENT MANAGEMENT

### 17. Get Payment Statistics
**Endpoint:** `GET /api/admin/payments/statistics`
**Auth:** Admin Required
**Request Body:** None
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
  "uniquePayingUsers": 120
}
```

### 18. Get All Payments (Paginated)
**Endpoint:** `GET /api/admin/payments`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20&status=Successful&userId=123
```
**Response:** Paginated payments list

### 19. Get Payment by ID
**Endpoint:** `GET /api/admin/payments/{id}`
**Auth:** Admin Required
**Request Body:** None
**Response:** Single payment details

### 20. Get Payments by User ID
**Endpoint:** `GET /api/admin/payments/user/{userId}`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20
```
**Response:** User payments list

### 21. Process Refund (Admin)
**Endpoint:** `POST /api/admin/payments/refund`
**Auth:** Admin Required
**Request Body:**
```json
{
  "paymentId": "pay_123",
  "amount": 99900,
  "reason": "Service issue"
}
```
**Response:** Refund response details

### 22. Update Payment Status
**Endpoint:** `PUT /api/admin/payments/update-status`
**Auth:** Admin Required
**Request Body:**
```json
{
  "paymentId": 1,
  "status": "Successful",
  "notes": "Manually verified",
  "updatedBy": "admin@example.com"
}
```
**Response:** Updated payment details

### 23. Get Failed Payments
**Endpoint:** `GET /api/admin/payments/failed`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20
```
**Response:** Failed payments list

### 24. Get Pending Payments
**Endpoint:** `GET /api/admin/payments/pending`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20
```
**Response:** Pending payments list

### 25. Get Successful Payments
**Endpoint:** `GET /api/admin/payments/successful`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20
```
**Response:** Successful payments list

### 26. Get Refunded Payments
**Endpoint:** `GET /api/admin/payments/refunded`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
pageNumber=1&pageSize=20
```
**Response:** Refunded payments list

### 27. Get Payments by Date Range
**Endpoint:** `GET /api/admin/payments/date-range`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
startDate=2024-01-01&endDate=2024-01-31&pageNumber=1&pageSize=20
```
**Response:** Payments in date range

### 28. Export Payments to CSV
**Endpoint:** `POST /api/admin/payments/export`
**Auth:** Admin Required
**Request Body:**
```json
{
  "status": "Successful",
  "createdDateFrom": "2024-01-01",
  "createdDateTo": "2024-01-31"
}
```
**Response:** CSV file download

### 29. Get Payment Analytics
**Endpoint:** `GET /api/admin/payments/analytics`
**Auth:** Admin Required
**Request Body:** Query Parameters
```
days=30
```
**Response:**
```json
{
  "totalRevenue": 29970.00,
  "totalTransactions": 30,
  "averageTransactionValue": 999.00,
  "dailyRevenue": [...],
  "paymentMethods": [...],
  "paymentStatuses": [...]
}
```

---

## QUICK REFERENCE

### User Endpoints (8 total)
- **Subscription:** GET my-subscription, GET history, POST create, POST activate, POST cancel
- **Payment:** POST create-order, POST verify, GET history

### Admin Endpoints (29 total)
- **Subscription:** 8 endpoints for user subscription management
- **Plans:** 8 endpoints for plan CRUD and bulk operations
- **Payments:** 13 endpoints for payment management, analytics, and exports

### Authentication
- All endpoints require JWT token in `Authorization: Bearer {token}` header
- Admin endpoints require `Admin` or `SuperAdmin` role

### Common Response Formats
- Success: 200 OK with data
- Created: 201 Created with new resource
- No Content: 204 No Content for delete/update operations
- Error: 400/401/403/404/500 with error details
