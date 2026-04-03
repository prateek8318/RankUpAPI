USE [RankUp_MasterDB]
GO
/****** Object:  StoredProcedure [dbo].[User_GetTotalCount]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[User_GetTotalCount]
GO
/****** Object:  StoredProcedure [dbo].[User_GetDailyActiveCount]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[User_GetDailyActiveCount]
GO
/****** Object:  StoredProcedure [dbo].[User_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[User_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_Update]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetBySubjectIds]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_GetBySubjectIds]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetBySubjectId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_GetById]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_Delete]
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[SubjectLanguage_Create]
GO
/****** Object:  StoredProcedure [dbo].[Subject_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_Update]
GO
/****** Object:  StoredProcedure [dbo].[Subject_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_ToggleStatus]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetByStreamId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetByStreamId]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetByIdByLanguage]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetByIdByLanguage]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetAllByLanguage]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetAllByLanguage]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetActiveByLanguage]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetActiveByLanguage]
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[Subject_Exists]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_Exists]
GO
/****** Object:  StoredProcedure [dbo].[Subject_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Subject_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Subject_Create]
GO
/****** Object:  StoredProcedure [dbo].[StreamLanguage_GetByStreamIds]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[StreamLanguage_GetByStreamIds]
GO
/****** Object:  StoredProcedure [dbo].[Stream_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_Update]
GO
/****** Object:  StoredProcedure [dbo].[Stream_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_SoftDelete]
GO
/****** Object:  StoredProcedure [dbo].[Stream_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_SetActive]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetByIdLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAllWithData]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetAllWithData]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAllSimple]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetAllSimple]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAllDetailed]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetAllDetailed]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetActiveLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActiveByQualificationIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetActiveByQualificationIdLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActiveByQualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetActiveByQualificationId]
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[Stream_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Stream_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Stream_Create]
GO
/****** Object:  StoredProcedure [dbo].[StateLanguage_GetByStateIds]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[StateLanguage_GetByStateIds]
GO
/****** Object:  StoredProcedure [dbo].[State_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_Update]
GO
/****** Object:  StoredProcedure [dbo].[State_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_ToggleStatus]
GO
/****** Object:  StoredProcedure [dbo].[State_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_SoftDelete]
GO
/****** Object:  StoredProcedure [dbo].[State_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_SetActive]
GO
/****** Object:  StoredProcedure [dbo].[State_GetWithEmptyNames]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetWithEmptyNames]
GO
/****** Object:  StoredProcedure [dbo].[State_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetByIdLocalized]
GO
/****** Object:  StoredProcedure [dbo].[State_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetById]
GO
/****** Object:  StoredProcedure [dbo].[State_GetByCountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetByCountryCode]
GO
/****** Object:  StoredProcedure [dbo].[State_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[State_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetActiveLocalized]
GO
/****** Object:  StoredProcedure [dbo].[State_GetActiveByCountryCodeLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetActiveByCountryCodeLocalized]
GO
/****** Object:  StoredProcedure [dbo].[State_GetActiveByCountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetActiveByCountryCode]
GO
/****** Object:  StoredProcedure [dbo].[State_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[State_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_Delete]
GO
/****** Object:  StoredProcedure [dbo].[State_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[State_Create]
GO
/****** Object:  StoredProcedure [dbo].[QualificationLanguage_GetByQualificationIds]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[QualificationLanguage_GetByQualificationIds]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_Update]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_SoftDelete]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_SetActive]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_HasRelatedStreams]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_HasRelatedStreams]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_HardDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_HardDelete]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetByIds]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetByIds]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetByIdLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetActiveLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActiveByCountryCodeLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetActiveByCountryCodeLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActiveByCountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetActiveByCountryCode]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Qualification_Create]
GO
/****** Object:  StoredProcedure [dbo].[Language_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Language_Update]
GO
/****** Object:  StoredProcedure [dbo].[Language_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Language_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Language_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Language_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Language_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Language_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[Language_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Language_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Language_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Language_Create]
GO
/****** Object:  StoredProcedure [dbo].[ExamQualification_GetByExamId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamQualification_GetByExamId]
GO
/****** Object:  StoredProcedure [dbo].[ExamQualification_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamQualification_Delete]
GO
/****** Object:  StoredProcedure [dbo].[ExamQualification_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamQualification_Create]
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamLanguage_Update]
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_GetByLanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamLanguage_GetByLanguageId]
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_GetByExamId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamLanguage_GetByExamId]
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamLanguage_Delete]
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[ExamLanguage_Create]
GO
/****** Object:  StoredProcedure [dbo].[Exam_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_Update]
GO
/****** Object:  StoredProcedure [dbo].[Exam_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_SoftDelete]
GO
/****** Object:  StoredProcedure [dbo].[Exam_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_SetActive]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdWithRelationsLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByIdWithRelationsLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdWithRelations]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByIdWithRelations]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdWithQualifications]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByIdWithQualifications]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByIdLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilterWithLanguagesLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByFilterWithLanguagesLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilterWithLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByFilterWithLanguages]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilterLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByFilterLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilter]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetByFilter]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAllWithLanguagesIncludingInactive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactive]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAllWithLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetAllWithLanguages]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActiveWithLanguagesLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetActiveWithLanguagesLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActiveWithLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetActiveWithLanguages]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetActiveLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[Exam_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Exam_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Exam_Create]
GO
/****** Object:  StoredProcedure [dbo].[Country_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_Update]
GO
/****** Object:  StoredProcedure [dbo].[Country_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_ToggleStatus]
GO
/****** Object:  StoredProcedure [dbo].[Country_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Country_GetByCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_GetByCode]
GO
/****** Object:  StoredProcedure [dbo].[Country_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Country_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Country_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Country_Create]
GO
/****** Object:  StoredProcedure [dbo].[CmsContentTranslation_DeleteByCmsContentId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContentTranslation_DeleteByCmsContentId]
GO
/****** Object:  StoredProcedure [dbo].[CmsContentTranslation_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContentTranslation_Create]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_Update]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetByKey]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_GetByKey]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_GetById]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_GetActive]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_ExistsByKeyExceptId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_ExistsByKeyExceptId]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_ExistsByKey]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_ExistsByKey]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_Delete]
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[CmsContent_Create]
GO
/****** Object:  StoredProcedure [dbo].[Category_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_Update]
GO
/****** Object:  StoredProcedure [dbo].[Category_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_ToggleStatus]
GO
/****** Object:  StoredProcedure [dbo].[Category_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_SoftDelete]
GO
/****** Object:  StoredProcedure [dbo].[Category_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_SetActive]
GO
/****** Object:  StoredProcedure [dbo].[Category_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_GetByIdLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Category_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_GetById]
GO
/****** Object:  StoredProcedure [dbo].[Category_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_GetAll]
GO
/****** Object:  StoredProcedure [dbo].[Category_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_GetActiveLocalized]
GO
/****** Object:  StoredProcedure [dbo].[Category_GetActiveByType]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_GetActiveByType]
GO
/****** Object:  StoredProcedure [dbo].[Category_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_Delete]
GO
/****** Object:  StoredProcedure [dbo].[Category_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP PROCEDURE [dbo].[Category_Create]
GO
ALTER TABLE [dbo].[SubjectLanguages] DROP CONSTRAINT [FK_SubjectLanguages_Subjects]
GO
ALTER TABLE [dbo].[SubjectLanguages] DROP CONSTRAINT [FK_SubjectLanguages_Languages]
GO
ALTER TABLE [dbo].[Streams] DROP CONSTRAINT [FK_Streams_Qualifications_QualificationId]
GO
ALTER TABLE [dbo].[StreamLanguages] DROP CONSTRAINT [FK_StreamLanguages_Streams_StreamId]
GO
ALTER TABLE [dbo].[StreamLanguages] DROP CONSTRAINT [FK_StreamLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[States] DROP CONSTRAINT [FK_States_Countries]
GO
ALTER TABLE [dbo].[StateLanguages] DROP CONSTRAINT [FK_StateLanguages_States_StateId]
GO
ALTER TABLE [dbo].[StateLanguages] DROP CONSTRAINT [FK_StateLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[QualificationLanguages] DROP CONSTRAINT [FK_QualificationLanguages_Qualifications_QualificationId]
GO
ALTER TABLE [dbo].[QualificationLanguages] DROP CONSTRAINT [FK_QualificationLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[ExamQualifications] DROP CONSTRAINT [FK_ExamQualifications_Streams_StreamId]
GO
ALTER TABLE [dbo].[ExamQualifications] DROP CONSTRAINT [FK_ExamQualifications_Qualifications_QualificationId]
GO
ALTER TABLE [dbo].[ExamQualifications] DROP CONSTRAINT [FK_ExamQualifications_Exams_ExamId]
GO
ALTER TABLE [dbo].[ExamLanguages] DROP CONSTRAINT [FK_ExamLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[ExamLanguages] DROP CONSTRAINT [FK_ExamLanguages_Exams_ExamId]
GO
ALTER TABLE [dbo].[CmsContentTranslations] DROP CONSTRAINT [FK_CmsContentTranslations_CmsContents_CmsContentId]
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Subjects]') AND name = N'DF__Subjects__Update__1D7B6025')
BEGIN
    ALTER TABLE [dbo].[Subjects] DROP CONSTRAINT [DF__Subjects__Update__1D7B6025]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Subjects]') AND name = N'DF__Subjects__Create__1C873BEC')
BEGIN
    ALTER TABLE [dbo].[Subjects] DROP CONSTRAINT [DF__Subjects__Create__1C873BEC]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Subjects]') AND name = N'DF__Subjects__IsActi__1B9317B3')
BEGIN
    ALTER TABLE [dbo].[Subjects] DROP CONSTRAINT [DF__Subjects__IsActi__1B9317B3]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[SubjectLanguages]') AND name = N'DF__SubjectLa__Updat__2334397B')
BEGIN
    ALTER TABLE [dbo].[SubjectLanguages] DROP CONSTRAINT [DF__SubjectLa__Updat__2334397B]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[SubjectLanguages]') AND name = N'DF__SubjectLa__Creat__22401542')
BEGIN
    ALTER TABLE [dbo].[SubjectLanguages] DROP CONSTRAINT [DF__SubjectLa__Creat__22401542]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[SubjectLanguages]') AND name = N'DF__SubjectLa__IsAct__214BF109')
BEGIN
    ALTER TABLE [dbo].[SubjectLanguages] DROP CONSTRAINT [DF__SubjectLa__IsAct__214BF109]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Streams]') AND name = N'DF__Streams__IsActiv__3587F3E0')
BEGIN
    ALTER TABLE [dbo].[Streams] DROP CONSTRAINT [DF__Streams__IsActiv__3587F3E0]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Streams]') AND name = N'DF__Streams__Created__3493CFA7')
BEGIN
    ALTER TABLE [dbo].[Streams] DROP CONSTRAINT [DF__Streams__Created__3493CFA7]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[StreamLanguages]') AND name = N'DF__StreamLan__IsAct__3A4CA8FD')
BEGIN
    ALTER TABLE [dbo].[StreamLanguages] DROP CONSTRAINT [DF__StreamLan__IsAct__3A4CA8FD]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[StreamLanguages]') AND name = N'DF__StreamLan__Creat__395884C4')
BEGIN
    ALTER TABLE [dbo].[StreamLanguages] DROP CONSTRAINT [DF__StreamLan__Creat__395884C4]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[States]') AND name = N'DF__States__IsActive__5070F446')
BEGIN
    ALTER TABLE [dbo].[States] DROP CONSTRAINT [DF__States__IsActive__5070F446]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[States]') AND name = N'DF__States__CreatedA__4F7CD00D')
BEGIN
    ALTER TABLE [dbo].[States] DROP CONSTRAINT [DF__States__CreatedA__4F7CD00D]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[StateLanguages]') AND name = N'DF__StateLangu__Name__6E01572D')
BEGIN
    ALTER TABLE [dbo].[StateLanguages] DROP CONSTRAINT [DF__StateLangu__Name__6E01572D]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[StateLanguages]') AND name = N'DF__StateLang__IsAct__628FA481')
BEGIN
    ALTER TABLE [dbo].[StateLanguages] DROP CONSTRAINT [DF__StateLang__IsAct__628FA481]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[StateLanguages]') AND name = N'DF__StateLang__Creat__619B8048')
BEGIN
    ALTER TABLE [dbo].[StateLanguages] DROP CONSTRAINT [DF__StateLang__Creat__619B8048]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Qualifications]') AND name = N'DF__Qualifica__IsAct__2A164134')
BEGIN
    ALTER TABLE [dbo].[Qualifications] DROP CONSTRAINT [DF__Qualifica__IsAct__2A164134]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Qualifications]') AND name = N'DF__Qualifica__Creat__29221CFB')
BEGIN
    ALTER TABLE [dbo].[Qualifications] DROP CONSTRAINT [DF__Qualifica__Creat__29221CFB]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[QualificationLanguages]') AND name = N'DF__Qualifica__IsAct__2FCF1A8A')
BEGIN
    ALTER TABLE [dbo].[QualificationLanguages] DROP CONSTRAINT [DF__Qualifica__IsAct__2FCF1A8A]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[QualificationLanguages]') AND name = N'DF__Qualifica__Creat__2EDAF651')
BEGIN
    ALTER TABLE [dbo].[QualificationLanguages] DROP CONSTRAINT [DF__Qualifica__Creat__2EDAF651]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Languages]') AND name = N'DF__Languages__IsAct__4CA06362')
BEGIN
    ALTER TABLE [dbo].[Languages] DROP CONSTRAINT [DF__Languages__IsAct__4CA06362]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Languages]') AND name = N'DF__Languages__Creat__4BAC3F29')
BEGIN
    ALTER TABLE [dbo].[Languages] DROP CONSTRAINT [DF__Languages__Creat__4BAC3F29]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Exams]') AND name = N'DF__Exams__IsInterna__607251E5')
BEGIN
    ALTER TABLE [dbo].[Exams] DROP CONSTRAINT [DF__Exams__IsInterna__607251E5]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Exams]') AND name = N'DF__Exams__IsActive__503BEA1C')
BEGIN
    ALTER TABLE [dbo].[Exams] DROP CONSTRAINT [DF__Exams__IsActive__503BEA1C]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Exams]') AND name = N'DF__Exams__CreatedAt__4F47C5E3')
BEGIN
    ALTER TABLE [dbo].[Exams] DROP CONSTRAINT [DF__Exams__CreatedAt__4F47C5E3]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[ExamQualifications]') AND name = N'DF__ExamQuali__IsAct__5BAD9CC8')
BEGIN
    ALTER TABLE [dbo].[ExamQualifications] DROP CONSTRAINT [DF__ExamQuali__IsAct__5BAD9CC8]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[ExamQualifications]') AND name = N'DF__ExamQuali__Creat__5AB9788F')
BEGIN
    ALTER TABLE [dbo].[ExamQualifications] DROP CONSTRAINT [DF__ExamQuali__Creat__5AB9788F]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[ExamLanguages]') AND name = N'DF__ExamLangu__IsAct__55F4C372')
BEGIN
    ALTER TABLE [dbo].[ExamLanguages] DROP CONSTRAINT [DF__ExamLangu__IsAct__55F4C372]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[ExamLanguages]') AND name = N'DF__ExamLangu__Creat__55009F39')
BEGIN
    ALTER TABLE [dbo].[ExamLanguages] DROP CONSTRAINT [DF__ExamLangu__Creat__55009F39]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Countries]') AND name = N'DF__Countries__Creat__2CC890AD')
BEGIN
    ALTER TABLE [dbo].[Countries] DROP CONSTRAINT [DF__Countries__Creat__2CC890AD]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Countries]') AND name = N'DF__Countries__IsAct__2BD46C74')
BEGIN
    ALTER TABLE [dbo].[Countries] DROP CONSTRAINT [DF__Countries__IsAct__2BD46C74]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Countries]') AND name = N'DF__Countries__Phone__2AE0483B')
BEGIN
    ALTER TABLE [dbo].[Countries] DROP CONSTRAINT [DF__Countries__Phone__2AE0483B]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[CmsContents]') AND name = N'DF__CmsConten__IsAct__07C12930')
BEGIN
    ALTER TABLE [dbo].[CmsContents] DROP CONSTRAINT [DF__CmsConten__IsAct__07C12930]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[CmsContents]') AND name = N'DF__CmsConten__Creat__06CD04F7')
BEGIN
    ALTER TABLE [dbo].[CmsContents] DROP CONSTRAINT [DF__CmsConten__Creat__06CD04F7]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Categories]') AND name = N'DF__Categorie__Statu__4F52B2DB')
BEGIN
    ALTER TABLE [dbo].[Categories] DROP CONSTRAINT [DF__Categorie__Statu__4F52B2DB]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Categories]') AND name = N'DF__Categorie__IsAct__03F0984C')
BEGIN
    ALTER TABLE [dbo].[Categories] DROP CONSTRAINT [DF__Categorie__IsAct__03F0984C]
END
GO
IF EXISTS (SELECT * FROM sys.default_constraints WHERE parent_object_id = OBJECT_ID(N'[dbo].[Categories]') AND name = N'DF__Categorie__Creat__02FC7413')
BEGIN
    ALTER TABLE [dbo].[Categories] DROP CONSTRAINT [DF__Categorie__Creat__02FC7413]
END
GO
/****** Object:  Index [IX_Subjects_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Subjects_IsActive] ON [dbo].[Subjects]
GO
/****** Object:  Index [IX_SubjectLanguages_SubjectId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_SubjectLanguages_SubjectId_LanguageId] ON [dbo].[SubjectLanguages]
GO
/****** Object:  Index [IX_SubjectLanguages_SubjectId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_SubjectLanguages_SubjectId] ON [dbo].[SubjectLanguages]
GO
/****** Object:  Index [IX_SubjectLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_SubjectLanguages_LanguageId] ON [dbo].[SubjectLanguages]
GO
/****** Object:  Index [IX_SubjectLanguages_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_SubjectLanguages_IsActive] ON [dbo].[SubjectLanguages]
GO
/****** Object:  Index [UQ_SubjectLanguages_Subject_Language]    Script Date: 4/3/2026 12:05:37 PM ******/
ALTER TABLE [dbo].[SubjectLanguages] DROP CONSTRAINT [UQ_SubjectLanguages_Subject_Language]
GO
/****** Object:  Index [IX_Streams_QualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Streams_QualificationId] ON [dbo].[Streams]
GO
/****** Object:  Index [IX_Streams_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Streams_Name] ON [dbo].[Streams]
GO
/****** Object:  Index [IX_Streams_IsActive_QualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Streams_IsActive_QualificationId] ON [dbo].[Streams]
GO
/****** Object:  Index [IX_Streams_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Streams_IsActive] ON [dbo].[Streams]
GO
/****** Object:  Index [IX_StreamLanguages_StreamId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_StreamLanguages_StreamId_LanguageId] ON [dbo].[StreamLanguages]
GO
/****** Object:  Index [IX_StreamLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_StreamLanguages_LanguageId] ON [dbo].[StreamLanguages]
GO
/****** Object:  Index [IX_States_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_States_Name] ON [dbo].[States]
GO
/****** Object:  Index [IX_States_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_States_IsActive] ON [dbo].[States]
GO
/****** Object:  Index [IX_StateLanguages_StateId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_StateLanguages_StateId_LanguageId] ON [dbo].[StateLanguages]
GO
/****** Object:  Index [IX_StateLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_StateLanguages_LanguageId] ON [dbo].[StateLanguages]
GO
/****** Object:  Index [IX_Qualifications_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Qualifications_Name] ON [dbo].[Qualifications]
GO
/****** Object:  Index [IX_Qualifications_IsActive_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Qualifications_IsActive_CountryCode] ON [dbo].[Qualifications]
GO
/****** Object:  Index [IX_Qualifications_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Qualifications_IsActive] ON [dbo].[Qualifications]
GO
/****** Object:  Index [IX_Qualifications_CountryId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Qualifications_CountryId] ON [dbo].[Qualifications]
GO
/****** Object:  Index [IX_Qualifications_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Qualifications_CountryCode] ON [dbo].[Qualifications]
GO
/****** Object:  Index [IX_QualificationLanguages_QualificationId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_QualificationLanguages_QualificationId_LanguageId] ON [dbo].[QualificationLanguages]
GO
/****** Object:  Index [IX_QualificationLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_QualificationLanguages_LanguageId] ON [dbo].[QualificationLanguages]
GO
/****** Object:  Index [IX_Languages_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Languages_Name] ON [dbo].[Languages]
GO
/****** Object:  Index [IX_Languages_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Languages_IsActive] ON [dbo].[Languages]
GO
/****** Object:  Index [IX_Languages_Code]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Languages_Code] ON [dbo].[Languages]
GO
/****** Object:  Index [IX_Exams_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Exams_Name] ON [dbo].[Exams]
GO
/****** Object:  Index [IX_Exams_IsInternational]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Exams_IsInternational] ON [dbo].[Exams]
GO
/****** Object:  Index [IX_Exams_IsActive_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Exams_IsActive_CountryCode] ON [dbo].[Exams]
GO
/****** Object:  Index [IX_Exams_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Exams_IsActive] ON [dbo].[Exams]
GO
/****** Object:  Index [IX_Exams_CountryId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Exams_CountryId] ON [dbo].[Exams]
GO
/****** Object:  Index [IX_Exams_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Exams_CountryCode] ON [dbo].[Exams]
GO
/****** Object:  Index [IX_ExamQualifications_StreamId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_ExamQualifications_StreamId] ON [dbo].[ExamQualifications]
GO
/****** Object:  Index [IX_ExamQualifications_QualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_ExamQualifications_QualificationId] ON [dbo].[ExamQualifications]
GO
/****** Object:  Index [IX_ExamQualifications_ExamId_QualificationId_StreamId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_ExamQualifications_ExamId_QualificationId_StreamId] ON [dbo].[ExamQualifications]
GO
/****** Object:  Index [IX_ExamLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_ExamLanguages_LanguageId] ON [dbo].[ExamLanguages]
GO
/****** Object:  Index [IX_ExamLanguages_ExamId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_ExamLanguages_ExamId_LanguageId] ON [dbo].[ExamLanguages]
GO
/****** Object:  Index [IX_Countries_Iso2]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Countries_Iso2] ON [dbo].[Countries]
GO
/****** Object:  Index [IX_Countries_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Countries_IsActive] ON [dbo].[Countries]
GO
/****** Object:  Index [IX_Countries_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Countries_CountryCode] ON [dbo].[Countries]
GO
/****** Object:  Index [IX_CmsContentTranslations_LanguageCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_CmsContentTranslations_LanguageCode] ON [dbo].[CmsContentTranslations]
GO
/****** Object:  Index [IX_CmsContentTranslations_CmsContentId_LanguageCode]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_CmsContentTranslations_CmsContentId_LanguageCode] ON [dbo].[CmsContentTranslations]
GO
/****** Object:  Index [IX_CmsContentTranslations_CmsContentId]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_CmsContentTranslations_CmsContentId] ON [dbo].[CmsContentTranslations]
GO
/****** Object:  Index [IX_CmsContents_Key]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_CmsContents_Key] ON [dbo].[CmsContents]
GO
/****** Object:  Index [IX_CmsContents_IsActive_Key]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_CmsContents_IsActive_Key] ON [dbo].[CmsContents]
GO
/****** Object:  Index [IX_CmsContents_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_CmsContents_IsActive] ON [dbo].[CmsContents]
GO
/****** Object:  Index [IX_Categories_Type]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Categories_Type] ON [dbo].[Categories]
GO
/****** Object:  Index [IX_Categories_Status]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Categories_Status] ON [dbo].[Categories]
GO
/****** Object:  Index [IX_Categories_Key]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Categories_Key] ON [dbo].[Categories]
GO
/****** Object:  Index [IX_Categories_IsActive_Type]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Categories_IsActive_Type] ON [dbo].[Categories]
GO
/****** Object:  Index [IX_Categories_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP INDEX [IX_Categories_IsActive] ON [dbo].[Categories]
GO
/****** Object:  Table [dbo].[Subjects]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Subjects]') AND type in (N'U'))
DROP TABLE [dbo].[Subjects]
GO
/****** Object:  Table [dbo].[SubjectLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SubjectLanguages]') AND type in (N'U'))
DROP TABLE [dbo].[SubjectLanguages]
GO
/****** Object:  Table [dbo].[Streams]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Streams]') AND type in (N'U'))
DROP TABLE [dbo].[Streams]
GO
/****** Object:  Table [dbo].[StreamLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StreamLanguages]') AND type in (N'U'))
DROP TABLE [dbo].[StreamLanguages]
GO
/****** Object:  Table [dbo].[States]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[States]') AND type in (N'U'))
DROP TABLE [dbo].[States]
GO
/****** Object:  Table [dbo].[StateLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StateLanguages]') AND type in (N'U'))
DROP TABLE [dbo].[StateLanguages]
GO
/****** Object:  Table [dbo].[Qualifications]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Qualifications]') AND type in (N'U'))
DROP TABLE [dbo].[Qualifications]
GO
/****** Object:  Table [dbo].[QualificationLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QualificationLanguages]') AND type in (N'U'))
DROP TABLE [dbo].[QualificationLanguages]
GO
/****** Object:  Table [dbo].[Languages]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Languages]') AND type in (N'U'))
DROP TABLE [dbo].[Languages]
GO
/****** Object:  Table [dbo].[Exams]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Exams]') AND type in (N'U'))
DROP TABLE [dbo].[Exams]
GO
/****** Object:  Table [dbo].[ExamQualifications]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamQualifications]') AND type in (N'U'))
DROP TABLE [dbo].[ExamQualifications]
GO
/****** Object:  Table [dbo].[ExamLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExamLanguages]') AND type in (N'U'))
DROP TABLE [dbo].[ExamLanguages]
GO
/****** Object:  Table [dbo].[Countries_Backup]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries_Backup]') AND type in (N'U'))
DROP TABLE [dbo].[Countries_Backup]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Countries]') AND type in (N'U'))
DROP TABLE [dbo].[Countries]
GO
/****** Object:  Table [dbo].[CmsContentTranslations]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContentTranslations]') AND type in (N'U'))
DROP TABLE [dbo].[CmsContentTranslations]
GO
/****** Object:  Table [dbo].[CmsContents]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CmsContents]') AND type in (N'U'))
DROP TABLE [dbo].[CmsContents]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Categories]') AND type in (N'U'))
DROP TABLE [dbo].[Categories]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/3/2026 12:05:37 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
DROP TABLE [dbo].[__EFMigrationsHistory]
GO
USE [master]
GO
/****** Object:  Database [RankUp_MasterDB]    Script Date: 4/3/2026 12:05:37 PM ******/
DROP DATABASE [RankUp_MasterDB]
GO
/****** Object:  Database [RankUp_MasterDB]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE DATABASE [RankUp_MasterDB]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RankUp_MasterDB', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_MasterDB.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'RankUp_MasterDB_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\RankUp_MasterDB_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [RankUp_MasterDB] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RankUp_MasterDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RankUp_MasterDB] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET ARITHABORT OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [RankUp_MasterDB] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RankUp_MasterDB] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RankUp_MasterDB] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET  ENABLE_BROKER 
GO
ALTER DATABASE [RankUp_MasterDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RankUp_MasterDB] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [RankUp_MasterDB] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [RankUp_MasterDB] SET  MULTI_USER 
GO
ALTER DATABASE [RankUp_MasterDB] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RankUp_MasterDB] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RankUp_MasterDB] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RankUp_MasterDB] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [RankUp_MasterDB] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [RankUp_MasterDB] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [RankUp_MasterDB] SET QUERY_STORE = ON
GO
ALTER DATABASE [RankUp_MasterDB] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [RankUp_MasterDB]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NameEn] [nvarchar](100) NOT NULL,
	[NameHi] [nvarchar](100) NULL,
	[Key] [nvarchar](50) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[Status] [nvarchar](20) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[DisplayOrder] [int] NULL,
 CONSTRAINT [PK_Categories] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CmsContents]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CmsContents](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Key] [nvarchar](100) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_CmsContents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CmsContentTranslations]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CmsContentTranslations](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CmsContentId] [int] NOT NULL,
	[LanguageCode] [nvarchar](10) NOT NULL,
	[Title] [nvarchar](500) NOT NULL,
	[Content] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_CmsContentTranslations] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Iso2] [nvarchar](2) NOT NULL,
	[CountryCode] [nvarchar](5) NOT NULL,
	[PhoneLength] [int] NOT NULL,
	[CurrencyCode] [nvarchar](3) NOT NULL,
	[Image] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
 CONSTRAINT [PK_Countries] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Countries_Backup]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Countries_Backup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](10) NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[SubdivisionLabelEn] [nvarchar](50) NULL,
	[SubdivisionLabelHi] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExamLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamLanguages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExamId] [int] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ExamLanguages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ExamQualifications]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExamQualifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExamId] [int] NOT NULL,
	[QualificationId] [int] NOT NULL,
	[StreamId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_ExamQualifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Exams]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Exams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](1000) NULL,
	[CountryCode] [nvarchar](10) NULL,
	[CountryId] [int] NULL,
	[MinAge] [int] NULL,
	[MaxAge] [int] NULL,
	[ImageUrl] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[IsInternational] [bit] NOT NULL,
 CONSTRAINT [PK_Exams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Languages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Languages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](10) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Languages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[QualificationLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QualificationLanguages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QualificationId] [int] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_QualificationLanguages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Qualifications]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Qualifications](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CountryCode] [nvarchar](10) NULL,
	[CountryId] [int] NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[NameHi] [nvarchar](100) NULL,
 CONSTRAINT [PK_Qualifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StateLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StateLanguages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StateId] [int] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
 CONSTRAINT [PK_StateLanguages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[States]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[States](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Code] [nvarchar](10) NULL,
	[CountryCode] [nvarchar](2) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StreamLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StreamLanguages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StreamId] [int] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_StreamLanguages] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Streams]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Streams](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[QualificationId] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NULL,
	[IsActive] [bit] NOT NULL,
	[NameHi] [nvarchar](100) NULL,
 CONSTRAINT [PK_Streams] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SubjectLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SubjectLanguages](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SubjectId] [int] NOT NULL,
	[LanguageId] [int] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Subjects]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Subjects](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[StreamId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260116125833_InitialCreate', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260116130043_AddCountryCodeAndStateLanguages', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260116130154_SeedCountryStateLanguageData', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260121091137_AddStateLanguageName', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260121092243_SeedStateLanguages', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260212165925_AddCmsContent', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260216044657_RefactorCmsToDynamicTranslations', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260216121901_UpdateCountryAndStateEntities', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260220053809_AddQualificationAndStream', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260220110401_AddExamMaster', N'8.0.0')
INSERT [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES (N'20260220123255_AddIsInternationalToExam', N'8.0.0')
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (1, N'Updated Test Category', N'??????? ??????? ??????', N'updated-test-category', N'category', CAST(N'2026-03-09T11:41:44.5200000' AS DateTime2), CAST(N'2026-03-19T11:04:29.2300000' AS DateTime2), 0, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (2, N'general', N'सामान्य', N'general', N'category', CAST(N'2026-03-10T05:25:01.7800000' AS DateTime2), CAST(N'2026-03-26T05:33:46.7100000' AS DateTime2), 1, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (3, N'Test Category', N'????? ??????', N'test-category', N'category', CAST(N'2026-03-10T06:33:17.6800000' AS DateTime2), CAST(N'2026-03-19T11:04:24.9000000' AS DateTime2), 0, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (7, N'SC', N'अनुसूचित जाति', N'sc', N'category', CAST(N'2026-03-10T06:41:37.7933333' AS DateTime2), CAST(N'2026-03-26T05:34:10.3300000' AS DateTime2), 1, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (8, N'Hindi Test', N'????? ???????', N'hindi-test', N'category', CAST(N'2026-03-10T06:45:32.9700000' AS DateTime2), CAST(N'2026-03-19T11:04:20.0966667' AS DateTime2), 0, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (11, N'New Test Category', N'??? ????? ??????', N'new-test-category', N'exam', CAST(N'2026-03-13T06:50:14.8466667' AS DateTime2), CAST(N'2026-03-13T06:50:14.8466667' AS DateTime2), 1, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (12, N'sadsad', N'', N'sadsa', N'category', CAST(N'2026-03-23T08:53:29.1966667' AS DateTime2), CAST(N'2026-03-23T08:53:37.2233333' AS DateTime2), 0, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (26, N'OBC', N'अन्य पिछड़ा वर्ग', N'obc', N'category', CAST(N'2026-03-25T08:33:55.4233333' AS DateTime2), CAST(N'2026-03-26T05:34:02.0600000' AS DateTime2), 1, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (27, N'ST', N'अनुसूचित जनजाति', N'st', N'category', CAST(N'2026-03-25T08:34:04.3766667' AS DateTime2), CAST(N'2026-03-26T05:34:17.7700000' AS DateTime2), 1, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (28, N'Test Simple', N'????? ?????', N'test-simple', N'category', CAST(N'2026-03-25T08:49:11.5766667' AS DateTime2), CAST(N'2026-03-25T10:47:28.0166667' AS DateTime2), 0, N'active', NULL, 0)
INSERT [dbo].[Categories] ([Id], [NameEn], [NameHi], [Key], [Type], [CreatedAt], [UpdatedAt], [IsActive], [Status], [Description], [DisplayOrder]) VALUES (29, N'testing', N'परीक्षण', N'test', N'category', CAST(N'2026-03-25T08:59:34.7000000' AS DateTime2), CAST(N'2026-03-25T08:59:41.0366667' AS DateTime2), 0, N'active', NULL, 0)
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[CmsContents] ON 

INSERT [dbo].[CmsContents] ([Id], [Key], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (12, N'privacy_policy', CAST(N'2026-02-16T08:41:11.8798121' AS DateTime2), CAST(N'2026-03-30T09:14:14.7766667' AS DateTime2), 1)
INSERT [dbo].[CmsContents] ([Id], [Key], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (18, N'terms_and_conditions', CAST(N'2026-02-16T09:11:22.9717948' AS DateTime2), CAST(N'2026-03-30T09:32:03.8000000' AS DateTime2), 1)
INSERT [dbo].[CmsContents] ([Id], [Key], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (19, N'about_us', CAST(N'2026-02-25T08:36:55.4329093' AS DateTime2), CAST(N'2026-03-30T09:50:49.0600000' AS DateTime2), 0)
INSERT [dbo].[CmsContents] ([Id], [Key], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (22, N'faq', CAST(N'2026-02-27T08:57:22.1528685' AS DateTime2), CAST(N'2026-03-30T08:31:01.8200000' AS DateTime2), 0)
SET IDENTITY_INSERT [dbo].[CmsContents] OFF
GO
SET IDENTITY_INSERT [dbo].[CmsContentTranslations] ON 

INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (67, 22, N'en', N'FAQ', N'Hello this is for testing')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (68, 22, N'hi', N'अक्सर पूछे जाने वाले प्रश्न', N'नमस्ते यह परीक्षण के लिए है')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (75, 12, N'en', N'Privacy Policy', N'<h1>Privacy Policy</h1><p>Your privacy is important to us. This policy explains how we collect, use, and protect your information.</p><h2>Information We Collect</h2><p>We collect information you provide directly to us, such as when you create an account, take tests, or contact us.</p><h2>How We Use Your Information</h2><p>We use the information we collect to provide, maintain, and improve our services, process your requests, and communicate with you.</p><h2>Information Sharing</h2><p>We do not sell, trade, or otherwise transfer your personal information to third parties without your consent.</p><h2>Data Security</h2><p>We implement appropriate technical and organizational measures to protect your personal information against unauthorized access, alteration, disclosure, or destruction.</p><h2>Contact Us</h2><p>If you have any questions about this Privacy Policy, please contact us at support@rankup.com.</p>')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (76, 12, N'hi', N'गोपनीयता नीति', N'<h1>गोपनीयता नीति</h1><p>आपकी निजता हमारे लिए महत्वपूर्ण है। यह नीति बताती है कि हम आपकी जानकारी कैसे एकत्र करते हैं, उसका उपयोग करते हैं और उसकी सुरक्षा कैसे करते हैं।</p><h2>जानकारी हम एकत्रित करते हैं</h2><p>हम आपके द्वारा सीधे हमें प्रदान की गई जानकारी एकत्र करते हैं, जैसे कि जब आप कोई खाता बनाते हैं, परीक्षण करते हैं, या हमसे संपर्क करते हैं।</p><h2>हम आपकी सूचना का किस प्रकार प्रयोग करते हैं</h2><p>हम एकत्रित की गई जानकारी का उपयोग अपनी सेवाएं प्रदान करने, बनाए रखने और सुधारने, आपके अनुरोधों पर कार्रवाई करने और आपके साथ संवाद करने के लिए करते हैं।</p><h2>जानकारी साझाकरण</h2><p>हम आपकी सहमति के बिना आपकी व्यक्तिगत जानकारी को तीसरे पक्ष को बेचते, व्यापार या अन्यथा हस्तांतरित नहीं करते हैं।</p><h2>डेटा सुरक्षा</h2><p>हम आपकी व्यक्तिगत जानकारी को अनधिकृत पहुंच, परिवर्तन, प्रकटीकरण या विनाश से बचाने के लिए उचित तकनीकी और संगठनात्मक उपाय लागू करते हैं।</p><h2>हमसे संपर्क करें</h2><p>यदि इस गोपनीयता नीति के बारे में आपके कोई प्रश्न हैं, तो कृपया support@rankup.com पर हमसे संपर्क करें।</p>')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (77, 18, N'en', N'Terms And Conditions', N'These are the updated terms and conditions')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (78, 18, N'hi', N'नियम और शर्तें', N'ये अद्यतन नियम और शर्तें हैं')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (79, 19, N'en', N'About Us', N'<p>Welcome to RankUp, your trusted partner in competitive exam preparation. Our mission is to empower students with high-quality study materials, realistic test series, and data-driven performance insights to help them achieve their academic and career goals.</p><h2>Who We Are</h2><p>RankUp is an innovative ed-tech platform designed to provide structured learning, expert-curated questions, and real-time analytics. We aim to bridge the gap between preparation and success by offering a smart and personalized learning experience.</p><h2>What We Offer</h2><ul><li>Comprehensive exam-focused test series</li><li>Detailed performance analysis and ranking</li><li>All-India level mock tests</li><li>Up-to-date syllabus-based content</li><li>User-friendly dashboard for tracking progress</li></ul><h2>Our Vision</h2><p>Our vision is to become a leading digital learning platform that supports millions of aspirants in achieving top ranks in their respective examinations.</p><h2>Our Commitment</h2><p>We are committed to providing reliable, affordable, and high-quality educational resources. At RankUp, your success is our priority.</p><p>If you have any questions or feedback, feel free to contact us at support@rankup.com.</p>')
INSERT [dbo].[CmsContentTranslations] ([Id], [CmsContentId], [LanguageCode], [Title], [Content]) VALUES (80, 19, N'hi', N'हमारे बारे में', N'<p>RankUp में आपका स्वागत है। हम प्रतिस्पर्धी परीक्षाओं की तैयारी करने वाले छात्रों के लिए एक विश्वसनीय डिजिटल प्लेटफ़ॉर्म हैं। हमारा उद्देश्य उच्च गुणवत्ता की अध्ययन सामग्री, वास्तविक परीक्षा जैसी टेस्ट सीरीज़ और प्रदर्शन विश्लेषण प्रदान करके छात्रों को सफलता की ओर अग्रसर करना है।</p><h2>हम कौन हैं</h2><p>RankUp एक आधुनिक एड-टेक प्लेटफ़ॉर्म है जो संरचित सीखने की सुविधा, विशेषज्ञों द्वारा तैयार प्रश्न और रियल-टाइम प्रदर्शन विश्लेषण प्रदान करता है। हम तैयारी और सफलता के बीच की दूरी को कम करने का प्रयास करते हैं।</p><h2>हम क्या प्रदान करते हैं</h2><ul><li>परीक्षा आधारित व्यापक टेस्ट सीरीज़</li><li>विस्तृत प्रदर्शन विश्लेषण और रैंकिंग</li><li>ऑल इंडिया स्तर की मॉक टेस्ट</li><li>अपडेटेड सिलेबस आधारित सामग्री</li><li>प्रगति ट्रैक करने के लिए उपयोगकर्ता-अनुकूल डैशबोर्ड</li></ul><h2>हमारा दृष्टिकोण</h2><p>हमारा लक्ष्य लाखों छात्रों को उनकी परीक्षाओं में उच्च रैंक प्राप्त करने में सहायता करने वाला एक अग्रणी डिजिटल लर्निंग प्लेटफ़ॉर्म बनना है।</p><h2>हमारी प्रतिबद्धता</h2><p>हम विश्वसनीय, किफायती और उच्च गुणवत्ता वाली शैक्षिक सेवाएं प्रदान करने के लिए प्रतिबद्ध हैं। RankUp में आपकी सफलता हमारी प्राथमिकता है।</p><p>किसी भी प्रश्न या सुझाव के लिए support@rankup.com पर संपर्क करें।</p>')
SET IDENTITY_INSERT [dbo].[CmsContentTranslations] OFF
GO
SET IDENTITY_INSERT [dbo].[Countries] ON 

INSERT [dbo].[Countries] ([Id], [Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (9, N'India', N'IN', N'+91', 10, N'INR', N'/uploads/flags/in.png', 1, CAST(N'2026-03-26T09:36:15.1433333' AS DateTime2), CAST(N'2026-03-27T06:12:20.7500000' AS DateTime2))
INSERT [dbo].[Countries] ([Id], [Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (11, N'United Kingdom', N'UK', N'+44', 11, N'GBP', N'/uploads/flags/uk.png', 0, CAST(N'2026-03-27T06:13:31.0333333' AS DateTime2), CAST(N'2026-03-27T06:53:29.8566667' AS DateTime2))
INSERT [dbo].[Countries] ([Id], [Name], [Iso2], [CountryCode], [PhoneLength], [CurrencyCode], [Image], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (12, N'USA', N'US', N'+1', 6, N'USD', N'/uploads/flags/us.png', 1, CAST(N'2026-03-27T06:58:18.8100000' AS DateTime2), CAST(N'2026-03-27T06:58:18.8100000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Countries] OFF
GO
SET IDENTITY_INSERT [dbo].[Countries_Backup] ON 

INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (1, N'India', N'IN', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (2, N'United States', N'US', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (3, N'United Kingdom', N'GB', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (4, N'Canada', N'CA', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), CAST(N'2026-02-25T09:36:55.7548712' AS DateTime2), 0, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (5, N'Australia', N'AU', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), CAST(N'2026-02-20T08:43:24.9346786' AS DateTime2), 0, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (6, N'Germany', N'DE', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), CAST(N'2026-02-27T09:24:24.9203154' AS DateTime2), 1, N'State', N'राज्य')
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (7, N'France', N'FR', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), CAST(N'2026-02-26T09:13:07.8863334' AS DateTime2), 0, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (8, N'Japan', N'JP', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (9, N'China', N'CN', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), CAST(N'2026-02-25T11:04:42.3845737' AS DateTime2), 0, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (10, N'Singapore', N'SG', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (11, N'United Arab Emirates', N'AE', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (12, N'Saudi Arabia', N'SA', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (13, N'Malaysia', N'MY', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (14, N'Thailand', N'TH', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (15, N'Indonesia', N'ID', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (16, N'Philippines', N'PH', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (17, N'Sri Lanka', N'LK', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (18, N'Bangladesh', N'BD', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), CAST(N'2026-02-20T08:43:29.6869103' AS DateTime2), 0, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (19, N'Nepal', N'NP', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (20, N'Pakistan', N'PK', CAST(N'2026-01-16T18:33:29.8200000' AS DateTime2), NULL, 1, NULL, NULL)
INSERT [dbo].[Countries_Backup] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive], [SubdivisionLabelEn], [SubdivisionLabelHi]) VALUES (23, N'Test Country', N'TC', CAST(N'2026-02-16T12:21:53.4713043' AS DateTime2), NULL, 1, N'Province', N'???')
SET IDENTITY_INSERT [dbo].[Countries_Backup] OFF
GO
SET IDENTITY_INSERT [dbo].[ExamLanguages] ON 

INSERT [dbo].[ExamLanguages] ([Id], [ExamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (42, 31, 50, N'RRB', N'Railway Recruitment Board Exam (RRB)', CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamLanguages] ([Id], [ExamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (43, 31, 49, N'आरआरबी', N'रेलवे भर्ती बोर्ड परीक्षा (आरआरबी)', CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamLanguages] ([Id], [ExamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (46, 32, 50, N'TOEFL', N'The TOEFL iBT is a premier, widely accepted English proficiency test for academic, work, or visa purposes, assessing all four skills—reading, listening, speaking, and writing.', CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamLanguages] ([Id], [ExamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (47, 32, 49, N'टॉफेल', N'TOEFL iBT शैक्षणिक, कार्य या वीज़ा उद्देश्यों के लिए एक प्रमुख, व्यापक रूप से स्वीकृत अंग्रेजी दक्षता परीक्षा है, जो सभी चार कौशलों- पढ़ना, सुनना, बोलना और लिखना का आकलन करती है।', CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[ExamLanguages] OFF
GO
SET IDENTITY_INSERT [dbo].[ExamQualifications] ON 

INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (87, 31, 4, 7, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (88, 31, 6, 6, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (89, 31, 5, 1, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (90, 31, 1, 4, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (91, 31, 7, 2, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (92, 31, 8, 3, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (93, 31, 2, 5, CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (101, 32, 4, 7, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (102, 32, 6, 6, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (103, 32, 5, 1, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (104, 32, 1, 4, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (105, 32, 7, 3, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (106, 32, 8, 2, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
INSERT [dbo].[ExamQualifications] ([Id], [ExamId], [QualificationId], [StreamId], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (107, 32, 2, 5, CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[ExamQualifications] OFF
GO
SET IDENTITY_INSERT [dbo].[Exams] ON 

INSERT [dbo].[Exams] ([Id], [Name], [Description], [CountryCode], [CountryId], [MinAge], [MaxAge], [ImageUrl], [CreatedAt], [UpdatedAt], [IsActive], [IsInternational]) VALUES (18, N'test 23', N'testings q', N'IN', NULL, 18, 60, N'http://192.168.1.19:5009/uploads/exams/exam_18_20260401083512_9867.jpeg', CAST(N'2026-03-19T10:37:34.8633333' AS DateTime2), CAST(N'2026-04-01T08:35:12.8066667' AS DateTime2), 1, 0)
INSERT [dbo].[Exams] ([Id], [Name], [Description], [CountryCode], [CountryId], [MinAge], [MaxAge], [ImageUrl], [CreatedAt], [UpdatedAt], [IsActive], [IsInternational]) VALUES (21, N'test 1234', N'testing12314', N'India', NULL, 18, 60, N'http://192.168.1.19:5009/uploads/exams/exam_21_20260401083453_6370.jpeg', CAST(N'2026-03-24T07:07:21.5333333' AS DateTime2), CAST(N'2026-04-01T08:34:53.0266667' AS DateTime2), 1, 0)
INSERT [dbo].[Exams] ([Id], [Name], [Description], [CountryCode], [CountryId], [MinAge], [MaxAge], [ImageUrl], [CreatedAt], [UpdatedAt], [IsActive], [IsInternational]) VALUES (31, N'RRB', N'Railway Recruitment Board Exam (RRB)', N'IN', NULL, 18, 45, N'http://192.168.1.23:5009/uploads/exams/exam_31_20260325093744_5334.png', CAST(N'2026-03-25T09:37:44.2366667' AS DateTime2), CAST(N'2026-03-25T09:37:44.4333333' AS DateTime2), 1, 0)
INSERT [dbo].[Exams] ([Id], [Name], [Description], [CountryCode], [CountryId], [MinAge], [MaxAge], [ImageUrl], [CreatedAt], [UpdatedAt], [IsActive], [IsInternational]) VALUES (32, N'TOEFL', N'The TOEFL iBT is a premier, widely accepted English proficiency test for academic, work, or visa purposes, assessing all four skills—reading, listening, speaking, and writing.', N'US', NULL, 18, 60, N'http://192.168.1.23:5009/uploads/exams/exam_32_20260325094147_9895.png', CAST(N'2026-03-25T09:41:47.7366667' AS DateTime2), CAST(N'2026-03-25T09:41:47.7733333' AS DateTime2), 1, 1)
SET IDENTITY_INSERT [dbo].[Exams] OFF
GO
SET IDENTITY_INSERT [dbo].[Languages] ON 

INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (49, N'Hindi / हिंदी', N'hi', CAST(N'2026-02-18T06:38:28.7336068' AS DateTime2), NULL, 1)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (50, N'English / अंग्रेज़ी', N'en', CAST(N'2026-02-18T06:39:15.2357928' AS DateTime2), CAST(N'2026-03-25T07:01:16.6466667' AS DateTime2), 1)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (52, N'Kannada / ಕನ್ನಡ', N'kn', CAST(N'2026-02-19T06:42:42.7640180' AS DateTime2), CAST(N'2026-02-19T08:57:51.7862692' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (53, N'Marathi / मराठी', N'mr', CAST(N'2026-02-19T09:01:31.9067930' AS DateTime2), CAST(N'2026-02-19T09:01:35.8171242' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (54, N'Gujarati / ગુજરાતી', N'gu', CAST(N'2026-02-25T09:37:19.6801922' AS DateTime2), CAST(N'2026-02-25T09:37:24.7865606' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (55, N'Odia / ଓଡ଼ିଆ', N'or', CAST(N'2026-02-25T10:13:00.5376049' AS DateTime2), CAST(N'2026-02-25T10:16:46.3607255' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (56, N'Tamil / தமிழ்', N'ta', CAST(N'2026-02-25T10:18:53.6580922' AS DateTime2), CAST(N'2026-02-25T10:18:57.0918037' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (57, N'Urdu / اردو', N'ur', CAST(N'2026-02-26T08:30:32.1974951' AS DateTime2), CAST(N'2026-02-26T08:30:36.9226103' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (59, N'Bengali', N'bn', CAST(N'2026-03-27T05:24:30.7166667' AS DateTime2), CAST(N'2026-03-27T08:33:14.9500000' AS DateTime2), 0)
INSERT [dbo].[Languages] ([Id], [Name], [Code], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (60, N'Bodo/बड़ो', N'brx', CAST(N'2026-03-27T05:36:46.0000000' AS DateTime2), CAST(N'2026-03-27T06:31:50.3200000' AS DateTime2), 0)
SET IDENTITY_INSERT [dbo].[Languages] OFF
GO
SET IDENTITY_INSERT [dbo].[QualificationLanguages] ON 

INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (5, 3, 50, N'Bachelor of Science', N'Undergraduate degree program in science', CAST(N'2026-02-20T06:17:04.6197572' AS DateTime2), NULL, 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (6, 3, 49, N'बैचलर ऑफ साइंस', N'विज्ञान में स्नातक डिग्री प्रोग्राम', CAST(N'2026-02-20T06:17:04.6197577' AS DateTime2), NULL, 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (41, 19, 50, N'Test Qualification', N'Test description', CAST(N'2026-03-27T10:39:04.4933333' AS DateTime2), CAST(N'2026-03-27T10:39:04.4933333' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (42, 19, 49, N'परीक्षण योग्यता', N'विवरण परीक्षण', CAST(N'2026-03-27T10:39:04.4933333' AS DateTime2), CAST(N'2026-03-27T10:39:04.4933333' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (43, 20, 50, N'Test Qualification', N'Test description', CAST(N'2026-03-27T10:39:45.1133333' AS DateTime2), CAST(N'2026-03-27T10:39:45.1133333' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (44, 20, 49, N'परीक्षण योग्यता', N'विवरण परीक्षण', CAST(N'2026-03-27T10:39:45.1133333' AS DateTime2), CAST(N'2026-03-27T10:39:45.1133333' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (46, 7, 49, N'हाई स्कूल', N'10वीं कक्षा के बाद माध्यमिक विद्यालय योग्यता पूरी', CAST(N'2026-03-27T10:54:04.8466667' AS DateTime2), CAST(N'2026-03-27T10:54:04.8466667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (47, 7, 50, N'HighSchool', N'Secondary school qualification completed after 10th grade', CAST(N'2026-03-27T10:54:04.8466667' AS DateTime2), CAST(N'2026-03-27T10:54:04.8466667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (48, 5, 50, N'BSc', N'Undergraduate degree program in science', CAST(N'2026-03-27T10:54:47.6866667' AS DateTime2), CAST(N'2026-03-27T10:54:47.6866667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (49, 5, 49, N'बीएससी', N'विज्ञान में स्नातक डिग्री कार्यक्रम', CAST(N'2026-03-27T10:54:47.6866667' AS DateTime2), CAST(N'2026-03-27T10:54:47.6866667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (50, 4, 49, N'बी ० ए', N'कला और मानविकी में स्नातक डिग्री कार्यक्रम', CAST(N'2026-03-27T10:55:19.0000000' AS DateTime2), CAST(N'2026-03-27T10:55:19.0000000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (51, 4, 50, N'BA', N'Undergraduate degree program in arts and humanities', CAST(N'2026-03-27T10:55:19.0000000' AS DateTime2), CAST(N'2026-03-27T10:55:19.0000000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (52, 6, 50, N'BCom', N'Undergraduate degree program in commerce and business', CAST(N'2026-03-27T10:55:52.8266667' AS DateTime2), CAST(N'2026-03-27T10:55:52.8266667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (53, 6, 49, N'बीकॉम', N'वाणिज्य और व्यवसाय में स्नातक डिग्री कार्यक्रम', CAST(N'2026-03-27T10:55:52.8266667' AS DateTime2), CAST(N'2026-03-27T10:55:52.8266667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (54, 1, 50, N'B.Tech', N'Engineering degree program', CAST(N'2026-03-27T10:56:26.1000000' AS DateTime2), CAST(N'2026-03-27T10:56:26.1000000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (55, 1, 49, N'बी.टेक', N'इंजीनियरिंग डिग्री प्रोग्राम', CAST(N'2026-03-27T10:56:26.1000000' AS DateTime2), CAST(N'2026-03-27T10:56:26.1000000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (56, 2, 50, N'M.Tech', N'Postgraduate engineering degree program', CAST(N'2026-03-27T10:57:07.0300000' AS DateTime2), CAST(N'2026-03-27T10:57:07.0300000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (57, 2, 49, N'एम.टेक', N'स्नातकोत्तर इंजीनियरिंग डिग्री कार्यक्रम', CAST(N'2026-03-27T10:57:07.0300000' AS DateTime2), CAST(N'2026-03-27T10:57:07.0300000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (58, 8, 50, N'BBA', N'Higher secondary school qualification completed after 12th grade', CAST(N'2026-03-27T10:57:39.7233333' AS DateTime2), CAST(N'2026-03-27T10:57:39.7233333' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (59, 8, 49, N'बीबीए', N'12वीं कक्षा के बाद पूर्ण की जाने वाली उच्च माध्यमिक शिक्षा योग्यता', CAST(N'2026-03-27T10:57:39.7233333' AS DateTime2), CAST(N'2026-03-27T10:57:39.7233333' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (60, 21, 50, N'Testing', N'sdasdlhs', CAST(N'2026-03-27T10:58:39.8066667' AS DateTime2), CAST(N'2026-03-27T10:58:39.8066667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (61, 21, 49, N'परीक्षण', N'sdasdlhs', CAST(N'2026-03-27T10:58:39.8066667' AS DateTime2), CAST(N'2026-03-27T10:58:39.8066667' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (62, 22, 50, N'testdsauto', N'jsdjhas hsdka', CAST(N'2026-03-27T11:43:52.5700000' AS DateTime2), CAST(N'2026-03-27T11:43:52.5700000' AS DateTime2), 1)
INSERT [dbo].[QualificationLanguages] ([Id], [QualificationId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (63, 22, 49, N'testdsauto', N'jsdjhas hsdka', CAST(N'2026-03-27T11:43:52.5700000' AS DateTime2), CAST(N'2026-03-27T11:43:52.5700000' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[QualificationLanguages] OFF
GO
SET IDENTITY_INSERT [dbo].[Qualifications] ON 

INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (1, N'B.Tech', N'Engineering degree program', N'IN', NULL, CAST(N'2026-02-20T06:15:26.8718424' AS DateTime2), CAST(N'2026-03-27T10:56:26.1000000' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (2, N'M.Tech', N'Postgraduate engineering degree program', N'IN', NULL, CAST(N'2026-02-20T06:16:52.6911175' AS DateTime2), CAST(N'2026-03-27T10:57:07.0300000' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (3, N'Bachelor of Science', N'Undergraduate degree program in science', N'IN', NULL, CAST(N'2026-02-20T06:17:04.6197556' AS DateTime2), CAST(N'2026-02-26T07:34:00.0714626' AS DateTime2), 0, N'???????')
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (4, N'BA', N'Undergraduate degree program in arts and humanities', N'IN', NULL, CAST(N'2026-02-20T06:17:44.4456101' AS DateTime2), CAST(N'2026-03-27T10:55:19.0000000' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (5, N'BSc', N'Undergraduate degree program in science', N'IN', NULL, CAST(N'2026-02-20T06:17:53.8992873' AS DateTime2), CAST(N'2026-03-27T10:54:47.6866667' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (6, N'BCom', N'Undergraduate degree program in commerce and business', N'IN', NULL, CAST(N'2026-02-20T06:18:10.3527732' AS DateTime2), CAST(N'2026-03-27T10:55:52.8266667' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (7, N'HighSchool', N'Secondary school qualification completed after 10th grade', N'IN', NULL, CAST(N'2026-02-20T06:19:00.6578993' AS DateTime2), CAST(N'2026-03-27T10:54:04.8466667' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (8, N'BBA', N'Higher secondary school qualification completed after 12th grade', N'IN', NULL, CAST(N'2026-02-20T06:19:10.4073388' AS DateTime2), CAST(N'2026-03-27T10:57:39.7233333' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (19, N'Test Qualification', N'Test description', N'IN', NULL, CAST(N'2026-03-27T10:32:05.9600000' AS DateTime2), CAST(N'2026-03-27T10:39:04.4933333' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (20, N'Test Qualification', N'Test description', N'IN', NULL, CAST(N'2026-03-27T10:32:18.3100000' AS DateTime2), CAST(N'2026-03-27T10:39:45.1133333' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (21, N'Testing', N'sdasdlhs', N'IN', NULL, CAST(N'2026-03-27T10:58:39.8066667' AS DateTime2), CAST(N'2026-03-27T10:58:39.8066667' AS DateTime2), 1, NULL)
INSERT [dbo].[Qualifications] ([Id], [Name], [Description], [CountryCode], [CountryId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (22, N'testdsauto', N'jsdjhas hsdka', N'IN', NULL, CAST(N'2026-03-27T11:43:52.5700000' AS DateTime2), CAST(N'2026-03-27T11:43:52.5700000' AS DateTime2), 1, NULL)
SET IDENTITY_INSERT [dbo].[Qualifications] OFF
GO
SET IDENTITY_INSERT [dbo].[StateLanguages] ON 

INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (153, 64, 50, CAST(N'2026-02-18T07:35:53.2145768' AS DateTime2), NULL, 1, N'Bihar')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (154, 64, 49, CAST(N'2026-02-18T07:35:53.2145960' AS DateTime2), NULL, 1, N'???')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (164, 69, 50, CAST(N'2026-02-19T05:40:50.1427347' AS DateTime2), NULL, 1, N'Himanchal Pradesh')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (165, 69, 49, CAST(N'2026-02-19T05:40:50.1427348' AS DateTime2), NULL, 1, N'हिमाचल प्रदेश')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (169, 71, 50, CAST(N'2026-03-19T10:06:24.3066667' AS DateTime2), CAST(N'2026-03-19T10:06:24.3066667' AS DateTime2), 1, N'Uttarakhand')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (170, 71, 49, CAST(N'2026-03-19T10:06:24.3066667' AS DateTime2), CAST(N'2026-03-19T10:06:24.3066667' AS DateTime2), 1, N'उत्तराखंड')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (174, 67, 50, CAST(N'2026-03-27T12:25:19.2733333' AS DateTime2), CAST(N'2026-03-27T12:25:19.2733333' AS DateTime2), 1, N'Bihar')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (175, 67, 49, CAST(N'2026-03-27T12:25:19.2733333' AS DateTime2), CAST(N'2026-03-27T12:25:19.2733333' AS DateTime2), 1, N'बिहार')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (176, 68, 50, CAST(N'2026-03-27T12:25:28.6466667' AS DateTime2), CAST(N'2026-03-27T12:25:28.6466667' AS DateTime2), 1, N'Madhya Pradesh')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (177, 68, 49, CAST(N'2026-03-27T12:25:28.6466667' AS DateTime2), CAST(N'2026-03-27T12:25:28.6466667' AS DateTime2), 1, N'मध्य प्रदेश')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (178, 65, 50, CAST(N'2026-03-27T12:25:40.9500000' AS DateTime2), CAST(N'2026-03-27T12:25:40.9500000' AS DateTime2), 1, N'Uttar Pradesh')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (179, 65, 49, CAST(N'2026-03-27T12:25:40.9500000' AS DateTime2), CAST(N'2026-03-27T12:25:40.9500000' AS DateTime2), 1, N'उतार प्रदेश।')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (180, 70, 52, CAST(N'2026-03-27T12:26:12.9466667' AS DateTime2), CAST(N'2026-03-27T12:26:12.9466667' AS DateTime2), 1, N'ಮಹಾರಾಷ್ಟ್ರ')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (181, 70, 50, CAST(N'2026-03-27T12:26:12.9466667' AS DateTime2), CAST(N'2026-03-27T12:26:12.9466667' AS DateTime2), 1, N'Maharastra')
INSERT [dbo].[StateLanguages] ([Id], [StateId], [LanguageId], [CreatedAt], [UpdatedAt], [IsActive], [Name]) VALUES (182, 70, 49, CAST(N'2026-03-27T12:26:12.9466667' AS DateTime2), CAST(N'2026-03-27T12:26:12.9466667' AS DateTime2), 1, N'महाराष्ट्र')
SET IDENTITY_INSERT [dbo].[StateLanguages] OFF
GO
SET IDENTITY_INSERT [dbo].[States] ON 

INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (64, N'Bihar', N'BR', N'IN', CAST(N'2026-02-18T07:35:53.2144999' AS DateTime2), CAST(N'2026-02-18T08:52:06.3702347' AS DateTime2), 0)
INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (65, N'Uttar Pradesh', N'UP', N'IN', CAST(N'2026-02-18T08:41:57.2051295' AS DateTime2), CAST(N'2026-03-27T12:25:40.9500000' AS DateTime2), 1)
INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (67, N'Bihar', N'BR', N'IN', CAST(N'2026-02-18T09:10:35.0087743' AS DateTime2), CAST(N'2026-03-27T12:25:19.2733333' AS DateTime2), 1)
INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (68, N'Madhya Pradesh', N'MP', N'IN', CAST(N'2026-02-18T09:51:10.2460585' AS DateTime2), CAST(N'2026-03-27T12:25:28.6466667' AS DateTime2), 1)
INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (69, N'Himanchal Pradesh', N'HP', N'IN', CAST(N'2026-02-19T05:40:50.1426957' AS DateTime2), NULL, 1)
INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (70, N'ಮಹಾರಾಷ್ಟ್ರ', N'MH', N'IN', CAST(N'2026-02-19T06:50:03.2416508' AS DateTime2), CAST(N'2026-03-27T12:29:35.4033333' AS DateTime2), 0)
INSERT [dbo].[States] ([Id], [Name], [Code], [CountryCode], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (71, N'Uttarakhand', N'UK', N'IN', CAST(N'2026-03-19T10:06:24.3066667' AS DateTime2), CAST(N'2026-03-19T10:06:24.3066667' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[States] OFF
GO
SET IDENTITY_INSERT [dbo].[StreamLanguages] ON 

INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (1, 1, 50, N'Computer Science', N'Computer Science and Engineering', CAST(N'2026-02-20T06:44:12.4785229' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (2, 1, 49, N'कंप्यूटर विज्ञान', N'कंप्यूटर विज्ञान और इंजीनियरिंग', CAST(N'2026-02-20T06:44:12.4785302' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (3, 2, 50, N'History', N'History stream', CAST(N'2026-02-20T06:48:41.8016339' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (4, 2, 49, N'इतिहास', N'इतिहास स्ट्रीम', CAST(N'2026-02-20T06:48:41.8016341' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (5, 3, 50, N'Political Science', N'Political Science stream', CAST(N'2026-02-20T06:48:53.9714435' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (6, 3, 49, N'राजनीति विज्ञान', N'राजनीति विज्ञान स्ट्रीम', CAST(N'2026-02-20T06:48:53.9714437' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (7, 4, 50, N'English', N'English stream', CAST(N'2026-02-20T06:49:04.9515068' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (8, 4, 49, N'अंग्रेजी', N'अंग्रेजी स्ट्रीम', CAST(N'2026-02-20T06:49:04.9515070' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (9, 5, 50, N'Science', N'Science stream', CAST(N'2026-02-20T06:49:18.1674621' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (10, 5, 49, N'विज्ञान', N'विज्ञान स्ट्रीम', CAST(N'2026-02-20T06:49:18.1674623' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (11, 6, 50, N'Commerce', N'Commerce stream', CAST(N'2026-02-20T06:49:30.0431735' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (12, 6, 49, N'वाणिज्य', N'वाणिज्य स्ट्रीम', CAST(N'2026-02-20T06:49:30.0431738' AS DateTime2), NULL, 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (32, 7, 50, N'English / अंग्रेज़ी', N'Arts stream', CAST(N'2026-03-27T07:25:47.5300000' AS DateTime2), CAST(N'2026-03-27T07:25:47.5300000' AS DateTime2), 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (33, 7, 49, N'आर्ट्स एक', N'कला धारा', CAST(N'2026-03-27T07:25:47.5300000' AS DateTime2), CAST(N'2026-03-27T07:25:47.5300000' AS DateTime2), 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (34, 16, 50, N'English / अंग्रेज़ी', N'testing one', CAST(N'2026-03-27T07:30:22.6833333' AS DateTime2), CAST(N'2026-03-27T07:30:22.6833333' AS DateTime2), 1)
INSERT [dbo].[StreamLanguages] ([Id], [StreamId], [LanguageId], [Name], [Description], [CreatedAt], [UpdatedAt], [IsActive]) VALUES (35, 16, 49, N'परीक्षा', N'एक का परीक्षण', CAST(N'2026-03-27T07:30:22.6833333' AS DateTime2), CAST(N'2026-03-27T07:30:22.6833333' AS DateTime2), 1)
SET IDENTITY_INSERT [dbo].[StreamLanguages] OFF
GO
SET IDENTITY_INSERT [dbo].[Streams] ON 

INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (1, N'Computer Science', N'Computer Science and Engineering', 1, CAST(N'2026-02-20T06:44:12.4784078' AS DateTime2), CAST(N'2026-03-10T09:33:41.6600000' AS DateTime2), 1, N'कंप्यूटर विज्ञान')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (2, N'History', N'History stream', 4, CAST(N'2026-02-20T06:48:41.8016322' AS DateTime2), CAST(N'2026-03-10T09:33:41.6600000' AS DateTime2), 1, N'इतिहास')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (3, N'Political Science', N'Political Science stream', 4, CAST(N'2026-02-20T06:48:53.9714416' AS DateTime2), CAST(N'2026-03-10T09:33:41.6600000' AS DateTime2), 1, N'राजनीति विज्ञान')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (4, N'English', N'English stream', 4, CAST(N'2026-02-20T06:49:04.9515055' AS DateTime2), CAST(N'2026-03-10T09:33:41.6600000' AS DateTime2), 1, N'अंग्रेज़ी')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (5, N'Science', N'Science stream', 8, CAST(N'2026-02-20T06:49:18.1674602' AS DateTime2), CAST(N'2026-03-10T09:33:41.6600000' AS DateTime2), 1, N'विज्ञान')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (6, N'Commerce', N'Commerce stream', 8, CAST(N'2026-02-20T06:49:30.0431715' AS DateTime2), CAST(N'2026-03-10T09:33:41.6600000' AS DateTime2), 1, N'वाणिज्य')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (7, N'Arts', N'Arts stream', 8, CAST(N'2026-02-20T06:49:43.8275664' AS DateTime2), CAST(N'2026-03-27T07:25:47.5300000' AS DateTime2), 1, N'कला')
INSERT [dbo].[Streams] ([Id], [Name], [Description], [QualificationId], [CreatedAt], [UpdatedAt], [IsActive], [NameHi]) VALUES (16, N'test', N'testing one', 4, CAST(N'2026-03-19T10:30:14.9633333' AS DateTime2), CAST(N'2026-03-27T07:30:22.6833333' AS DateTime2), 1, NULL)
SET IDENTITY_INSERT [dbo].[Streams] OFF
GO
SET IDENTITY_INSERT [dbo].[SubjectLanguages] ON 

INSERT [dbo].[SubjectLanguages] ([Id], [SubjectId], [LanguageId], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (5, 6, 50, N'test ', N'test 2', 1, CAST(N'2026-03-27T11:56:01.0400000' AS DateTime2), CAST(N'2026-03-27T11:56:01.0400000' AS DateTime2))
INSERT [dbo].[SubjectLanguages] ([Id], [SubjectId], [LanguageId], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (6, 6, 49, N'परीक्षा', N'परीक्षण 2', 1, CAST(N'2026-03-27T11:56:01.0400000' AS DateTime2), CAST(N'2026-03-27T11:56:01.0400000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[SubjectLanguages] OFF
GO
SET IDENTITY_INSERT [dbo].[Subjects] ON 

INSERT [dbo].[Subjects] ([Id], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (1, N'Mathematics', N'Mathematics subject', 1, CAST(N'2026-02-24T14:36:52.2700000' AS DateTime2), CAST(N'2026-02-25T07:03:40.5634511' AS DateTime2))
INSERT [dbo].[Subjects] ([Id], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (2, N'Physics', N'Physics subject', 1, CAST(N'2026-02-24T14:36:52.2700000' AS DateTime2), CAST(N'2026-03-25T06:54:36.5433333' AS DateTime2))
INSERT [dbo].[Subjects] ([Id], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (3, N'Chemistry', N'Chemistry subject', 1, CAST(N'2026-02-24T14:36:52.2700000' AS DateTime2), CAST(N'2026-02-24T09:06:52.2700000' AS DateTime2))
INSERT [dbo].[Subjects] ([Id], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (5, N'Computer Science', N'Computer Science subject', 1, CAST(N'2026-02-24T14:36:52.2700000' AS DateTime2), CAST(N'2026-02-24T09:06:52.2700000' AS DateTime2))
INSERT [dbo].[Subjects] ([Id], [Name], [Description], [IsActive], [CreatedAt], [UpdatedAt]) VALUES (6, N'test ', N'test 2', 1, CAST(N'2026-02-25T07:03:16.7640839' AS DateTime2), CAST(N'2026-03-27T11:56:01.0400000' AS DateTime2))
SET IDENTITY_INSERT [dbo].[Subjects] OFF
GO
/****** Object:  Index [IX_Categories_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_IsActive] ON [dbo].[Categories]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_IsActive_Type]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_IsActive_Type] ON [dbo].[Categories]
(
	[IsActive] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_Key]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Categories_Key] ON [dbo].[Categories]
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_Status]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_Status] ON [dbo].[Categories]
(
	[Status] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Categories_Type]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Categories_Type] ON [dbo].[Categories]
(
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CmsContents_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_CmsContents_IsActive] ON [dbo].[CmsContents]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CmsContents_IsActive_Key]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_CmsContents_IsActive_Key] ON [dbo].[CmsContents]
(
	[IsActive] ASC,
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CmsContents_Key]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CmsContents_Key] ON [dbo].[CmsContents]
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_CmsContentTranslations_CmsContentId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_CmsContentTranslations_CmsContentId] ON [dbo].[CmsContentTranslations]
(
	[CmsContentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CmsContentTranslations_CmsContentId_LanguageCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_CmsContentTranslations_CmsContentId_LanguageCode] ON [dbo].[CmsContentTranslations]
(
	[CmsContentId] ASC,
	[LanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_CmsContentTranslations_LanguageCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_CmsContentTranslations_LanguageCode] ON [dbo].[CmsContentTranslations]
(
	[LanguageCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Countries_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Countries_CountryCode] ON [dbo].[Countries]
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Countries_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Countries_IsActive] ON [dbo].[Countries]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Countries_Iso2]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Countries_Iso2] ON [dbo].[Countries]
(
	[Iso2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExamLanguages_ExamId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ExamLanguages_ExamId_LanguageId] ON [dbo].[ExamLanguages]
(
	[ExamId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExamLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExamLanguages_LanguageId] ON [dbo].[ExamLanguages]
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExamQualifications_ExamId_QualificationId_StreamId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_ExamQualifications_ExamId_QualificationId_StreamId] ON [dbo].[ExamQualifications]
(
	[ExamId] ASC,
	[QualificationId] ASC,
	[StreamId] ASC
)
WHERE ([StreamId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExamQualifications_QualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExamQualifications_QualificationId] ON [dbo].[ExamQualifications]
(
	[QualificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ExamQualifications_StreamId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_ExamQualifications_StreamId] ON [dbo].[ExamQualifications]
(
	[StreamId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Exams_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Exams_CountryCode] ON [dbo].[Exams]
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Exams_CountryId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Exams_CountryId] ON [dbo].[Exams]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Exams_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Exams_IsActive] ON [dbo].[Exams]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Exams_IsActive_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Exams_IsActive_CountryCode] ON [dbo].[Exams]
(
	[IsActive] ASC,
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Exams_IsInternational]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Exams_IsInternational] ON [dbo].[Exams]
(
	[IsInternational] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Exams_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Exams_Name] ON [dbo].[Exams]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Languages_Code]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Languages_Code] ON [dbo].[Languages]
(
	[Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Languages_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Languages_IsActive] ON [dbo].[Languages]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Languages_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Languages_Name] ON [dbo].[Languages]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualificationLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_QualificationLanguages_LanguageId] ON [dbo].[QualificationLanguages]
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_QualificationLanguages_QualificationId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_QualificationLanguages_QualificationId_LanguageId] ON [dbo].[QualificationLanguages]
(
	[QualificationId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Qualifications_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Qualifications_CountryCode] ON [dbo].[Qualifications]
(
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Qualifications_CountryId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Qualifications_CountryId] ON [dbo].[Qualifications]
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Qualifications_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Qualifications_IsActive] ON [dbo].[Qualifications]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Qualifications_IsActive_CountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Qualifications_IsActive_CountryCode] ON [dbo].[Qualifications]
(
	[IsActive] ASC,
	[CountryCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Qualifications_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Qualifications_Name] ON [dbo].[Qualifications]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StateLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_StateLanguages_LanguageId] ON [dbo].[StateLanguages]
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StateLanguages_StateId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StateLanguages_StateId_LanguageId] ON [dbo].[StateLanguages]
(
	[StateId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_States_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_States_IsActive] ON [dbo].[States]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_States_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_States_Name] ON [dbo].[States]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StreamLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_StreamLanguages_LanguageId] ON [dbo].[StreamLanguages]
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_StreamLanguages_StreamId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_StreamLanguages_StreamId_LanguageId] ON [dbo].[StreamLanguages]
(
	[StreamId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Streams_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Streams_IsActive] ON [dbo].[Streams]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Streams_IsActive_QualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Streams_IsActive_QualificationId] ON [dbo].[Streams]
(
	[IsActive] ASC,
	[QualificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Streams_Name]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Streams_Name] ON [dbo].[Streams]
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Streams_QualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Streams_QualificationId] ON [dbo].[Streams]
(
	[QualificationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UQ_SubjectLanguages_Subject_Language]    Script Date: 4/3/2026 12:05:37 PM ******/
