# RankUpAPI QuizService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/QuizService/QuizService.API/QuizService.API.csproj", "Services/QuizService/QuizService.API/"]
COPY ["Services/QuizService/QuizService.Application/QuizService.Application.csproj", "Services/QuizService/QuizService.Application/"]
COPY ["Services/QuizService/QuizService.Domain/QuizService.Domain.csproj", "Services/QuizService/QuizService.Domain/"]
COPY ["Services/QuizService/QuizService.Infrastructure/QuizService.Infrastructure.csproj", "Services/QuizService/QuizService.Infrastructure/"]

RUN dotnet restore "Services/QuizService/QuizService.API/QuizService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/QuizService/", "Services/QuizService/"]

WORKDIR "/src/Services/QuizService/QuizService.API"
RUN dotnet publish "QuizService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "QuizService.API.dll"]
