# Project Restructuring Plan - Entity-Based Folder Structure

## Current Status
✅ Exam entity - COMPLETED
- Areas/Admin/Exam/Controllers/ExamController.cs
- Areas/Admin/Exam/Services/Interfaces/IExamService.cs
- Areas/Admin/Exam/Services/Implementations/ExamService.cs
- Areas/Admin/Exam/DTOs/ExamDto.cs
- Areas/Admin/Exam/Repositories/Interfaces/IExamRepository.cs
- Areas/Admin/Exam/Repositories/Implementations/ExamRepository.cs

## Remaining Entities to Restructure

### 1. Qualification
- Areas/Admin/Qualification/Controllers/QualificationController.cs
- Areas/Admin/Qualification/Services/Interfaces/IQualificationService.cs
- Areas/Admin/Qualification/Services/Implementations/QualificationService.cs
- Areas/Admin/Qualification/DTOs/QualificationDto.cs

### 2. Subject
- Areas/Admin/Subject/Controllers/SubjectController.cs (move from Areas/Admin/Controllers)
- Areas/Admin/Subject/Services/Interfaces/ISubjectService.cs
- Areas/Admin/Subject/Services/Implementations/SubjectService.cs
- Areas/Admin/Subject/DTOs/SubjectDto.cs

### 3. Chapter
- Areas/Admin/Chapter/Controllers/ChapterController.cs (move from Areas/Admin/Controllers)
- Areas/Admin/Chapter/Services/Interfaces/IChapterService.cs
- Areas/Admin/Chapter/Services/Implementations/ChapterService.cs
- Areas/Admin/Chapter/DTOs/ChapterDto.cs

### 4. Question
- Areas/Admin/Question/Controllers/QuestionController.cs (move from Areas/Admin/Controllers)
- Areas/Admin/Question/Services/Interfaces/IQuestionService.cs
- Areas/Admin/Question/Services/Implementations/QuestionService.cs
- Areas/Admin/Question/DTOs/QuestionDto.cs

### 5. TestSeries
- Areas/Admin/TestSeries/Controllers/TestSeriesController.cs (move from Areas/Admin/Controllers)
- Areas/Admin/TestSeries/Services/Interfaces/ITestSeriesService.cs
- Areas/Admin/TestSeries/Services/Implementations/TestSeriesService.cs
- Areas/Admin/TestSeries/DTOs/TestSeriesDto.cs

### 6. HomeContent
- Areas/Admin/HomeContent/Controllers/HomeContentController.cs (move from Areas/Admin/Controllers)
- Areas/Admin/HomeContent/Services/Interfaces/IHomeContentService.cs
- Areas/Admin/HomeContent/Services/Implementations/HomeContentService.cs
- Areas/Admin/HomeContent/DTOs/HomeContentDtos.cs

## Notes
- Repositories remain global (shared infrastructure)
- Models remain global (shared domain entities)
- Namespaces will be updated: RankUpAPI.Areas.Admin.{Entity}.{Layer}
- Program.cs needs to register new services
- AutoMapper needs to be updated for new DTO namespaces
- Old files can be removed after verification