ALTER TABLE [dbo].[SubjectLanguages] ADD  CONSTRAINT [UQ_SubjectLanguages_Subject_Language] UNIQUE NONCLUSTERED 
(
	[SubjectId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubjectLanguages_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubjectLanguages_IsActive] ON [dbo].[SubjectLanguages]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubjectLanguages_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubjectLanguages_LanguageId] ON [dbo].[SubjectLanguages]
(
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubjectLanguages_SubjectId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubjectLanguages_SubjectId] ON [dbo].[SubjectLanguages]
(
	[SubjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SubjectLanguages_SubjectId_LanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_SubjectLanguages_SubjectId_LanguageId] ON [dbo].[SubjectLanguages]
(
	[SubjectId] ASC,
	[LanguageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Subjects_IsActive]    Script Date: 4/3/2026 12:05:37 PM ******/
CREATE NONCLUSTERED INDEX [IX_Subjects_IsActive] ON [dbo].[Subjects]
(
	[IsActive] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Categories] ADD  DEFAULT ('active') FOR [Status]
GO
ALTER TABLE [dbo].[CmsContents] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[CmsContents] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Countries] ADD  DEFAULT ((10)) FOR [PhoneLength]
GO
ALTER TABLE [dbo].[Countries] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Countries] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ExamLanguages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ExamLanguages] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[ExamQualifications] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[ExamQualifications] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Exams] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Exams] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Exams] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsInternational]
GO
ALTER TABLE [dbo].[Languages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Languages] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[QualificationLanguages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[QualificationLanguages] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Qualifications] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Qualifications] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[StateLanguages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[StateLanguages] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[StateLanguages] ADD  DEFAULT (N'') FOR [Name]
GO
ALTER TABLE [dbo].[States] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[States] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[StreamLanguages] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[StreamLanguages] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[Streams] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Streams] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsActive]
GO
ALTER TABLE [dbo].[SubjectLanguages] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[SubjectLanguages] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[SubjectLanguages] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[Subjects] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Subjects] ADD  DEFAULT (getutcdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Subjects] ADD  DEFAULT (getutcdate()) FOR [UpdatedAt]
GO
ALTER TABLE [dbo].[CmsContentTranslations]  WITH CHECK ADD  CONSTRAINT [FK_CmsContentTranslations_CmsContents_CmsContentId] FOREIGN KEY([CmsContentId])
REFERENCES [dbo].[CmsContents] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CmsContentTranslations] CHECK CONSTRAINT [FK_CmsContentTranslations_CmsContents_CmsContentId]
GO
ALTER TABLE [dbo].[ExamLanguages]  WITH CHECK ADD  CONSTRAINT [FK_ExamLanguages_Exams_ExamId] FOREIGN KEY([ExamId])
REFERENCES [dbo].[Exams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExamLanguages] CHECK CONSTRAINT [FK_ExamLanguages_Exams_ExamId]
GO
ALTER TABLE [dbo].[ExamLanguages]  WITH CHECK ADD  CONSTRAINT [FK_ExamLanguages_Languages_LanguageId] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExamLanguages] CHECK CONSTRAINT [FK_ExamLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[ExamQualifications]  WITH CHECK ADD  CONSTRAINT [FK_ExamQualifications_Exams_ExamId] FOREIGN KEY([ExamId])
REFERENCES [dbo].[Exams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExamQualifications] CHECK CONSTRAINT [FK_ExamQualifications_Exams_ExamId]
GO
ALTER TABLE [dbo].[ExamQualifications]  WITH CHECK ADD  CONSTRAINT [FK_ExamQualifications_Qualifications_QualificationId] FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualifications] ([Id])
GO
ALTER TABLE [dbo].[ExamQualifications] CHECK CONSTRAINT [FK_ExamQualifications_Qualifications_QualificationId]
GO
ALTER TABLE [dbo].[ExamQualifications]  WITH CHECK ADD  CONSTRAINT [FK_ExamQualifications_Streams_StreamId] FOREIGN KEY([StreamId])
REFERENCES [dbo].[Streams] ([Id])
GO
ALTER TABLE [dbo].[ExamQualifications] CHECK CONSTRAINT [FK_ExamQualifications_Streams_StreamId]
GO
ALTER TABLE [dbo].[QualificationLanguages]  WITH CHECK ADD  CONSTRAINT [FK_QualificationLanguages_Languages_LanguageId] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QualificationLanguages] CHECK CONSTRAINT [FK_QualificationLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[QualificationLanguages]  WITH CHECK ADD  CONSTRAINT [FK_QualificationLanguages_Qualifications_QualificationId] FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualifications] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[QualificationLanguages] CHECK CONSTRAINT [FK_QualificationLanguages_Qualifications_QualificationId]
GO
ALTER TABLE [dbo].[StateLanguages]  WITH CHECK ADD  CONSTRAINT [FK_StateLanguages_Languages_LanguageId] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StateLanguages] CHECK CONSTRAINT [FK_StateLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[StateLanguages]  WITH CHECK ADD  CONSTRAINT [FK_StateLanguages_States_StateId] FOREIGN KEY([StateId])
REFERENCES [dbo].[States] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StateLanguages] CHECK CONSTRAINT [FK_StateLanguages_States_StateId]
GO
ALTER TABLE [dbo].[States]  WITH NOCHECK ADD  CONSTRAINT [FK_States_Countries] FOREIGN KEY([CountryCode])
REFERENCES [dbo].[Countries] ([Iso2])
GO
ALTER TABLE [dbo].[States] CHECK CONSTRAINT [FK_States_Countries]
GO
ALTER TABLE [dbo].[StreamLanguages]  WITH CHECK ADD  CONSTRAINT [FK_StreamLanguages_Languages_LanguageId] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StreamLanguages] CHECK CONSTRAINT [FK_StreamLanguages_Languages_LanguageId]
GO
ALTER TABLE [dbo].[StreamLanguages]  WITH CHECK ADD  CONSTRAINT [FK_StreamLanguages_Streams_StreamId] FOREIGN KEY([StreamId])
REFERENCES [dbo].[Streams] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StreamLanguages] CHECK CONSTRAINT [FK_StreamLanguages_Streams_StreamId]
GO
ALTER TABLE [dbo].[Streams]  WITH CHECK ADD  CONSTRAINT [FK_Streams_Qualifications_QualificationId] FOREIGN KEY([QualificationId])
REFERENCES [dbo].[Qualifications] ([Id])
GO
ALTER TABLE [dbo].[Streams] CHECK CONSTRAINT [FK_Streams_Qualifications_QualificationId]
GO
ALTER TABLE [dbo].[SubjectLanguages]  WITH CHECK ADD  CONSTRAINT [FK_SubjectLanguages_Languages] FOREIGN KEY([LanguageId])
REFERENCES [dbo].[Languages] ([Id])
GO
ALTER TABLE [dbo].[SubjectLanguages] CHECK CONSTRAINT [FK_SubjectLanguages_Languages]
GO
ALTER TABLE [dbo].[SubjectLanguages]  WITH CHECK ADD  CONSTRAINT [FK_SubjectLanguages_Subjects] FOREIGN KEY([SubjectId])
REFERENCES [dbo].[Subjects] ([Id])
GO
ALTER TABLE [dbo].[SubjectLanguages] CHECK CONSTRAINT [FK_SubjectLanguages_Subjects]
GO
/****** Object:  StoredProcedure [dbo].[Category_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_Create]
    @NameEn NVARCHAR(100),
    @NameHi NVARCHAR(100) = NULL,
    @Key NVARCHAR(50),
    @Type NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @DisplayOrder INT = 0,
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO Categories (NameEn, NameHi, [Key], Type, Description, DisplayOrder, IsActive, CreatedAt, UpdatedAt)
    VALUES (@NameEn, @NameHi, @Key, @Type, @Description, @DisplayOrder, @IsActive, @CreatedAt, @UpdatedAt);
    
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[Category_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories
    SET 
        IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_GetActiveByType]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_GetActiveByType]
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NameEn,
        NameHi,
        [Key],
        Type,
        Description,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE Type = @Type AND IsActive = 1
    ORDER BY DisplayOrder, NameEn;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Category_GetActiveLocalized]
    @LanguageCode NVARCHAR(10) = 'en',
    @Type NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.Id,
        c.NameEn,
        c.NameHi,
        CASE 
            WHEN @LanguageCode = 'hi' AND COALESCE(c.NameHi, '') != '' THEN c.NameHi
            ELSE c.NameEn
        END AS Name,
        c.[Key],
        c.Type,
        c.Description,
        c.DisplayOrder,
        c.IsActive,
        c.CreatedAt,
        c.UpdatedAt
    FROM Categories c
    WHERE c.IsActive = 1
    AND (@Type IS NULL OR c.Type = @Type)
    ORDER BY c.DisplayOrder, c.NameEn;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NameEn,
        NameHi,
        [Key],
        Type,
        Description,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE IsActive = 1
    ORDER BY DisplayOrder, NameEn;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        Id,
        NameEn,
        NameHi,
        [Key],
        Type,
        Description,
        DisplayOrder,
        IsActive,
        CreatedAt,
        UpdatedAt
    FROM [dbo].[Categories] 
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Category_GetByIdLocalized]
    @Id INT,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.Id,
        c.NameEn,
        c.NameHi,
        CASE 
            WHEN @LanguageCode = 'hi' AND COALESCE(c.NameHi, '') != '' THEN c.NameHi
            ELSE c.NameEn
        END AS Name,
        c.[Key],
        c.Type,
        c.Description,
        c.DisplayOrder,
        c.IsActive,
        c.CreatedAt,
        c.UpdatedAt
    FROM Categories c
    WHERE c.Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Category_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories 
    SET IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Category_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories 
    SET IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Category_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if category exists
    IF NOT EXISTS (SELECT 1 FROM Categories WHERE Id = @Id)
    BEGIN
        RAISERROR('Category not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Categories
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END

GO
/****** Object:  StoredProcedure [dbo].[Category_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Category_Update]
    @Id INT,
    @NameEn NVARCHAR(100),
    @NameHi NVARCHAR(100) = NULL,
    @Key NVARCHAR(50),
    @Type NVARCHAR(50),
    @Description NVARCHAR(500) = NULL,
    @DisplayOrder INT = 0,
    @IsActive BIT = 1,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Categories
    SET 
        NameEn = @NameEn,
        NameHi = @NameHi,
        [Key] = @Key,
        Type = @Type,
        Description = @Description,
        DisplayOrder = @DisplayOrder,
        IsActive = @IsActive,
        UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_Create]
    @Key       NVARCHAR(100),
    @IsActive  BIT,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2,
    @Id        INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CmsContents ([Key], IsActive, CreatedAt, UpdatedAt)
    VALUES (@Key, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.CmsContents WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_ExistsByKey]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_ExistsByKey]
    @Key NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.CmsContents WHERE [Key] = @Key;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_ExistsByKeyExceptId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_ExistsByKeyExceptId]
    @Key       NVARCHAR(100),
    @ExcludeId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1) FROM dbo.CmsContents 
    WHERE [Key] = @Key AND Id != @ExcludeId;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt,
           t.Id AS TranslationId, t.CmsContentId, t.LanguageCode, t.Title, t.Content
    FROM dbo.CmsContents c
    LEFT JOIN dbo.CmsContentTranslations t ON c.Id = t.CmsContentId
    WHERE c.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt,
           t.Id AS TranslationId, t.CmsContentId, t.LanguageCode, t.Title, t.Content
    FROM dbo.CmsContents c
    LEFT JOIN dbo.CmsContentTranslations t ON c.Id = t.CmsContentId;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt,
           t.Id AS TranslationId, t.CmsContentId, t.LanguageCode, t.Title, t.Content
    FROM dbo.CmsContents c
    LEFT JOIN dbo.CmsContentTranslations t ON c.Id = t.CmsContentId
    WHERE c.Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_GetByKey]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_GetByKey]
    @Key NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT c.Id, c.[Key], c.IsActive, c.CreatedAt, c.UpdatedAt,
           t.Id AS TranslationId, t.CmsContentId, t.LanguageCode, t.Title, t.Content
    FROM dbo.CmsContents c
    LEFT JOIN dbo.CmsContentTranslations t ON c.Id = t.CmsContentId
    WHERE c.[Key] = @Key;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContent_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContent_Update]
    @Id        INT,
    @Key       NVARCHAR(100),
    @IsActive  BIT,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.CmsContents
    SET [Key] = @Key, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContentTranslation_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContentTranslation_Create]
    @CmsContentId INT,
    @LanguageCode NVARCHAR(10),
    @Title        NVARCHAR(500),
    @Content      NVARCHAR(MAX),
    @Id           INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.CmsContentTranslations (CmsContentId, LanguageCode, Title, Content)
    VALUES (@CmsContentId, @LanguageCode, @Title, @Content);
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[CmsContentTranslation_DeleteByCmsContentId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CmsContentTranslation_DeleteByCmsContentId]
    @CmsContentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.CmsContentTranslations WHERE CmsContentId = @CmsContentId;
END
GO
/****** Object:  StoredProcedure [dbo].[Country_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_Create]
    @Name                   NVARCHAR(100),
    @Iso2                   NVARCHAR(2),
    @CountryCode            NVARCHAR(5),
    @PhoneLength            INT = 10,
    @CurrencyCode           NVARCHAR(3) = '',
    @Image                  NVARCHAR(255) = NULL,
    @IsActive               BIT = 1,
    @CreatedAt              DATETIME2,
    @UpdatedAt              DATETIME2,
    @Id                     INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check for duplicate Iso2
    IF EXISTS (SELECT 1 FROM dbo.Countries WHERE Iso2 = @Iso2)
    BEGIN
        RAISERROR('Country with this ISO2 code already exists', 16, 1);
        RETURN -1;
    END
    
    INSERT INTO dbo.Countries (Name, Iso2, CountryCode, PhoneLength, CurrencyCode, Image, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Iso2, @CountryCode, @PhoneLength, @CurrencyCode, @Image, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[Country_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Countries WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Country_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Iso2, CountryCode, PhoneLength, CurrencyCode, Image, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries;
END
GO
/****** Object:  StoredProcedure [dbo].[Country_GetByCode]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_GetByCode]
    @Iso2 NVARCHAR(2)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Iso2, CountryCode, PhoneLength, CurrencyCode, Image, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries
    WHERE Iso2 = @Iso2;
END
GO
/****** Object:  StoredProcedure [dbo].[Country_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Iso2, CountryCode, PhoneLength, CurrencyCode, Image, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Countries
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Country_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Countries 
    SET IsActive = @IsActive, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Country_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Country_Update]
    @Id                     INT,
    @Name                   NVARCHAR(100),
    @Iso2                   NVARCHAR(2),
    @CountryCode            NVARCHAR(5),
    @PhoneLength            INT = 10,
    @CurrencyCode           NVARCHAR(3) = '',
    @Image                  NVARCHAR(255) = NULL,
    @IsActive               BIT,
    @UpdatedAt              DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check for duplicate Iso2 (excluding current record)
    IF EXISTS (SELECT 1 FROM dbo.Countries WHERE Iso2 = @Iso2 AND Id != @Id)
    BEGIN
        RAISERROR('Country with this ISO2 code already exists', 16, 1);
        RETURN -1;
    END
    
    UPDATE dbo.Countries
    SET Name = @Name, Iso2 = @Iso2, CountryCode = @CountryCode, PhoneLength = @PhoneLength,
        CurrencyCode = @CurrencyCode, Image = @Image, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_Create]
    @Name        NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10) = NULL,
    @MinAge      INT = NULL,
    @MaxAge      INT = NULL,
    @ImageUrl    NVARCHAR(500) = NULL,
    @IsInternational BIT = 0,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @RelationsJson NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Exams (Name, Description, CountryCode, MinAge, MaxAge, ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Name, @Description, @CountryCode, @MinAge, @MaxAge, @ImageUrl, @IsInternational, @IsActive, @CreatedAt, @UpdatedAt);
        SET @Id = SCOPE_IDENTITY();

        IF @NamesJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                LanguageId,
                Name,
                Description,
                1 as IsActive,
                @CreatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@NamesJson) 
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(150) '$.Name',
                Description NVARCHAR(1000) '$.Description'
            );
        END

        IF @RelationsJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                QualificationId,
                StreamId,
                1 as IsActive,
                @CreatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@RelationsJson) 
            WITH (
                QualificationId INT '$.QualificationId',
                StreamId INT '$.StreamId'
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY
        DELETE FROM dbo.ExamLanguages WHERE ExamId = @Id;
        DELETE FROM dbo.ExamQualifications WHERE ExamId = @Id;
        DELETE FROM dbo.Exams WHERE Id = @Id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    WHERE IsActive = 1
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetActiveLocalized]
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT e.Id, 
           COALESCE(el.Name, e.Name) as Name,
           COALESCE(el.Description, e.Description) as Description,
           e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
        AND el.LanguageId = CASE @LanguageCode 
            WHEN 'en' THEN 50 
            WHEN 'hi' THEN 49 
            ELSE 50 
        END
        AND el.IsActive = 1
    WHERE e.IsActive = 1
    ORDER BY COALESCE(el.Name, e.Name);
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActiveWithLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetActiveWithLanguages]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all active exams
    SELECT e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    WHERE e.IsActive = 1
    ORDER BY e.Name;
    
    -- Get all exam languages for active exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    WHERE el.IsActive = 1 AND e.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get all exam qualifications for active exams
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId,
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    INNER JOIN dbo.Exams e ON eq.ExamId = e.Id
    WHERE eq.IsActive = 1 AND e.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetActiveWithLanguagesLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetActiveWithLanguagesLocalized]
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all active exams with localized names
    SELECT e.Id, 
           COALESCE(el.Name, e.Name) as Name,
           COALESCE(el.Description, e.Description) as Description,
           e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
        AND el.LanguageId = CASE @LanguageCode 
            WHEN 'en' THEN 50 
            WHEN 'hi' THEN 49 
            ELSE 50 
        END
        AND el.IsActive = 1
    WHERE e.IsActive = 1
    ORDER BY COALESCE(el.Name, e.Name);
    
    -- Get all exam languages for active exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    WHERE el.IsActive = 1 AND e.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    -- Get all exam qualifications for active exams
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId,
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    INNER JOIN dbo.Exams e ON eq.ExamId = e.Id
    WHERE eq.IsActive = 1 AND e.IsActive = 1
    ORDER BY eq.ExamId, eq.QualificationId;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAllWithLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetAllWithLanguages]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get all exams
    SELECT e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    ORDER BY e.Name;
    
    -- Get all exam languages
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
END

GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAllWithLanguagesIncludingInactive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactive]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Description,
        e.CountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive,
        e.CreatedAt,
        e.UpdatedAt
    FROM [dbo].[Exams] e
    ORDER BY e.Name;
    
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        el.IsActive,
        el.CreatedAt,
        el.UpdatedAt
    FROM [dbo].[ExamLanguages] el
    WHERE el.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        eq.StreamId,
        eq.IsActive,
        eq.CreatedAt,
        eq.UpdatedAt
    FROM [dbo].[ExamQualifications] eq
    WHERE eq.IsActive = 1
    ORDER BY eq.ExamId;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetAllWithLanguagesIncludingInactiveLocalized]
    @LanguageCode NVARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        ISNULL(el.Name, e.Name) AS Name,
        e.Description,
        e.CountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive,
        e.CreatedAt,
        e.UpdatedAt
    FROM [dbo].[Exams] e
    LEFT JOIN [dbo].[ExamLanguages] el ON e.Id = el.ExamId 
        AND el.LanguageId = (SELECT Id FROM Languages WHERE Code = @LanguageCode AND IsActive = 1)
        AND el.IsActive = 1
    ORDER BY ISNULL(el.Name, e.Name);
    
    SELECT 
        el.Id,
        el.ExamId,
        el.LanguageId,
        el.Name,
        el.Description,
        el.IsActive,
        el.CreatedAt,
        el.UpdatedAt
    FROM [dbo].[ExamLanguages] el
    WHERE el.IsActive = 1
    ORDER BY el.ExamId, el.LanguageId;
    
    SELECT 
        eq.Id,
        eq.ExamId,
        eq.QualificationId,
        eq.StreamId,
        eq.IsActive,
        eq.CreatedAt,
        eq.UpdatedAt
    FROM [dbo].[ExamQualifications] eq
    WHERE eq.IsActive = 1
    ORDER BY eq.ExamId;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilter]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetByFilter]
    @CountryCode     NVARCHAR(10) = NULL,
    @QualificationId INT = NULL,
    @StreamId        INT = NULL,
    @MinAge          INT = NULL,
    @MaxAge          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT DISTINCT
        e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
        e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY e.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilterLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetByFilterLocalized]
    @LanguageCode     NVARCHAR(10) = NULL,
    @CountryCode      NVARCHAR(10) = NULL,
    @QualificationId  INT = NULL,
    @StreamId         INT = NULL,
    @MinAge           INT = NULL,
    @MaxAge           INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT
        e.Id, 
        COALESCE(el.Name, e.Name) as Name,
        COALESCE(el.Description, e.Description) as Description,
        e.CountryCode, e.MinAge, e.MaxAge,
        e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
        AND el.LanguageId = CASE @LanguageCode 
            WHEN 'en' THEN 50 
            WHEN 'hi' THEN 49 
            ELSE 50 
        END
        AND el.IsActive = 1
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY COALESCE(el.Name, e.Name);
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilterWithLanguages]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetByFilterWithLanguages]
    @CountryCode     NVARCHAR(10) = NULL,
    @QualificationId INT = NULL,
    @StreamId        INT = NULL,
    @MinAge          INT = NULL,
    @MaxAge          INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get filtered exams
    SELECT DISTINCT
        e.Id, e.Name, e.Description, e.CountryCode, e.MinAge, e.MaxAge,
        e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY e.Name;
    
    -- Get exam languages for filtered exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE el.IsActive = 1 AND e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY el.ExamId, el.LanguageId;
END

GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByFilterWithLanguagesLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetByFilterWithLanguagesLocalized]
    @LanguageCode     NVARCHAR(10) = NULL,
    @CountryCode      NVARCHAR(10) = NULL,
    @QualificationId  INT = NULL,
    @StreamId         INT = NULL,
    @MinAge           INT = NULL,
    @MaxAge           INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get filtered exams with localized names
    SELECT DISTINCT
        e.Id, 
        COALESCE(el.Name, e.Name) as Name,
        COALESCE(el.Description, e.Description) as Description,
        e.CountryCode, e.MinAge, e.MaxAge,
        e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
        AND el.LanguageId = CASE @LanguageCode 
            WHEN 'en' THEN 50 
            WHEN 'hi' THEN 49 
            ELSE 50 
        END
        AND el.IsActive = 1
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY COALESCE(el.Name, e.Name);
    
    -- Get exam languages for filtered exams
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    INNER JOIN dbo.Exams e ON el.ExamId = e.Id
    LEFT JOIN dbo.ExamQualifications eq ON e.Id = eq.ExamId
    WHERE el.IsActive = 1 AND e.IsActive = 1
    AND (@CountryCode IS NULL OR e.CountryCode = @CountryCode)
    AND (@MinAge IS NULL OR e.MinAge IS NULL OR e.MinAge <= @MinAge)
    AND (@MaxAge IS NULL OR e.MaxAge IS NULL OR e.MaxAge >= @MaxAge)
    AND (@QualificationId IS NULL OR eq.QualificationId = @QualificationId)
    AND (@StreamId IS NULL OR EXISTS (
        SELECT 1 FROM dbo.ExamQualifications eq2
        WHERE eq2.ExamId = e.Id AND eq2.StreamId = @StreamId
    ))
    ORDER BY el.ExamId, el.LanguageId;
END

GO
/****** Object:  StoredProcedure [dbo].[Exam_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetByIdLocalized]
    @Id INT,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.Description,
        e.CountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive,
        e.CreatedAt,
        e.UpdatedAt,
        el.LanguageId,
        el.Name AS LocalizedName,
        el.Description AS LocalizedDescription,
        l.Code AS LanguageCode
    FROM Exams e
    LEFT JOIN ExamLanguages el ON e.Id = el.ExamId AND el.IsActive = 1
    LEFT JOIN Languages l ON el.LanguageId = l.Id
    WHERE e.Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdWithQualifications]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_GetByIdWithQualifications]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRY
        SELECT 
            e.Id, 
            e.Name, 
            e.Description, 
           
            e.IsActive, 
            e.ImageUrl,
            e.IsInternational,
            e.CreatedAt, 
            e.UpdatedAt,
            -- Include ExamQualifications as a nested collection
            (
                SELECT 
                    eq.Id,
                    eq.ExamId,
                    eq.QualificationId,
                    eq.StreamId,
                    eq.IsActive,
                    eq.CreatedAt,
                    eq.UpdatedAt
                FROM ExamQualifications eq
                WHERE eq.ExamId = e.Id AND eq.IsActive = 1
                FOR JSON PATH
            ) AS ExamQualifications
        FROM Exams e
        WHERE e.Id = @Id;
    END TRY
    BEGIN CATCH
        SELECT NULL AS Id, 
               ERROR_MESSAGE() AS Message,
               ERROR_NUMBER() AS ErrorNumber;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdWithRelations]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetByIdWithRelations]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get exam details
    SELECT Id, Name, Description, CountryCode, MinAge, MaxAge,
           ImageUrl, IsInternational, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Exams
    WHERE Id = @Id;
    
    -- Get exam languages
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.ExamId = @Id AND el.IsActive = 1;
    
    -- Get exam qualifications
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    WHERE eq.ExamId = @Id AND eq.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_GetByIdWithRelationsLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[Exam_GetByIdWithRelationsLocalized]
    @Id INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Get exam details with localized names
    SELECT e.Id, 
           COALESCE(el.Name, e.Name) as Name,
           COALESCE(el.Description, e.Description) as Description,
           e.CountryCode, e.MinAge, e.MaxAge,
           e.ImageUrl, e.IsInternational, e.IsActive, e.CreatedAt, e.UpdatedAt
    FROM dbo.Exams e
    LEFT JOIN dbo.ExamLanguages el ON e.Id = el.ExamId 
        AND el.LanguageId = CASE @LanguageCode 
            WHEN 'en' THEN 50 
            WHEN 'hi' THEN 49 
            ELSE 50 
        END
        AND el.IsActive = 1
    WHERE e.Id = @Id;
    
    -- Get all exam languages for this exam
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.ExamId = @Id AND el.IsActive = 1;
    
    -- Get exam qualifications
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt
    FROM dbo.ExamQualifications eq
    WHERE eq.ExamId = @Id AND eq.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Exams
    SET IsActive = @IsActive, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Exams
    SET IsActive = 0, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Exam_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Exam_Update]
    @Id          INT,
    @Name        NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10) = NULL,
    @MinAge      INT = NULL,
    @MaxAge      INT = NULL,
    @ImageUrl    NVARCHAR(500) = NULL,
    @IsInternational BIT = 0,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @RelationsJson NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.Exams
        SET Name = @Name,
            Description = @Description,
            CountryCode = @CountryCode,
            MinAge = @MinAge,
            MaxAge = @MaxAge,
            ImageUrl = @ImageUrl,
            IsInternational = @IsInternational,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id;

        DELETE FROM dbo.ExamLanguages WHERE ExamId = @Id;
        IF @NamesJson IS NOT NULL
        BEGIN
            INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                LanguageId,
                Name,
                Description,
                1 as IsActive,
                @UpdatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@NamesJson) 
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(150) '$.Name',
                Description NVARCHAR(1000) '$.Description'
            );
        END

        DELETE FROM dbo.ExamQualifications WHERE ExamId = @Id;
        IF @RelationsJson IS NOT NULL
        BEGIN
            INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
            SELECT 
                @Id as ExamId,
                QualificationId,
                StreamId,
                1 as IsActive,
                @UpdatedAt as CreatedAt,
                @UpdatedAt as UpdatedAt
            FROM OPENJSON(@RelationsJson) 
            WITH (
                QualificationId INT '$.QualificationId',
                StreamId INT '$.StreamId'
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamLanguage_Create]
    @ExamId     INT,
    @LanguageId INT,
    @Name       NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @IsActive   BIT = 1,
    @CreatedAt  DATETIME2,
    @UpdatedAt  DATETIME2,
    @Id         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExamLanguages (ExamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @LanguageId, @Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamLanguage_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ExamLanguages WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_GetByExamId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamLanguage_GetByExamId]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.ExamId = @ExamId AND el.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_GetByLanguageId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamLanguage_GetByLanguageId]
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT el.Id, el.ExamId, el.LanguageId, el.Name, el.Description, 
           el.IsActive, el.CreatedAt, el.UpdatedAt,
           l.Code as LanguageCode, l.Name as LanguageName
    FROM dbo.ExamLanguages el
    INNER JOIN dbo.Languages l ON el.LanguageId = l.Id
    WHERE el.LanguageId = @LanguageId AND el.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[ExamLanguage_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamLanguage_Update]
    @Id         INT,
    @Name       NVARCHAR(150),
    @Description NVARCHAR(1000) = NULL,
    @IsActive   BIT,
    @UpdatedAt  DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.ExamLanguages
    SET Name = @Name, Description = @Description, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[ExamQualification_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamQualification_Create]
    @ExamId         INT,
    @QualificationId INT,
    @StreamId       INT = NULL,
    @IsActive       BIT = 1,
    @CreatedAt      DATETIME2,
    @UpdatedAt      DATETIME2,
    @Id             INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.ExamQualifications (ExamId, QualificationId, StreamId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@ExamId, @QualificationId, @StreamId, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[ExamQualification_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamQualification_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.ExamQualifications WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[ExamQualification_GetByExamId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
CREATE PROCEDURE [dbo].[ExamQualification_GetByExamId]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT eq.Id, eq.ExamId, eq.QualificationId, eq.StreamId, 
           eq.IsActive, eq.CreatedAt, eq.UpdatedAt,
           q.Name as QualificationName,
           s.Name as StreamName
    FROM dbo.ExamQualifications eq
    LEFT JOIN dbo.Qualifications q ON eq.QualificationId = q.Id
    LEFT JOIN dbo.Streams s ON eq.StreamId = s.Id
    WHERE eq.ExamId = @ExamId AND eq.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Language_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Language_Create]
    @Name       NVARCHAR(100),
    @Code       NVARCHAR(10),
    @IsActive   BIT,
    @CreatedAt  DATETIME2,
    @UpdatedAt  DATETIME2,
    @Id         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Languages (Name, Code, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[Language_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Language_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Languages WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Language_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Language_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Languages
    WHERE IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Language_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Language_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Languages;
END
GO
/****** Object:  StoredProcedure [dbo].[Language_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Language_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Languages
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Language_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Language_Update]
    @Id         INT,
    @Name       NVARCHAR(100),
    @Code       NVARCHAR(10),
    @IsActive   BIT,
    @UpdatedAt  DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Languages
    SET Name = @Name, Code = @Code, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_Create]
    @Name        NVARCHAR(200),
    @NameHi      NVARCHAR(100) = NULL,
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Qualifications (Name, NameHi, Description, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @NameHi, @Description, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();

    IF @NamesJson IS NOT NULL AND @Id > 0
    BEGIN
        INSERT INTO dbo.QualificationLanguages (QualificationId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as QualificationId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @CreatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Qualifications WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameHi, Description, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActiveByCountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetActiveByCountryCode]
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE IsActive = 1 AND CountryCode = @CountryCode;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActiveByCountryCodeLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetActiveByCountryCodeLocalized]
    @CountryCode NVARCHAR(10),
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, COALESCE(NameHi, Name) as Name, NameHi, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive
        FROM dbo.Qualifications
        WHERE IsActive = 1 AND CountryCode = @CountryCode
        ORDER BY COALESCE(NameHi, Name);
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, CountryCode, CountryId, CreatedAt, UpdatedAt, IsActive
        FROM dbo.Qualifications
        WHERE IsActive = 1 AND CountryCode = @CountryCode
        ORDER BY Name;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetActiveLocalized]
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT 
            q.Id, 
            COALESCE(q.NameHi, q.Name) as Name, 
            q.NameHi, 
            q.Description, 
            q.CountryCode, 
            q.CountryId, 
            q.CreatedAt, 
            q.UpdatedAt, 
            q.IsActive,
            (
                SELECT ql.LanguageId, l.Code as LanguageCode, l.Name as LanguageName, ql.Name, ql.Description
                FROM dbo.QualificationLanguages ql
                INNER JOIN dbo.Languages l ON ql.LanguageId = l.Id
                WHERE ql.QualificationId = q.Id AND ql.IsActive = 1
                FOR JSON PATH
            ) as Names
        FROM dbo.Qualifications q
        WHERE q.IsActive = 1
        ORDER BY COALESCE(q.NameHi, q.Name);
    END
    ELSE
    BEGIN
        SELECT 
            q.Id, 
            q.Name, 
            q.NameHi, 
            q.Description, 
            q.CountryCode, 
            q.CountryId, 
            q.CreatedAt, 
            q.UpdatedAt, 
            q.IsActive,
            (
                SELECT ql.LanguageId, l.Code as LanguageCode, l.Name as LanguageName, ql.Name, ql.Description
                FROM dbo.QualificationLanguages ql
                INNER JOIN dbo.Languages l ON ql.LanguageId = l.Id
                WHERE ql.QualificationId = q.Id AND ql.IsActive = 1
                FOR JSON PATH
            ) as Names
        FROM dbo.Qualifications q
        WHERE q.IsActive = 1
        ORDER BY q.Name;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameHi, Description, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Qualifications
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetByIdLocalized]
    @Id INT,
    @LanguageCode NVARCHAR(10) = 'en'
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        q.Id,
        q.Name,
        q.Description,
        q.CountryCode,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt,
        ql.LanguageId,
        ql.Name AS LocalizedName,
        ql.Description AS LocalizedDescription,
        l.Code AS LanguageCode
    FROM Qualifications q
    LEFT JOIN QualificationLanguages ql ON q.Id = ql.QualificationId AND ql.IsActive = 1
    LEFT JOIN Languages l ON ql.LanguageId = l.Id
    WHERE q.Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_GetByIds]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_GetByIds]
    @Ids NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        q.Id,
        q.Name,
        q.NameHi,
        q.Description,
        q.CountryCode,
        q.IsActive,
        q.CreatedAt,
        q.UpdatedAt
    FROM dbo.Qualifications q
    INNER JOIN STRING_SPLIT(@Ids, ',') ids ON TRY_CAST(ids.value AS INT) = q.Id
    WHERE q.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_HardDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_HardDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Qualifications WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_HasRelatedStreams]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_HasRelatedStreams]
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.Streams
    WHERE QualificationId = @QualificationId;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Qualifications 
    SET IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Qualifications 
    SET IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Qualification_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Qualification_Update]
    @Id          INT,
    @Name        NVARCHAR(200),
    @NameHi      NVARCHAR(100) = NULL,
    @Description NVARCHAR(1000) = NULL,
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Qualifications
    SET Name = @Name, NameHi = @NameHi, Description = @Description, CountryCode = @CountryCode, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    -- Replace language rows when provided
    IF @NamesJson IS NOT NULL
    BEGIN
        DELETE FROM dbo.QualificationLanguages WHERE QualificationId = @Id;

        INSERT INTO dbo.QualificationLanguages (QualificationId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as QualificationId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @UpdatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[QualificationLanguage_GetByQualificationIds]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[QualificationLanguage_GetByQualificationIds]
    @QualificationIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        ql.Id,
        ql.QualificationId,
        ql.LanguageId,
        ql.Name,
        ql.Description,
        ql.IsActive,
        ql.CreatedAt,
        ql.UpdatedAt,
        l.Id,
        l.Code,
        l.Name
    FROM dbo.QualificationLanguages ql
    LEFT JOIN dbo.Languages l ON l.Id = ql.LanguageId
    INNER JOIN STRING_SPLIT(@QualificationIds, ',') ids ON TRY_CAST(ids.value AS INT) = ql.QualificationId
    WHERE ql.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[State_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_Create]
    @Name        NVARCHAR(100),
    @Code        NVARCHAR(10),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.States (Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Code, @CountryCode, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();

    IF @NamesJson IS NOT NULL AND @Id > 0
    BEGIN
        INSERT INTO dbo.StateLanguages (StateId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StateId,
            LanguageId,
            Name,
            1 as IsActive,
            @CreatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(100) '$.Name'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[State_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.States WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetActiveByCountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetActiveByCountryCode]
    @CountryCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE IsActive = 1 AND CountryCode = @CountryCode;
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetActiveByCountryCodeLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetActiveByCountryCodeLocalized]
    @CountryCode NVARCHAR(10),
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.Id,
        ISNULL(sl.Name, s.Name) AS Name,
        s.Code,
        s.CountryCode,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM dbo.States s
    LEFT JOIN dbo.Languages l ON l.Code = @LanguageCode AND l.IsActive = 1
    LEFT JOIN dbo.StateLanguages sl ON sl.StateId = s.Id AND sl.LanguageId = l.Id AND sl.IsActive = 1
    WHERE s.IsActive = 1 AND s.CountryCode = @CountryCode
    ORDER BY ISNULL(sl.Name, s.Name);
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetActiveLocalized]
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.Id,
        ISNULL(sl.Name, s.Name) AS Name,
        s.Code,
        s.CountryCode,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM dbo.States s
    LEFT JOIN dbo.Languages l ON l.Code = @LanguageCode AND l.IsActive = 1
    LEFT JOIN dbo.StateLanguages sl ON sl.StateId = s.Id AND sl.LanguageId = l.Id AND sl.IsActive = 1
    WHERE s.IsActive = 1
    ORDER BY ISNULL(sl.Name, s.Name);
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States;
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetByCountryCode]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[State_GetByCountryCode]
    @CountryCode NVARCHAR(10),
    @LanguageId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageId IS NOT NULL
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id AND sl.LanguageId = @LanguageId 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        WHERE s.CountryCode = @CountryCode
        ORDER BY s.Name;
    END
    ELSE
    BEGIN
        SELECT 
            s.Id,
            s.Name,
            s.Code,
            s.CountryCode,
            s.IsActive,
            s.CreatedAt,
            s.UpdatedAt,
            (SELECT sl.LanguageId, sl.Name, sl.IsActive 
             FROM StateLanguages sl 
             WHERE sl.StateId = s.Id 
             FOR JSON PATH) AS Names
        FROM [dbo].[States] s
        WHERE s.CountryCode = @CountryCode
        ORDER BY s.Name;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetByIdLocalized]
    @Id INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT
        s.Id,
        ISNULL(sl.Name, s.Name) AS Name,
        s.Code,
        s.CountryCode,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM dbo.States s
    LEFT JOIN dbo.Languages l ON l.Code = @LanguageCode AND l.IsActive = 1
    LEFT JOIN dbo.StateLanguages sl ON sl.StateId = s.Id AND sl.LanguageId = l.Id AND sl.IsActive = 1
    WHERE s.Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[State_GetWithEmptyNames]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_GetWithEmptyNames]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Code, CountryCode, IsActive, CreatedAt, UpdatedAt
    FROM dbo.States
    WHERE Name IS NULL OR Name = '';
END
GO
/****** Object:  StoredProcedure [dbo].[State_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.States
    SET IsActive = @IsActive, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[State_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.States
    SET IsActive = 0, UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[State_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[State_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if state exists
    IF NOT EXISTS (SELECT 1 FROM States WHERE Id = @Id)
    BEGIN
        RAISERROR('State not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE States
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[State_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[State_Update]
    @Id          INT,
    @Name        NVARCHAR(100),
    @Code        NVARCHAR(10),
    @CountryCode NVARCHAR(10),
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.States
    SET Name = @Name, Code = @Code, CountryCode = @CountryCode, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    -- Replace language rows when provided
    IF @NamesJson IS NOT NULL
    BEGIN
        DELETE FROM dbo.StateLanguages WHERE StateId = @Id;

        INSERT INTO dbo.StateLanguages (StateId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StateId,
            LanguageId,
            Name,
            1 as IsActive,
            @UpdatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(100) '$.Name'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[StateLanguage_GetByStateIds]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[StateLanguage_GetByStateIds]
    @StateIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sl.Id,
        sl.StateId,
        sl.LanguageId,
        sl.Name,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        l.Code AS LanguageCode,
        l.Name AS LanguageName
    FROM dbo.StateLanguages sl
    LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
    INNER JOIN STRING_SPLIT(@StateIds, ',') ids ON TRY_CAST(ids.value AS INT) = sl.StateId
    WHERE sl.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_Create]
    @Name            NVARCHAR(200),
    @Description     NVARCHAR(1000) = NULL,
    @QualificationId INT,
    @IsActive        BIT = 1,
    @NamesJson       NVARCHAR(MAX) = NULL,
    @CreatedAt       DATETIME2,
    @UpdatedAt       DATETIME2,
    @Id              INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Streams (Name, Description, QualificationId, IsActive, CreatedAt, UpdatedAt)
    VALUES (@Name, @Description, @QualificationId, @IsActive, @CreatedAt, @UpdatedAt);
    SET @Id = SCOPE_IDENTITY();

    IF @NamesJson IS NOT NULL AND @Id > 0
    BEGIN
        INSERT INTO dbo.StreamLanguages (StreamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StreamId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @CreatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Streams WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams
    WHERE IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActiveByQualificationId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetActiveByQualificationId]
    @QualificationId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams
    WHERE IsActive = 1 AND QualificationId = @QualificationId;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActiveByQualificationIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetActiveByQualificationIdLocalized]
    @QualificationId INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1 AND QualificationId = @QualificationId AND NameHi IS NOT NULL AND NameHi != '';
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1 AND QualificationId = @QualificationId;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetActiveLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetActiveLocalized]
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1 AND NameHi IS NOT NULL AND NameHi != '';
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE IsActive = 1;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAllDetailed]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Stream_GetAllDetailed]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Result Set 1: Main Stream Data
    SELECT 
        s.Id as StreamId,
        s.Name as StreamName,
        s.Description as StreamDescription,
        s.QualificationId,
        s.IsActive as StreamIsActive,
        s.CreatedAt as StreamCreatedAt,
        s.UpdatedAt as StreamUpdatedAt,
        q.Name as QualificationName,
        q.Description as QualificationDescription,
        q.CountryCode,
        c.Name as CountryName
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[Countries] c ON q.CountryCode = c.CountryCode
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND c.IsActive = 1
    ORDER BY c.Name, q.Name, s.Name;
    
    -- Result Set 2: Stream Languages
    SELECT 
        sl.StreamId,
        sl.LanguageId,
        l.Code as LanguageCode,
        l.Name as LanguageName,
        sl.Name as StreamLocalName,
        sl.Description as StreamLocalDescription,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt
    FROM [dbo].[StreamLanguages] sl
    INNER JOIN [dbo].[Streams] s ON sl.StreamId = s.Id
    INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
    WHERE sl.IsActive = 1 AND s.IsActive = 1 AND l.IsActive = 1
    ORDER BY sl.StreamId, l.Name;
    
    -- Result Set 3: Qualification Languages
    SELECT 
        ql.QualificationId,
        ql.LanguageId,
        l.Code as LanguageCode,
        l.Name as LanguageName,
        ql.Name as QualificationLocalName,
        ql.Description as QualificationLocalDescription,
        ql.IsActive,
        ql.CreatedAt,
        ql.UpdatedAt
    FROM [dbo].[QualificationLanguages] ql
    INNER JOIN [dbo].[Qualifications] q ON ql.QualificationId = q.Id
    INNER JOIN [dbo].[Languages] l ON ql.LanguageId = l.Id
    WHERE ql.IsActive = 1 AND q.IsActive = 1 AND l.IsActive = 1
    ORDER BY ql.QualificationId, l.Name;
    
    -- Result Set 4: Related Exams
    SELECT 
        s.Id as StreamId,
        s.Name as StreamName,
        e.Id as ExamId,
        e.Name as ExamName,
        e.Description as ExamDescription,
        e.CountryCode as ExamCountryCode,
        e.MinAge,
        e.MaxAge,
        e.ImageUrl,
        e.IsInternational,
        e.IsActive as ExamIsActive,
        e.CreatedAt as ExamCreatedAt,
        e.UpdatedAt as ExamUpdatedAt
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[ExamQualifications] eq ON q.Id = eq.QualificationId
    INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND e.IsActive = 1 
    AND eq.IsActive = 1 AND (eq.StreamId IS NULL OR eq.StreamId = s.Id)
    ORDER BY s.Name, e.Name;
END

GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAllSimple]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Stream_GetAllSimple]
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.Description,
        s.QualificationId,
        q.Name as QualificationName,
        q.CountryCode,
        c.Name as CountryName,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[Countries] c ON q.CountryCode = c.CountryCode
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND c.IsActive = 1
    ORDER BY c.Name, q.Name, s.Name;
END

GO
/****** Object:  StoredProcedure [dbo].[Stream_GetAllWithData]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Stream_GetAllWithData]
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Main Stream Data with Qualification and Language Information
    SELECT 
        s.Id as StreamId,
        s.Name as StreamName,
        s.Description as StreamDescription,
        s.QualificationId,
        s.IsActive as StreamIsActive,
        s.CreatedAt as StreamCreatedAt,
        s.UpdatedAt as StreamUpdatedAt,
        
        -- Qualification Information
        q.Id as QualificationId,
        q.Name as QualificationName,
        q.Description as QualificationDescription,
        q.CountryCode,
        q.IsActive as QualificationIsActive,
        q.CreatedAt as QualificationCreatedAt,
        q.UpdatedAt as QualificationUpdatedAt,
        
        -- Country Information
        c.Id as CountryId,
        c.Name as CountryName,
        c.Code as CountryCode,
        c.IsActive as CountryIsActive,
        c.CreatedAt as CountryCreatedAt,
        c.UpdatedAt as CountryUpdatedAt,
        
        -- Stream Languages (JSON format for multiple languages)
        (
            SELECT 
                sl.LanguageId,
                l.Code as LanguageCode,
                l.Name as LanguageName,
                sl.Name as StreamLocalName,
                sl.Description as StreamLocalDescription
            FROM [dbo].[StreamLanguages] sl
            INNER JOIN [dbo].[Languages] l ON sl.LanguageId = l.Id
            WHERE sl.StreamId = s.Id AND sl.IsActive = 1 AND l.IsActive = 1
            FOR JSON PATH
        ) as StreamLanguages,
        
        -- Qualification Languages (JSON format for multiple languages)
        (
            SELECT 
                ql.LanguageId,
                l.Code as LanguageCode,
                l.Name as LanguageName,
                ql.Name as QualificationLocalName,
                ql.Description as QualificationLocalDescription
            FROM [dbo].[QualificationLanguages] ql
            INNER JOIN [dbo].[Languages] l ON ql.LanguageId = l.Id
            WHERE ql.QualificationId = q.Id AND ql.IsActive = 1 AND l.IsActive = 1
            FOR JSON PATH
        ) as QualificationLanguages,
        
        -- Related Exams Count
        (
            SELECT COUNT(*)
            FROM [dbo].[ExamQualifications] eq
            INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
            WHERE eq.QualificationId = q.Id 
            AND (eq.StreamId IS NULL OR eq.StreamId = s.Id)
            AND eq.IsActive = 1 AND e.IsActive = 1
        ) as RelatedExamsCount,
        
        -- Related Exams (JSON format)
        (
            SELECT 
                e.Id as ExamId,
                e.Name as ExamName,
                e.Description as ExamDescription,
                e.CountryCode as ExamCountryCode,
                e.MinAge,
                e.MaxAge,
                e.ImageUrl,
                e.IsInternational,
                e.IsActive as ExamIsActive,
                e.CreatedAt as ExamCreatedAt,
                e.UpdatedAt as ExamUpdatedAt
            FROM [dbo].[ExamQualifications] eq
            INNER JOIN [dbo].[Exams] e ON eq.ExamId = e.Id
            WHERE eq.QualificationId = q.Id 
            AND (eq.StreamId IS NULL OR eq.StreamId = s.Id)
            AND eq.IsActive = 1 AND e.IsActive = 1
            FOR JSON PATH
        ) as RelatedExams
        
    FROM [dbo].[Streams] s
    INNER JOIN [dbo].[Qualifications] q ON s.QualificationId = q.Id
    INNER JOIN [dbo].[Countries] c ON q.CountryCode = c.CountryCode
    WHERE s.IsActive = 1 AND q.IsActive = 1 AND c.IsActive = 1
    ORDER BY c.Name, q.Name, s.Name;
END

GO
/****** Object:  StoredProcedure [dbo].[Stream_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, QualificationId, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Streams
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_GetByIdLocalized]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_GetByIdLocalized]
    @Id INT,
    @LanguageCode NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    
    IF @LanguageCode = 'hi'
    BEGIN
        SELECT Id, NameHi as Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE Id = @Id AND IsActive = 1 AND NameHi IS NOT NULL AND NameHi != '';
    END
    ELSE
    BEGIN
        SELECT Id, Name, NameHi, Description, QualificationId, IsActive, CreatedAt, UpdatedAt
        FROM dbo.Streams
        WHERE Id = @Id AND IsActive = 1;
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_SetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_SetActive]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Streams 
    SET IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_SoftDelete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_SoftDelete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Streams 
    SET IsActive = 0,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Stream_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Stream_Update]
    @Id              INT,
    @Name            NVARCHAR(200),
    @Description     NVARCHAR(1000) = NULL,
    @QualificationId INT,
    @IsActive        BIT,
    @NamesJson       NVARCHAR(MAX) = NULL,
    @UpdatedAt       DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.Streams
    SET Name = @Name, Description = @Description, QualificationId = @QualificationId, IsActive = @IsActive, UpdatedAt = @UpdatedAt
    WHERE Id = @Id;

    IF @NamesJson IS NOT NULL
    BEGIN
        DELETE FROM dbo.StreamLanguages WHERE StreamId = @Id;

        INSERT INTO dbo.StreamLanguages (StreamId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
        SELECT
            @Id as StreamId,
            LanguageId,
            Name,
            Description,
            1 as IsActive,
            @UpdatedAt as CreatedAt,
            @UpdatedAt as UpdatedAt
        FROM OPENJSON(@NamesJson)
        WITH (
            LanguageId INT '$.LanguageId',
            Name NVARCHAR(200) '$.Name',
            Description NVARCHAR(1000) '$.Description'
        );
    END
END
GO
/****** Object:  StoredProcedure [dbo].[StreamLanguage_GetByStreamIds]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[StreamLanguage_GetByStreamIds]
    @StreamIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sl.Id,
        sl.StreamId,
        sl.LanguageId,
        sl.Name,
        sl.Description,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        l.Id,
        l.Code,
        l.Name
    FROM dbo.StreamLanguages sl
    LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
    INNER JOIN STRING_SPLIT(@StreamIds, ',') ids ON TRY_CAST(ids.value AS INT) = sl.StreamId
    WHERE sl.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_Create]
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @IsActive    BIT = 1,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @CreatedAt   DATETIME2,
    @UpdatedAt   DATETIME2,
    @Id          INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        INSERT INTO dbo.Subjects (Name, Description, IsActive, CreatedAt, UpdatedAt)
        VALUES (@Name, @Description, @IsActive, @CreatedAt, @UpdatedAt);
        SET @Id = SCOPE_IDENTITY();

        IF @NamesJson IS NOT NULL AND @Id > 0
        BEGIN
            INSERT INTO dbo.SubjectLanguages (SubjectId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT
                @Id,
                LanguageId,
                Name,
                Description,
                ISNULL(IsActive, 1),
                @CreatedAt,
                @UpdatedAt
            FROM OPENJSON(@NamesJson)
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(200) '$.Name',
                Description NVARCHAR(1000) '$.Description',
                IsActive BIT '$.IsActive'
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM dbo.Subjects WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_Exists]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_Exists]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(1)
    FROM dbo.Subjects
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetActive]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_GetActive]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Subjects
    WHERE IsActive = 1
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetActiveByLanguage]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Subject_GetActiveByLanguage]
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    WHERE s.IsActive = 1
      AND EXISTS (
          SELECT 1 FROM SubjectLanguages sl 
          WHERE sl.SubjectId = s.Id 
            AND sl.LanguageId = @LanguageId 
            AND sl.IsActive = 1
      )
    ORDER BY s.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_GetAll]
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Subjects
    ORDER BY Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetAllByLanguage]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Subject_GetAllByLanguage]
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    WHERE s.IsActive = 1
      AND EXISTS (
          SELECT 1 FROM SubjectLanguages sl 
          WHERE sl.SubjectId = s.Id 
            AND sl.LanguageId = @LanguageId 
            AND sl.IsActive = 1
      )
    ORDER BY s.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT Id, Name, Description, IsActive, CreatedAt, UpdatedAt
    FROM dbo.Subjects
    WHERE Id = @Id;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetByIdByLanguage]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Subject_GetByIdByLanguage]
    @Id INT,
    @LanguageId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        s.Name,
        s.Description,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [dbo].[Subjects] s
    WHERE s.Id = @Id
      AND s.IsActive = 1
      AND EXISTS (
          SELECT 1 FROM SubjectLanguages sl 
          WHERE sl.SubjectId = s.Id 
            AND sl.LanguageId = @LanguageId 
            AND sl.IsActive = 1
      );
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_GetByStreamId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Subject_GetByStreamId]
    @StreamId INT,
    @LanguageId INT = 1
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        s.Id,
        ISNULL(sl.Name, s.Name) AS Name,
        ISNULL(sl.Description, s.Description) AS Description,
        s.StreamId,
        st.Name AS StreamName,
        s.IsActive,
        s.CreatedAt,
        s.UpdatedAt
    FROM [Subjects] s
    LEFT JOIN [SubjectLanguages] sl ON s.Id = sl.SubjectId AND sl.LanguageId = @LanguageId AND sl.IsActive = 1
    LEFT JOIN [Streams] st ON s.StreamId = st.Id
    WHERE s.StreamId = @StreamId AND s.IsActive = 1
    ORDER BY ISNULL(sl.Name, s.Name);
END;
GO
/****** Object:  StoredProcedure [dbo].[Subject_ToggleStatus]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Subject_ToggleStatus]
    @Id INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Check if subject exists
    IF NOT EXISTS (SELECT 1 FROM Subjects WHERE Id = @Id)
    BEGIN
        RAISERROR('Subject not found', 16, 1);
        RETURN -1;
    END
    
    UPDATE Subjects
    SET 
        IsActive = @IsActive,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @Id;
    
    RETURN 0;
END
GO
/****** Object:  StoredProcedure [dbo].[Subject_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Subject_Update]
    @Id          INT,
    @Name        NVARCHAR(200),
    @Description NVARCHAR(1000) = NULL,
    @IsActive    BIT,
    @NamesJson   NVARCHAR(MAX) = NULL,
    @UpdatedAt   DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    BEGIN TRY
        UPDATE dbo.Subjects
        SET Name = @Name,
            Description = @Description,
            IsActive = @IsActive,
            UpdatedAt = @UpdatedAt
        WHERE Id = @Id;

        IF @NamesJson IS NOT NULL
        BEGIN
            DELETE FROM dbo.SubjectLanguages WHERE SubjectId = @Id;

            INSERT INTO dbo.SubjectLanguages (SubjectId, LanguageId, Name, Description, IsActive, CreatedAt, UpdatedAt)
            SELECT
                @Id,
                LanguageId,
                Name,
                Description,
                ISNULL(IsActive, 1),
                @UpdatedAt,
                @UpdatedAt
            FROM OPENJSON(@NamesJson)
            WITH (
                LanguageId INT '$.LanguageId',
                Name NVARCHAR(200) '$.Name',
                Description NVARCHAR(1000) '$.Description',
                IsActive BIT '$.IsActive'
            );
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Create]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Create]
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(200),
    @IsActive BIT = 1,
    @CreatedAt DATETIME2,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        INSERT INTO SubjectLanguages (SubjectId, LanguageId, Name, IsActive, CreatedAt, UpdatedAt)
        OUTPUT INSERTED.Id
        VALUES (@SubjectId, @LanguageId, @Name, @IsActive, @CreatedAt, @UpdatedAt);
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Delete]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages 
        SET IsActive = 0, UpdatedAt = GETUTCDATE()
        WHERE Id = @Id;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetById]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetById]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.Id = @Id AND sl.IsActive = 1;
END
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetBySubjectId]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_GetBySubjectId]
    @SubjectId INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT sl.Id, sl.SubjectId, sl.LanguageId, sl.Name, sl.IsActive, sl.CreatedAt, sl.UpdatedAt,
           l.Id as LanguageId, l.Code as LanguageCode, l.Name as LanguageName
    FROM SubjectLanguages sl WITH (NOLOCK)
    INNER JOIN Languages l WITH (NOLOCK) ON sl.LanguageId = l.Id
    WHERE sl.SubjectId = @SubjectId
    ORDER BY l.Name;
END
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_GetBySubjectIds]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SubjectLanguage_GetBySubjectIds]
    @SubjectIds NVARCHAR(MAX),
    @LanguageId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        sl.Id,
        sl.SubjectId,
        sl.LanguageId,
        sl.Name,
        sl.Description,
        sl.IsActive,
        sl.CreatedAt,
        sl.UpdatedAt,
        l.Id,
        l.Name,
        l.Code
    FROM dbo.SubjectLanguages sl
    LEFT JOIN dbo.Languages l ON l.Id = sl.LanguageId
    INNER JOIN STRING_SPLIT(@SubjectIds, ',') ids ON TRY_CAST(ids.value AS INT) = sl.SubjectId
    WHERE sl.IsActive = 1
      AND (@LanguageId IS NULL OR sl.LanguageId = @LanguageId);
END
GO
/****** Object:  StoredProcedure [dbo].[SubjectLanguage_Update]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SubjectLanguage_Update]
    @Id INT,
    @SubjectId INT,
    @LanguageId INT,
    @Name NVARCHAR(200),
    @IsActive BIT = 1,
    @UpdatedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE SubjectLanguages 
        SET SubjectId = @SubjectId, LanguageId = @LanguageId, Name = @Name, 
            IsActive = @IsActive, UpdatedAt = @UpdatedAt
        WHERE Id = @Id;
        
        RETURN 0; -- Success
    END TRY
    BEGIN CATCH
        RETURN ERROR_NUMBER();
    END CATCH
END
GO
/****** Object:  StoredProcedure [dbo].[User_GetAll]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetAll]
        @Page INT = 1,
        @PageSize INT = 50
    AS
    BEGIN
        SET NOCOUNT ON;
        
        DECLARE @Offset INT = (@Page - 1) * @PageSize;
        
        SELECT 
            Id, Name, Email, PhoneNumber, CountryCode, Gender, DateOfBirth, Qualification,
            PreferredLanguage, ProfilePhoto, PreferredExam, StateId, LanguageId,
            QualificationId, ExamId, CategoryId, StreamId, RefreshToken,
            RefreshTokenExpiryTime, IsPhoneVerified, InterestedInIntlExam,
            IsActive, CreatedAt, UpdatedAt, LastLoginAt
        FROM [Users]
        ORDER BY CreatedAt DESC
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY;
    END;
GO
/****** Object:  StoredProcedure [dbo].[User_GetDailyActiveCount]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetDailyActiveCount]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT COUNT(*) 
        FROM [Users]
        WHERE LastLoginAt >= DATEADD(DAY, -1, GETDATE())
        AND IsActive = 1;
    END;
GO
/****** Object:  StoredProcedure [dbo].[User_GetTotalCount]    Script Date: 4/3/2026 12:05:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_GetTotalCount]
    AS
    BEGIN
        SET NOCOUNT ON;
        
        SELECT COUNT(*) 
        FROM [Users];
    END;
GO
USE [master]
GO
ALTER DATABASE [RankUp_MasterDB] SET  READ_WRITE 
GO
