# RankUpAPI QuestionService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/QuestionService/QuestionService.API/QuestionService.API.csproj", "Services/QuestionService/QuestionService.API/"]
COPY ["Services/QuestionService/QuestionService.Application/QuestionService.Application.csproj", "Services/QuestionService/QuestionService.Application/"]
COPY ["Services/QuestionService/QuestionService.Domain/QuestionService.Domain.csproj", "Services/QuestionService/QuestionService.Domain/"]
COPY ["Services/QuestionService/QuestionService.Infrastructure/QuestionService.Infrastructure.csproj", "Services/QuestionService/QuestionService.Infrastructure/"]

RUN dotnet restore "Services/QuestionService/QuestionService.API/QuestionService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/QuestionService/", "Services/QuestionService/"]

WORKDIR "/src/Services/QuestionService/QuestionService.API"
RUN dotnet publish "QuestionService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "QuestionService.API.dll"]
