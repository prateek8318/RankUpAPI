@echo off
echo Fixing HTTPS certificate issues for RankUpAPI services...
echo.

echo Trusting HTTPS development certificate...
dotnet dev-certs https --trust

echo.
echo Cleaning and rebuilding solution...
dotnet clean RankUpAPI.sln
dotnet build RankUpAPI.sln

echo.
echo HTTPS certificate fixed! You can now:
echo 1. Press F5 in Visual Studio to run services with HTTPS
echo 2. Or run start-all-services.bat for HTTP URLs
echo.
pause
