# API Response Standardization Guide

## Overview
All UserService API endpoints now return standardized, self-explanatory responses with specific messages for each HTTP status code.

## Response Structure

### Success Response (200 OK)
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { ... }, // Response data
  "timestamp": "2026-01-19T07:30:00.000Z"
}
```

### Error Response (400, 404, 401, 500)
```json
{
  "success": false,
  "message": "Specific error message explaining what went wrong",
  "errorCode": "ERROR_CODE", // Machine-readable error code
  "timestamp": "2026-01-19T07:30:00.000Z"
}
```

## HTTP Status Codes & Messages

### 200 OK - Success Messages
- **OTP Sent**: `"OTP sent successfully to +919876543210"`
- **Login**: `"Login successful! Welcome back."`
- **Profile Retrieved**: `"User profile retrieved successfully"`
- **Profile Updated**: `"User profile updated successfully"`
- **Profile with Photo Updated**: `"User profile with photo updated successfully"`
- **Logout**: `"Logout successful. Please discard the token from client side."`
- **Users List**: `"Retrieved 25 users successfully"`
- **User Retrieved**: `"User with ID 123 retrieved successfully"`
- **Count Retrieved**: `"Total users count retrieved successfully"`

### 400 Bad Request - Validation Errors
- **Invalid Mobile**: `"Please provide a valid 10-digit mobile number"`
  - Error Code: `INVALID_MOBILE_NUMBER`
- **Invalid Country Code**: `"Please provide a valid country code (e.g., +91, +1, +44)"`
  - Error Code: `INVALID_COUNTRY_CODE`
- **Missing Fields**: `"Mobile number and OTP are required for verification"`
  - Error Code: `MISSING_REQUIRED_FIELDS`
- **Invalid OTP**: `"The OTP you entered is invalid or has expired. Please try again."`
  - Error Code: `INVALID_OTP`
- **Invalid File**: `[File-specific error message]`
  - Error Code: `INVALID_FILE_FORMAT`

### 404 Not Found - Resource Missing
- **User Not Found**: `"User profile with ID 123 was not found"`
  - Error Code: `USER_NOT_FOUND`
- **Profile Not Found**: `"User profile not found for update"`
  - Error Code: `USER_NOT_FOUND`

### 401 Unauthorized - Authentication Issues
- **Invalid Token**: `"Authentication token is invalid or expired"`
  - Error Code: `INVALID_TOKEN`
- **Access Denied**: `"You don't have permission to access this resource"`
  - Error Code: `ACCESS_DENIED`

### 500 Internal Server Error - System Issues
- **Database Error**: `"Unable to process your request. Please try again later."`
  - Error Code: `DATABASE_ERROR`
- **File Upload Error**: `"Unable to process file upload. Please try again."`
  - Error Code: `FILE_UPLOAD_ERROR`
- **General Server Error**: `"An unexpected error occurred. Please try again later."`
  - Error Code: `INTERNAL_SERVER_ERROR`

## Error Codes Reference

### Validation Errors (400)
- `INVALID_MOBILE_NUMBER` - Mobile number format invalid
- `INVALID_COUNTRY_CODE` - Country code format invalid
- `MISSING_REQUIRED_FIELDS` - Required parameters missing
- `INVALID_OTP` - OTP is invalid or expired
- `EXPIRED_OTP` - OTP has expired
- `INVALID_FILE_FORMAT` - Uploaded file format not supported
- `FILE_TOO_LARGE` - File size exceeds limit
- `INVALID_EMAIL_FORMAT` - Email format invalid
- `DUPLICATE_EMAIL` - Email already exists
- `DUPLICATE_PHONE` - Phone number already exists

### Not Found Errors (404)
- `USER_NOT_FOUND` - User with specified ID not found
- `PROFILE_NOT_FOUND` - User profile not found
- `RESOURCE_NOT_FOUND` - General resource not found

### Unauthorized Errors (401)
- `INVALID_TOKEN` - JWT token is invalid
- `TOKEN_EXPIRED` - JWT token has expired
- `ACCESS_DENIED` - User lacks required permissions

### Server Errors (500)
- `DATABASE_ERROR` - Database operation failed
- `FILE_UPLOAD_ERROR` - File upload processing failed
- `EXTERNAL_SERVICE_ERROR` - External service call failed
- `INTERNAL_SERVER_ERROR` - General server error

## Endpoint Examples

### POST /api/users/auth/send-otp
**Request:**
```json
{
  "mobileNumber": "9876543210",
  "countryCode": "+91"
}
```

**Success Response (200):**
```json
{
  "success": true,
  "message": "OTP sent successfully to +919876543210",
  "data": {
    "phoneNumber": "+919876543210",
    "otpHint": "Use default OTP: 1234"
  },
  "timestamp": "2026-01-19T07:30:00.000Z"
}
```

**Error Response (400):**
```json
{
  "success": false,
  "message": "Please provide a valid 10-digit mobile number",
  "errorCode": "INVALID_MOBILE_NUMBER",
  "timestamp": "2026-01-19T07:30:00.000Z"
}
```

### GET /api/users/profile
**Success Response (200):**
```json
{
  "success": true,
  "message": "User profile retrieved successfully",
  "data": {
    "id": 123,
    "name": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "9876543210",
    "countryCode": "+91",
    "isActive": true,
    "isPhoneVerified": true
  },
  "timestamp": "2026-01-19T07:30:00.000Z"
}
```

**Error Response (404):**
```json
{
  "success": false,
  "message": "User profile with ID 123 was not found",
  "errorCode": "USER_NOT_FOUND",
  "timestamp": "2026-01-19T07:30:00.000Z"
}
```

## Benefits for Frontend & Postman

1. **Self-Explanatory**: Each response clearly indicates success or failure reason
2. **Machine-Readable**: Error codes allow programmatic error handling
3. **Consistent Format**: All endpoints follow the same response structure
4. **Timestamp**: Every response includes when it was generated
5. **Actionable Messages**: Messages guide users on what to do next

## Implementation Details

- **Generic Response Class**: `ApiResponse<T>` for typed responses
- **Static Factory Methods**: `CreateSuccess()`, `CreateBadRequest()`, etc.
- **Error Codes**: Centralized in `ErrorCodes` static class
- **Timestamp**: Automatically added to all responses
- **Logging**: Detailed error logging for debugging

This standardization ensures that Postman tests and frontend applications can easily understand and handle API responses without ambiguity.
