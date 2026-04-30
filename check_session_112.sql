-- Check MockTestSessions for session 112
SELECT * 
FROM MockTestSessions 
WHERE Id = 112;

-- Check MockTestAttempts for attempt 23
SELECT * 
FROM MockTestAttempts 
WHERE Id = 23;

-- Check if there are any attempts for user 45 and mock test 12
SELECT TOP 5 *
FROM MockTestAttempts 
WHERE UserId = 45 AND MockTestId = 12
ORDER BY Id DESC;

-- Check if there are any sessions for user 45 and mock test 12
SELECT TOP 5 *
FROM MockTestSessions 
WHERE UserId = 45 AND MockTestId = 12
ORDER BY Id DESC;
