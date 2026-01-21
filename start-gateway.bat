@echo off
echo Starting Gateway API...
cd Services\GatewayAPI
start "Gateway API" cmd /k "dotnet run"
echo Gateway API started on http://localhost:8000
echo Gateway API started on http://192.168.1.14:8000
echo.
echo Gateway Routes:
echo - Exam Service: http://localhost:8000/api/exam/*
echo - Exam Service: http://localhost:8000/api/exams/*
echo - User Service: http://localhost:8000/api/users/*
echo - Admin Service: http://localhost:8000/api/admin/*
echo - Master Service: http://localhost:8000/api/master/*
echo - Languages: http://localhost:8000/api/languages/*
echo - Home Dashboard: http://localhost:8000/api/homedashboard/*
echo.
echo IP Address Access:
echo - Gateway: http://192.168.1.14:8000
echo - All routes work with IP address as well
echo.
pause
