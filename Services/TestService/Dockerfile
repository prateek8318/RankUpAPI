# RankUpAPI TestService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/TestService/TestService.API/TestService.API.csproj", "Services/TestService/TestService.API/"]
COPY ["Services/TestService/TestService.Application/TestService.Application.csproj", "Services/TestService/TestService.Application/"]
COPY ["Services/TestService/TestService.Domain/TestService.Domain.csproj", "Services/TestService/TestService.Domain/"]
COPY ["Services/TestService/TestService.Infrastructure/TestService.Infrastructure.csproj", "Services/TestService/TestService.Infrastructure/"]

RUN dotnet restore "Services/TestService/TestService.API/TestService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/TestService/", "Services/TestService/"]

WORKDIR "/src/Services/TestService/TestService.API"
RUN dotnet publish "TestService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "TestService.API.dll"]
