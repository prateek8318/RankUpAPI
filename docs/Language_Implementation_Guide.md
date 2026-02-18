# Language Implementation Guide

## Overview
This guide explains the implementation of a consistent language key system across the .NET microservices architecture.

## Architecture Components

### 1. Common Language Infrastructure

#### Location: `Common/Language/`
- **LanguageConstants.cs** - Defines supported languages (en, hi, ta, gu) and constants
- **LanguageValidator.cs** - Validation and normalization logic
- **LanguageValidationAttribute.cs** - Data annotation for model validation

#### Location: `Common/Middleware/`
- **LanguageMiddleware.cs** - Extracts language from Accept-Language header and stores in HttpContext

#### Location: `Common/Services/`
- **LanguageService.cs** - Service to access current language throughout the application

#### Location: `Common/HttpClient/`
- **LanguageHeaderHandler.cs** - Automatically forwards Accept-Language header in inter-service calls

### 2. API Gateway Implementation

#### Location: `Services/GatewayAPI/`
- **Middleware/LanguageValidationMiddleware.cs** - Validates Accept-Language header at gateway level
- **Program.cs** - Registers language validation middleware
- **ocelot.json** - Updated with DownstreamHeaderTransform for all routes

### 3. Database Schema Changes

#### User Service
```sql
-- Update Users table
ALTER TABLE Users 
DROP COLUMN LanguagePreference;

ALTER TABLE Users 
ADD PreferredLanguage NVARCHAR(5) NULL;

-- Add validation constraint for supported languages
ALTER TABLE Users 
ADD CONSTRAINT CHK_User_PreferredLanguage 
CHECK (PreferredLanguage IN ('en', 'hi', 'ta', 'gu') OR PreferredLanguage IS NULL);
```

#### Exam Service
```sql
-- New ExamSessions table
CREATE TABLE ExamSessions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ExamId INT NOT NULL,
    ExamLanguage NVARCHAR(5) NOT NULL DEFAULT 'en',
    StartTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    EndTime DATETIME2 NULL,
    DurationInMinutes INT NOT NULL DEFAULT 60,
    LastPauseTime DATETIME2 NULL,
    TotalPauseDuration INT NOT NULL DEFAULT 0,
    IsCompleted BIT NOT NULL DEFAULT 0,
    IsPaused BIT NOT NULL DEFAULT 0,
    CurrentQuestionIndex INT NOT NULL DEFAULT 0,
    Score INT NOT NULL DEFAULT 0,
    CorrectAnswers INT NOT NULL DEFAULT 0,
    IncorrectAnswers INT NOT NULL DEFAULT 0,
    SkippedQuestions INT NOT NULL DEFAULT 0,
    Notes NVARCHAR(1000) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NULL,
    UpdatedBy INT NULL,
    
    CONSTRAINT CHK_ExamSession_ExamLanguage 
    CHECK (ExamLanguage IN ('en', 'hi', 'ta', 'gu'))
);

-- New ExamAnswers table
CREATE TABLE ExamAnswers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ExamSessionId INT NOT NULL,
    QuestionId INT NOT NULL,
    SelectedOptionId INT NULL,
    TextAnswer NVARCHAR(2000) NULL,
    IsCorrect BIT NOT NULL DEFAULT 0,
    PointsEarned INT NOT NULL DEFAULT 0,
    TimeSpentSeconds INT NOT NULL DEFAULT 0,
    AnsweredAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsMarkedForReview BIT NOT NULL DEFAULT 0,
    IsSkipped BIT NOT NULL DEFAULT 0,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedBy INT NULL,
    UpdatedBy INT NULL,
    
    FOREIGN KEY (ExamSessionId) REFERENCES ExamSessions(Id),
    FOREIGN KEY (QuestionId) REFERENCES Questions(Id),
    FOREIGN KEY (SelectedOptionId) REFERENCES QuestionOptions(Id)
);
```

## Implementation Steps

### 1. Middleware Registration
Add to each service's `Program.cs`:
```csharp
using Common.Middleware;
using Common.Services;
using Common.HttpClient;

// Register services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ILanguageService, LanguageService>();

// Add language header handler for inter-service calls
builder.Services.AddTransient<LanguageHeaderHandler>();
builder.Services.AddHttpClient("InternalServices")
    .AddHttpMessageHandler<LanguageHeaderHandler>();

// Add middleware to pipeline
app.UseLanguage();
```

