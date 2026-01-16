# UserService Profile Patch API - Postman Collection Guide

## 📋 Overview

This Postman collection provides comprehensive testing for UserService profile update endpoints with both JSON and form-data support including image upload functionality.

## 🚀 Quick Start

### 1. Import Collection
1. Open Postman
2. Click **Import** → **Select Files**
3. Choose `UserService_Profile_Postman_Collection.json`
4. Click **Import**

### 2. Set Environment Variables
The collection uses these variables:
- `authToken`: Automatically set after successful login
- `userId`: User ID (default: 1)

### 3. Run in Order
Execute requests in this sequence:
1. 🔐 **Send OTP** - Generate OTP for login
2. 🔑 **Verify OTP & Get Token** - Get authentication token
3. 👤 **Get User Profile** - View current profile
4. 📝 **Update Profile** - Update profile with JSON
5. 📸 **Update Profile with Image** - Update with form-data + image
6. 🖼️ **Upload Profile Image Only** - Upload just the image
7. 📧 **Update Email Only** - Update only email field
8. 📊 **Test File Upload Validation** - Test invalid file upload

## 📡 API Endpoints

### Authentication Endpoints

#### Send OTP
```
POST http://localhost:5002/api/users/auth/send-otp
Content-Type: application/json

{
    "mobileNumber": "1234567890",
    "countryCode": "+91"
}
```

#### Verify OTP & Get Token
```
POST http://localhost:5002/api/users/auth/verify-otp
Content-Type: application/json

{
    "mobileNumber": "1234567890",
    "countryCode": "+91",
    "otp": "1234"
}
```

### Profile Endpoints

#### Get User Profile
```
GET http://localhost:5002/api/users/auth/profile/{{userId}}
Authorization: Bearer {{authToken}}
```

#### Update Profile (JSON)
```
PATCH http://localhost:5002/api/users/{{userId}}/profile
Authorization: Bearer {{authToken}}
Content-Type: application/json

{
    "FullName": "John Doe Updated",
    "Email": "john.updated@example.com",
    "Gender": "Male",
    "Dob": "1990-01-01",
    "StateId": 1,
    "LanguageId": 1,
    "QualificationId": 1,
    "ExamId": 1
}
```

#### Update Profile with Image (Form-Data)
```
PATCH http://localhost:5002/api/users/{{userId}}/profile-with-image
Authorization: Bearer {{authToken}}
Content-Type: multipart/form-data

Form Data:
- FullName: "John Doe Updated"
- Email: "john.updated@example.com"
- Gender: "Male"
- Dob: "1990-01-01"
- StateId: "1"
- LanguageId: "1"
- QualificationId: "1"
- ExamId: "1"
- ProfilePhoto: [file]
```

## 🖼️ Image Upload Details

### Supported File Formats
- ✅ JPG (.jpg, .jpeg)
- ✅ PNG (.png)
- ✅ GIF (.gif)
- ✅ WEBP (.webp)

### File Size Limit
- 📏 Maximum: 5MB per file

### Storage Path
- 📁 Images stored in: `/wwwroot/uploads/profiles/`
- 🏷️ Naming: `user_{userId}_{timestamp}.{extension}`

### Image URL Format
After upload, profile photo will be accessible at:
```
http://localhost:5002/uploads/profiles/user_1_20260116135900.jpg
```

## 🧪 Testing Scenarios

### 1. Basic Profile Update (JSON)
1. Run "Send OTP" and "Verify OTP" to get token
2. Use "Update Profile (JSON)" with sample data
3. Verify response contains updated fields

### 2. Profile Update with Image (Form-Data)
1. Ensure you have a valid image file ready
2. In Postman, switch to **Form-Data** tab
3. Add text fields and select image file
4. Send request and verify response

### 3. Image Upload Only
1. Use "Upload Profile Image Only" request
2. Select only the ProfilePhoto file
3. Verify response contains profilePhoto URL

### 4. File Validation Testing
1. Try uploading invalid file (e.g., .txt, .pdf)
2. Verify 400 Bad Request response
3. Check error message about invalid file type

## 🔧 Configuration

### UserService Settings
- **Server**: http://localhost:5002 (HTTP) / https://localhost:5003 (HTTPS)
- **Default OTP**: 1234
- **Default Country Code**: +91
- **JWT Expiry**: 60 minutes

### File Upload Settings
- **Max File Size**: 5MB
- **Allowed Extensions**: .jpg, .jpeg, .png, .gif, .webp
- **Upload Directory**: wwwroot/uploads/profiles/

## 📝 Sample Test Data

### User Profile Fields
```json
{
    "FullName": "John Doe",
    "Email": "john.doe@example.com",
    "Gender": "Male",
    "Dob": "1990-01-01",
    "StateId": 1,
    "LanguageId": 1,
    "QualificationId": 1,
    "ExamId": 1
}
```

### Sample Image Files for Testing
- ✅ Valid: `profile.jpg`, `avatar.png`, `photo.webp`
- ❌ Invalid: `document.pdf`, `text.txt`, `video.mp4`

## 🐛 Common Issues & Solutions

### Issue: 401 Unauthorized
**Solution**: Run "Verify OTP & Get Token" first to get fresh token

### Issue: 400 Bad Request for file upload
**Solution**: Check file format and size (must be image < 5MB)

### Issue: 404 Not Found
**Solution**: Ensure UserService is running on port 5002

### Issue: Image not accessible
**Solution**: Check if wwwroot/uploads/profiles directory exists and has proper permissions

## 📊 Response Formats

### Success Response (200 OK)
```json
{
    "id": 1,
    "name": "John Doe Updated",
    "email": "john.updated@example.com",
    "phoneNumber": "1234567890",
    "countryCode": "+91",
    "profilePhoto": "uploads/profiles/user_1_20260116135900.jpg",
    "gender": "Male",
    "dateOfBirth": "1990-01-01T00:00:00",
    "stateId": 1,
    "languageId": 1,
    "qualificationId": 1,
    "examId": 1,
    "lastLoginAt": "2026-01-16T13:58:59.123Z",
    "isPhoneVerified": true,
    "isActive": true,
    "createdAt": "2026-01-16T13:45:55.123Z"
}
```

### Error Response (400 Bad Request)
```json
{
    "success": false,
    "message": "Invalid file type. Only JPG, JPEG, PNG, GIF, and WEBP files are allowed."
}
```

### Error Response (401 Unauthorized)
```json
{
    "error": "Unauthorized"
}
```

## 🎯 Testing Checklist

- [ ] Send OTP works correctly
- [ ] Verify OTP returns valid JWT token
- [ ] Token is stored in collection variables
- [ ] Get profile returns user data
- [ ] JSON patch updates profile fields
- [ ] Form-data patch updates profile with image
- [ ] Image upload works with valid files
- [ ] Invalid file upload returns proper error
- [ ] Profile photo URL is accessible
- [ ] Email field updates correctly

## 📞 Support

If you encounter issues:
1. Ensure UserService is running: `dotnet run` in UserService.API directory
2. Check console logs for error messages
3. Verify file permissions for uploads directory
4. Test with different image formats and sizes

---

**🎉 Happy Testing!**
