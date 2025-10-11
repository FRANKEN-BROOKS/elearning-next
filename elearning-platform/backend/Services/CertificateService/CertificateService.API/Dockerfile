FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5005

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/CertificateService/CertificateService.API/CertificateService.API.csproj", "Services/CertificateService/CertificateService.API/"]
COPY ["Services/CertificateService/CertificateService.Infrastructure/CertificateService.Infrastructure.csproj", "Services/CertificateService/CertificateService.Infrastructure/"]
COPY ["Services/CertificateService/CertificateService.Core/CertificateService.Core.csproj", "Services/CertificateService/CertificateService.Core/"]
COPY ["Shared/Shared.Common/Shared.Common.csproj", "Shared/Shared.Common/"]
COPY ["Shared/Shared.MessageQueue/Shared.MessageQueue.csproj", "Shared/Shared.MessageQueue/"]

RUN dotnet restore "Services/CertificateService/CertificateService.API/CertificateService.API.csproj"

COPY . .
WORKDIR "/src/Services/CertificateService/CertificateService.API"
RUN dotnet build "CertificateService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CertificateService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CertificateService.API.dll"]