### 2. Language Usage in Services

#### Getting Current Language
```csharp
public class QuestionService
{
    private readonly ILanguageService _languageService;
    
    public QuestionService(ILanguageService languageService)
    {
        _languageService = languageService;
    }
    
    public async Task<QuestionDto> GetQuestionAsync(int questionId)
    {
        var language = _languageService.GetCurrentLanguage();
        // Use language to fetch translated content
    }
}
```

#### Exam Session Language Lock
```csharp
public class ExamSessionService
{
    public async Task<ExamSession> StartExamAsync(int userId, int examId, string language)
    {
        // Validate language
        LanguageValidator.ValidateLanguage(language);
        
        // Create session with locked language
        var session = new ExamSession
        {
            UserId = userId,
            ExamId = examId,
            ExamLanguage = language // This language is locked for entire exam lifecycle
        };
        
        return await _repository.AddAsync(session);
    }
    
    public async Task<QuestionDto> GetExamQuestionAsync(int sessionId, int questionId)
    {
        var session = await _repository.GetByIdAsync(sessionId);
        // Always use exam session language, not user preference
        var language = session.ExamLanguage;
        
        return await _questionService.GetQuestionAsync(questionId, language);
    }
}
```

### 3. API Usage

#### Frontend Request Headers
```javascript
// Set language header for all requests
axios.defaults.headers.common['Accept-Language'] = 'hi';

// Or per request
const response = await axios.get('/api/exams/1', {
  headers: {
    'Accept-Language': 'ta'
  }
});
```

#### Validation Responses
Invalid language returns:
```json
{
  "error": "Invalid language header",
  "message": "Supported languages: en, hi, ta, gu",
  "supportedLanguages": ["en", "hi", "ta", "gu"]
}
```

## Common Pitfalls to Avoid

### 1. Language Inconsistency
- **Problem**: Using user profile language instead of exam session language
- **Solution**: Always use `ExamSession.ExamLanguage` for exam-related operations

### 2. Missing Header Forwarding
- **Problem**: Inter-service calls don't include Accept-Language header
- **Solution**: Use `LanguageHeaderHandler` with named HttpClient

### 3. Invalid Language Values
- **Problem**: Accepting invalid language codes
- **Solution**: Always validate using `LanguageValidator.ValidateLanguage()`

### 4. Database Constraint Issues
- **Problem**: Database allows invalid language values
- **Solution**: Add CHECK constraints for all language columns

### 5. Race Conditions in Exams
- **Problem**: Language changes after exam starts
- **Solution**: Lock language in ExamSession entity and validate on each request

### 6. Missing Middleware Registration
- **Problem**: Language not available in HttpContext
- **Solution**: Ensure `app.UseLanguage()` is called before `app.UseAuthentication()`

## Testing Checklist

### 1. API Gateway Tests
- [ ] Invalid language header returns 400
- [ ] Valid language header is forwarded to services
- [ ] Missing language header defaults to 'en'

### 2. Service Tests
- [ ] Language middleware extracts header correctly
- [ ] LanguageService returns correct language
- [ ] Inter-service calls include Accept-Language header

### 3. Exam Session Tests
- [ ] Language is locked when exam starts
- [ ] Language cannot be changed after exam starts
- [ ] Resume exam loads in same language
- [ ] Reports generate in exam language

### 4. Database Tests
- [ ] Language constraints prevent invalid values
- [ ] User profile updates validate language
- [ ] Exam session creation validates language

## Migration Strategy

### Phase 1: Infrastructure
1. Deploy common language components
2. Update API Gateway with validation
3. Add database constraints

### Phase 2: Service Updates
1. Update UserService with new User entity
2. Update ExamService with ExamSession entity
3. Register middleware in all services

### Phase 3: Client Integration
1. Update frontend to send Accept-Language header
2. Test language consistency across flows
3. Monitor for validation errors

### Phase 4: Cleanup
1. Remove old LanguagePreference column
2. Update documentation
3. Add monitoring for language-related errors
