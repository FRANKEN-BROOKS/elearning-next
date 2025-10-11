FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj", "Services/SubscriptionService/SubscriptionService.API/"]
COPY ["Services/SubscriptionService/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj", "Services/SubscriptionService/SubscriptionService.Infrastructure/"]
COPY ["Services/SubscriptionService/SubscriptionService.Core/SubscriptionService.Core.csproj", "Services/SubscriptionService/SubscriptionService.Core/"]
COPY ["Shared/Shared.Common/Shared.Common.csproj", "Shared/Shared.Common/"]
COPY ["Shared/Shared.MessageQueue/Shared.MessageQueue.csproj", "Shared/Shared.MessageQueue/"]

RUN dotnet restore "Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj"

COPY . .
WORKDIR "/src/Services/SubscriptionService/SubscriptionService.API"
RUN dotnet build "SubscriptionService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriptionService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriptionService.API.dll"]