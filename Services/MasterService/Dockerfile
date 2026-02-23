# RankUpAPI MasterService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Common/Common.csproj", "Services/Common/"]
COPY ["Services/MasterService/MasterService.API/MasterService.API.csproj", "Services/MasterService/MasterService.API/"]
COPY ["Services/MasterService/MasterService.Application/MasterService.Application.csproj", "Services/MasterService/MasterService.Application/"]
COPY ["Services/MasterService/MasterService.Domain/MasterService.Domain.csproj", "Services/MasterService/MasterService.Domain/"]
COPY ["Services/MasterService/MasterService.Infrastructure/MasterService.Infrastructure.csproj", "Services/MasterService/MasterService.Infrastructure/"]

RUN dotnet restore "Services/MasterService/MasterService.API/MasterService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Common/", "Services/Common/"]
COPY ["Services/MasterService/", "Services/MasterService/"]

WORKDIR "/src/Services/MasterService/MasterService.API"
RUN dotnet publish "MasterService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "MasterService.API.dll"]
