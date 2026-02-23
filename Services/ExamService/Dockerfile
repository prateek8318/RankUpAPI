# RankUpAPI ExamService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/ExamService/ExamService.API/ExamService.API.csproj", "Services/ExamService/ExamService.API/"]
COPY ["Services/ExamService/ExamService.Application/ExamService.Application.csproj", "Services/ExamService/ExamService.Application/"]
COPY ["Services/ExamService/ExamService.Domain/ExamService.Domain.csproj", "Services/ExamService/ExamService.Domain/"]
COPY ["Services/ExamService/ExamService.Infrastructure/ExamService.Infrastructure.csproj", "Services/ExamService/ExamService.Infrastructure/"]

RUN dotnet restore "Services/ExamService/ExamService.API/ExamService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/ExamService/", "Services/ExamService/"]

WORKDIR "/src/Services/ExamService/ExamService.API"
RUN dotnet publish "ExamService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "ExamService.API.dll"]
