# RankUpAPI SubscriptionService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj", "Services/SubscriptionService/SubscriptionService.API/"]
COPY ["Services/SubscriptionService/SubscriptionService.Application/SubscriptionService.Application.csproj", "Services/SubscriptionService/SubscriptionService.Application/"]
COPY ["Services/SubscriptionService/SubscriptionService.Domain/SubscriptionService.Domain.csproj", "Services/SubscriptionService/SubscriptionService.Domain/"]
COPY ["Services/SubscriptionService/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj", "Services/SubscriptionService/SubscriptionService.Infrastructure/"]

RUN dotnet restore "Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/SubscriptionService/", "Services/SubscriptionService/"]

WORKDIR "/src/Services/SubscriptionService/SubscriptionService.API"
RUN dotnet publish "SubscriptionService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "SubscriptionService.API.dll"]
