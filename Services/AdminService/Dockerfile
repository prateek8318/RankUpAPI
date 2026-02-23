# RankUpAPI AdminService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/AdminService/AdminService.API/AdminService.API.csproj", "Services/AdminService/AdminService.API/"]
COPY ["Services/AdminService/AdminService.Application/AdminService.Application.csproj", "Services/AdminService/AdminService.Application/"]
COPY ["Services/AdminService/AdminService.Domain/AdminService.Domain.csproj", "Services/AdminService/AdminService.Domain/"]
COPY ["Services/AdminService/AdminService.Infrastructure/AdminService.Infrastructure.csproj", "Services/AdminService/AdminService.Infrastructure/"]
COPY ["Services/UserService/UserService.Application/UserService.Application.csproj", "Services/UserService/UserService.Application/"]
COPY ["Services/UserService/UserService.Domain/UserService.Domain.csproj", "Services/UserService/UserService.Domain/"]
COPY ["Services/ExamService/ExamService.Application/ExamService.Application.csproj", "Services/ExamService/ExamService.Application/"]
COPY ["Services/ExamService/ExamService.Domain/ExamService.Domain.csproj", "Services/ExamService/ExamService.Domain/"]
COPY ["Common/Common.csproj", "Common/"]

RUN dotnet restore "Services/AdminService/AdminService.API/AdminService.API.csproj"

COPY ["Services/AdminService/", "Services/AdminService/"]
COPY ["Services/UserService/UserService.Application/", "Services/UserService/UserService.Application/"]
COPY ["Services/UserService/UserService.Domain/", "Services/UserService/UserService.Domain/"]
COPY ["Services/ExamService/ExamService.Application/", "Services/ExamService/ExamService.Application/"]
COPY ["Services/ExamService/ExamService.Domain/", "Services/ExamService/ExamService.Domain/"]
COPY ["Common/", "Common/"]

WORKDIR "/src/Services/AdminService/AdminService.API"
RUN dotnet publish "AdminService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "AdminService.API.dll"]
