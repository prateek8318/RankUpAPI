# 🔐 Microservice Authentication & Token Management Guide

## 📋 Overview (खाकिय)

यह guide आपके microservices के बीच में secure communication के लिए है। जब एक user login करता है तो उसे JWT token मिलता है, और services आपसे में आपसे communicate करती हैं।

## 🎯 Problem Statement (समस्या)

आपके questions:
- "sb connect ho gye n aur sbka endpoints kya rhega"
- "aur token kaise manage kroe isme ho rha ya nhi"

## 🔧 Solution Implemented (हल)

### 1. **JWT Token Management**

#### **User Login Flow:**
```
User Login → UserService → JWT Token Generated → Token Stored in Client
```

#### **Service-to-Service Communication:**
```
Service A → Generate Service Token → Call Service B → Validate Token → Process Request
```

### 2. **Created Components**

#### **A. Token Management Classes:**
- `ServiceToken` - Token information store करने के लिए
- `ServiceRequest` - Inter-service requests के लिए
- `ServiceAuthResponse` - Authentication responses के लिए

#### **B. Authentication Service:**
- `AuthService` - Token validation और generation
- User token validation (UserService से)
- Service token generation और validation
- Role-based access control

#### **C. HTTP Clients:**
- `UserServiceClient` - Already existed
- `ExamServiceClient` - नया created
- Retry policies और error handling
- Proper logging

#### **D. Middleware:**
- `ServiceAuthMiddleware` - Request-level authentication
- Bearer token validation
- Service token validation
- Proper error responses

## 🚀 How It Works (कैसे काम)

### **For User Requests:**
1. User login करता है UserService में
2. JWT token मिलता जाता है
3. User request करता है किसी भी service में
4. Middleware token validate करता है
5. UserService से user data validate करता है
6. Request proceed करता है अगर validation

### **For Service-to-Service Requests:**
1. Service A अपना token generate करता है
2. Service B को request भेजता है token के साथ
3. Service B token validate करता है
4. Request process करता है
5. Response भेजता है

## 📝 Configuration (सेटअप)

### **Service URLs Added:**
```json
{
  "Services": {
    "UserService": "https://localhost:5002",
    "ExamService": "https://localhost:5003", 
    "QuestionService": "https://localhost:5004",
    "QuizService": "https://localhost:5005",
    "SubscriptionService": "https://localhost:5006",
    "PaymentService": "https://localhost:5007"
  }
}
```

### **Connection Strings:**
```json
{
  "ConnectionStrings": {
    "AdminServiceConnection": "Server=ABHIJEET;Database=RankUp_AdminDB;",
    "UserServiceConnection": "Server=ABHIJEET;Database=RankUp_UserDB;",
    // ... बाकी services
  }
}
```

## 🔨 Usage Examples (उपयोग)

### **1. User Token Validation:**
```csharp
// AdminService में
var authService = serviceProvider.GetService<AuthService>();
var isValid = await authService.ValidateUserTokenAsync(userToken);

if (isValid)
{
    // User authenticated - proceed with request
}
else
{
    // Return 401 Unauthorized
}
```

### **2. Service-to-Service Call:**
```csharp
// AdminService से ExamService call करना
var examClient = serviceProvider.GetService<IExamServiceClient>();
var exam = await examClient.GetExamByIdAsync(examId);

// Client automatically handles:
// - Service token generation
// - Request headers
// - Retry logic
// - Error handling
```

### **3. Middleware Integration:**
```csharp
// Program.cs में
app.UseMiddleware<ServiceAuthMiddleware>();

// Automatically validates:
// - User tokens (via UserService)
// - Service tokens (internal)
// - Proper error responses
```

## 🛡️ Security Features (सुरक्षा)

### **Token Security:**
- HMAC SHA256 signing
- 24-hour expiry for service tokens
- Role-based access control
- Token blacklisting capability

### **Communication Security:**
- HTTPS required for all service calls
- Service-to-service authentication
- Request/response logging
- Retry mechanisms with backoff

### **Data Validation:**
- User data validation across services
- Role verification
- Token expiry checks
- Proper error handling

## 🔄 Next Steps (अगले चरण)

### **1. Implement in Other Services:**
- UserService में token generation endpoint
- ExamService में authentication middleware
- सभी services में similar pattern

### **2. Token Storage:**
- Redis में token caching
- Token refresh mechanism
- Blacklist management
- Revocation handling

### **3. Monitoring:**
- Authentication success/failure logging
- Service call metrics
- Performance monitoring
- Security alerts

## 📞 Troubleshooting (समस्या हल)

### **Common Issues:**
1. **"Token not working"** → Check service URLs
2. **"401 Unauthorized"** → Validate token format
3. **"Service not reachable"** → Check network configuration
4. **"Database connection failed"** → Verify connection strings

### **Debug Steps:**
1. Check appsettings.json configuration
2. Verify service URLs are accessible
3. Check token generation logic
4. Validate middleware registration
5. Review service logs

## ✅ Benefits (लाभ)

### **Security:**
- Centralized authentication
- Consistent token validation
- Role-based access control
- Audit trail

### **Scalability:**
- Independent service scaling
- Load balancing ready
- Fault tolerance
- Easy service addition

### **Maintainability:**
- Standardized authentication
- Reusable components
- Clear separation of concerns
- Easy testing

## 🎯 Summary (सारांश)

अब आपके microservices में:
- ✅ **JWT Token Management** setup
- ✅ **Service-to-Service Authentication** implemented  
- ✅ **HTTP Clients** for communication
- ✅ **Middleware** for request validation
- ✅ **Configuration** for service URLs
- ✅ **Security** best practices applied

अब आप secure inter-service communication के लिए completely ready हैं! 🚀
