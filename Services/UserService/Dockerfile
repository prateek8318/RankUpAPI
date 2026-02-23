# RankUpAPI UserService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/UserService/UserService.API/UserService.API.csproj", "Services/UserService/UserService.API/"]
COPY ["Services/UserService/UserService.Application/UserService.Application.csproj", "Services/UserService/UserService.Application/"]
COPY ["Services/UserService/UserService.Domain/UserService.Domain.csproj", "Services/UserService/UserService.Domain/"]
COPY ["Services/UserService/UserService.Infrastructure/UserService.Infrastructure.csproj", "Services/UserService/UserService.Infrastructure/"]

RUN dotnet restore "Services/UserService/UserService.API/UserService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/UserService/", "Services/UserService/"]

WORKDIR "/src/Services/UserService/UserService.API"
RUN dotnet publish "UserService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "UserService.API.dll"]
