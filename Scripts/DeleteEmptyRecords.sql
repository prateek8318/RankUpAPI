

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Starting cleanup of empty records...';
    

    
    -- First delete related exam qualifications for exams with empty names
    DELETE EQ
    FROM ExamQualifications EQ
    INNER JOIN Exams E ON EQ.ExamId = E.Id
    WHERE (E.Name IS NULL OR E.Name = '' OR E.Name = ' ');
    
    DECLARE @ExamCount INT = @@ROWCOUNT;
    PRINT CAST(@ExamCount AS VARCHAR) + ' exam qualification records deleted for exams with empty names';
    
    -- Then delete exams with empty names
    DELETE FROM Exams
    WHERE Name IS NULL OR Name = '' OR E.Name = ' ';
    
    SET @ExamCount = @@ROWCOUNT;
    PRINT CAST(@ExamCount AS VARCHAR) + ' exams with empty names deleted';
    
    -- =====================================================
    -- Clean Qualifications with empty names
    -- =====================================================
    
    -- Delete qualification languages for qualifications with empty names
    DELETE QL
    FROM QualificationLanguages QL
    INNER JOIN Qualifications Q ON QL.QualificationId = Q.Id
    WHERE (Q.Name IS NULL OR Q.Name = '' OR Q.Name = ' ');
    
    DECLARE @QualLangCount INT = @@ROWCOUNT;
    PRINT CAST(@QualLangCount AS VARCHAR) + ' qualification language records deleted for qualifications with empty names';
    
    -- Delete qualifications with empty names
    DELETE FROM Qualifications
    WHERE Name IS NULL OR Name = '' OR Name = ' ';
    
    DECLARE @QualCount INT = @@ROWCOUNT;
    PRINT CAST(@QualCount AS VARCHAR) + ' qualifications with empty names deleted';
    
    -- =====================================================
    -- Summary Report
    -- =====================================================
    
    SELECT 
        'Cleanup Summary' AS Operation,
        GETDATE() AS CompletedAt,
        'Success' AS Status;
    
    COMMIT TRANSACTION;
    
    PRINT '✅ Database cleanup completed successfully!';
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT '❌ Error during cleanup: ' + ERROR_MESSAGE();
    
    -- Return error details
    SELECT 
        'Cleanup Error' AS Operation,
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_MESSAGE() AS ErrorMessage,
        GETDATE() AS ErrorTime;
END CATCH
