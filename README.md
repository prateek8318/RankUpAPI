# RankUpAPI - Comprehensive Exam Preparation Platform

A microservices-based API platform for competitive exam preparation with comprehensive test management, user analytics, and administrative features.

## 🚀 Features

### Core Services
- **Admin Service** - Administrative operations and analytics
- **User Service** - User management, profiles, and authentication
- **Exam Service** - National and International exam management
- **Question Service** - Question bank management
- **Quiz Service** - Interactive quiz functionality
- **Test Service** - Comprehensive test execution and analytics
- **Home Dashboard Service** - Unified dashboard experience
- **Subscription Service** - Payment and subscription management
- **API Gateway** - Centralized routing with Ocelot

### Key Features
- 🎯 **Multi-Exam Support** - National (JEE, NEET, UPSC) and International (SAT, IELTS, TOEFL) exams
- 📊 **Comprehensive Testing** - Practice tests, mock tests, and full-length exams
- 👥 **User Management** - Profile management, image uploads, and preferences
- 📈 **Analytics & Reporting** - Performance tracking and detailed analytics
- 🔐 **Secure Authentication** - JWT-based authentication with role-based access
- 🌐 **Microservices Architecture** - Scalable and maintainable service-oriented design
- 📱 **API Gateway** - Unified entry point with load balancing
- 💳 **Subscription Management** - Payment integration and subscription tiers

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   API Gateway   │────│   Admin Service │────│   User Service  │
│     (Ocelot)    │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ├───────────────────────┼───────────────────────┤
         │                       │                       │
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Exam Service  │    │ Question Service│    │  Quiz Service   │
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
         │                       │                       │
         ├───────────────────────┼───────────────────────┤
         │                       │                       │
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│  Test Service   │    │ Analytics Svc   │    │ Subscription Svc│
│                 │    │                 │    │                 │
└─────────────────┘    └─────────────────┘    └─────────────────┘
```

## 🛠️ Technology Stack

- **.NET 8** - Core framework
- **ASP.NET Core Web API** - RESTful APIs
- **Entity Framework Core** - ORM with SQL Server
- **SQL Server** - Primary database
- **JWT Authentication** - Secure token-based auth
- **Ocelot** - API Gateway
- **AutoMapper** - Object mapping
- **Swagger/OpenAPI** - API documentation
- **Docker** - Containerization support

## 📋 Prerequisites

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **SQL Server** - Local or SQL Server Express
- **Visual Studio 2022** or **VS Code** - IDE

## 🚀 Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/prateek8318/RankUpAPI.git
cd RankUpAPI
```

### 2. Database Setup
```bash
# Update connection strings in appsettings.json files
# Run database migrations
.\update-all-databases.bat
```

### 3. Start All Services
```bash
# Start all services with gateway
.\start-all-services-with-gateway.bat

# Or start individually
.\start-all-services.bat
```

## 🌐 API Endpoints

### Gateway Base URL
- **Development**: `http://localhost:5087`
- **API Documentation**: `http://localhost:5087/swagger`

### Key Endpoints

#### Authentication
```
POST /api/auth/login
POST /api/auth/register
POST /api/auth/refresh-token
```

#### User Management
```
GET /api/users/profile
PUT /api/users/profile
POST /api/users/upload-image
```

#### Exams & Tests
```
GET /api/exams
GET /api/tests
POST /api/tests/{id}/execute
GET /api/tests/{id}/results
```

#### Administration
```
GET /api/admin/analytics
POST /api/admin/users
PUT /api/admin/exams
```

## 📊 Available Exams

### National Exams
- **JEE Main & Advanced** - Engineering entrance
- **NEET** - Medical entrance
- **UPSC Civil Services** - Administrative services
- **SSC CGL** - Staff selection commission
- **Banking PO** - Banking sector exams
- **Railway NTPC** - Railway recruitment
- **Teaching CTET** - Teacher eligibility

### International Exams
- **SAT** - College admission test
- **IELTS Academic** - English proficiency
- **TOEFL** - English proficiency
- **GRE** - Graduate school admission
- **GMAT** - Business school admission
- **PLAB** - Medical registration (UK)
- **OET** - Occupational English test

## 🔧 Configuration

### Environment Variables
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RankUpAPI;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "YourSecretKeyHere",
    "Issuer": "RankUpAPI",
    "Audience": "RankUpAPI"
  }
}
```

### Service Ports
- **API Gateway**: 5087
- **Admin Service**: 5002
- **User Service**: 5003
- **Exam Service**: 5000/5001 (HTTP/HTTPS)
- **Question Service**: 5008
- **Quiz Service**: 5009
- **Test Service**: 56928
- **Subscription Service**: 7070

## 📝 API Documentation

### Swagger Documentation
Each service provides comprehensive Swagger documentation:
- **Gateway**: `http://localhost:5087/swagger`
- **Individual Services**: `http://localhost:{port}/swagger`

### Postman Collections
Check the `docs/` folder for ready-to-use Postman collections:
- `TestService_Postman_Collection.json`
- Sample data files for bulk uploads

## 🧪 Testing

### Run Tests
```bash
dotnet test
```

### Seed Test Data
```bash
# Sample test data available in
docs/Sample_Test_Bulk_Upload.csv
```

## 📁 Project Structure

```
RankUpAPI/
├── Services/
│   ├── AdminService/          # Administrative operations
│   ├── UserService/           # User management
│   ├── ExamService/           # Exam management
│   ├── QuestionService/       # Question bank
│   ├── QuizService/           # Quiz functionality
│   ├── TestService/           # Test execution & analytics
│   ├── HomeDashboardService/  # Dashboard service
│   ├── SubscriptionService/   # Payment & subscriptions
│   └── GatewayAPI/            # API Gateway (Ocelot)
├── docs/                      # Documentation & collections
├── Scripts/                   # Database scripts
└── wwwroot/                   # Static files
```

## 🔐 Authentication & Authorization

### JWT Token Structure
```json
{
  "sub": "admin@rankup.com",
  "jti": "unique-token-id",
  "role": "Admin",
  "email": "admin@rankup.com",
  "adminId": "1",
  "exp": 1234567890,
  "iss": "RankUpAPI",
  "aud": "RankUpAPI"
}
```

### Role-Based Access
- **Admin** - Full administrative access
- **User** - Limited user access
- **Subscriber** - Premium features access

## 🚀 Deployment

### Docker Deployment
```bash
# Build all services
docker-compose build

# Run all services
docker-compose up -d
```

### Production Considerations
- Configure HTTPS certificates
- Set up proper database connections
- Configure CORS policies
- Set up monitoring and logging
- Configure load balancing

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Support

For support and queries:
<<<<<<< HEAD
- 📧 Email: support@rankup.com
=======
- 📧 Email: prateekpandey2580@gmail.com
>>>>>>> bde97e23815154a52a681be00a5f18b8a6c91da8
- 🐛 Issues: [GitHub Issues](https://github.com/prateek8318/RankUpAPI/issues)
- 📖 Documentation: [Wiki](https://github.com/prateek8318/RankUpAPI/wiki)

## 🙏 Acknowledgments

- All contributors who have helped shape this project
- The open-source community for the amazing tools and libraries
- Our users for their valuable feedback and suggestions

---

**Built with ❤️ for students preparing for competitive exams**
