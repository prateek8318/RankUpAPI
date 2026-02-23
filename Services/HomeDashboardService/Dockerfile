# RankUpAPI HomeDashboardService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/HomeDashboardService/HomeDashboardService.API/HomeDashboardService.API.csproj", "Services/HomeDashboardService/HomeDashboardService.API/"]
COPY ["Services/HomeDashboardService/HomeDashboardService.Application/HomeDashboardService.Application.csproj", "Services/HomeDashboardService/HomeDashboardService.Application/"]
COPY ["Services/HomeDashboardService/HomeDashboardService.Domain/HomeDashboardService.Domain.csproj", "Services/HomeDashboardService/HomeDashboardService.Domain/"]
COPY ["Services/HomeDashboardService/HomeDashboardService.Infrastructure/HomeDashboardService.Infrastructure.csproj", "Services/HomeDashboardService/HomeDashboardService.Infrastructure/"]

RUN dotnet restore "Services/HomeDashboardService/HomeDashboardService.API/HomeDashboardService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/HomeDashboardService/", "Services/HomeDashboardService/"]

WORKDIR "/src/Services/HomeDashboardService/HomeDashboardService.API"
RUN dotnet publish "HomeDashboardService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "HomeDashboardService.API.dll"]
