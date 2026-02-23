# RankUpAPI Gateway (Ocelot) - Dockerfile
# Build context: solution root (e.g. docker build -f Services/GatewayAPI/Dockerfile .)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and projects (minimal for restore)
COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/GatewayAPI/GatewayAPI.csproj", "Services/GatewayAPI/"]

RUN dotnet restore "Services/GatewayAPI/GatewayAPI.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/GatewayAPI/", "Services/GatewayAPI/"]

WORKDIR "/src/Services/GatewayAPI"
RUN dotnet publish "GatewayAPI.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Install curl for optional healthchecks
RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80
ENTRYPOINT ["dotnet", "GatewayAPI.dll"]
