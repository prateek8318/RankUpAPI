# Admin Microservice

This microservice handles:
- Admin authentication
- Admin operations
- Content management (Subjects, Chapters, Questions, TestSeries)
- Home content management

## Controllers
- `Areas/Admin/Controllers/AdminController.cs` - Admin operations
- `Areas/Admin/Controllers/AuthController.cs` - Admin authentication
- `Areas/Admin/Controllers/SubjectController.cs` - Subject management
- `Areas/Admin/Controllers/ChapterController.cs` - Chapter management
- `Areas/Admin/Controllers/QuestionController.cs` - Question management
- `Areas/Admin/Controllers/TestSeriesController.cs` - TestSeries management
- `Areas/Admin/Controllers/HomeContentController.cs` - Home content management

## Services
- `Areas/Admin/Services/IAdminAuthService` - Admin authentication
- `Services/SubjectService` - Subject operations
- `Services/ChapterService` - Chapter operations
- `Services/QuestionService` - Question operations
- `Services/TestSeriesService` - TestSeries operations
- `Services/HomeContentService` - Home content operations
