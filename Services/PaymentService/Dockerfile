# RankUpAPI PaymentService - Dockerfile
# Build context: solution root
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Common/Common.csproj", "Common/"]
COPY ["Services/PaymentService/PaymentService.API/PaymentService.API.csproj", "Services/PaymentService/PaymentService.API/"]
COPY ["Services/PaymentService/PaymentService.Application/PaymentService.Application.csproj", "Services/PaymentService/PaymentService.Application/"]
COPY ["Services/PaymentService/PaymentService.Domain/PaymentService.Domain.csproj", "Services/PaymentService/PaymentService.Domain/"]
COPY ["Services/PaymentService/PaymentService.Infrastructure/PaymentService.Infrastructure.csproj", "Services/PaymentService/PaymentService.Infrastructure/"]

RUN dotnet restore "Services/PaymentService/PaymentService.API/PaymentService.API.csproj"

COPY ["Common/", "Common/"]
COPY ["Services/PaymentService/", "Services/PaymentService/"]

WORKDIR "/src/Services/PaymentService/PaymentService.API"
RUN dotnet publish "PaymentService.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
ENTRYPOINT ["dotnet", "PaymentService.API.dll"]
