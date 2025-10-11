FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Services/CourseService/CourseService.API/CourseService.API.csproj", "Services/CourseService/CourseService.API/"]
COPY ["Services/CourseService/CourseService.Infrastructure/CourseService.Infrastructure.csproj", "Services/CourseService/CourseService.Infrastructure/"]
COPY ["Services/CourseService/CourseService.Core/CourseService.Core.csproj", "Services/CourseService/CourseService.Core/"]
COPY ["Shared/Shared.Common/Shared.Common.csproj", "Shared/Shared.Common/"]
COPY ["Shared/Shared.MessageQueue/Shared.MessageQueue.csproj", "Shared/Shared.MessageQueue/"]

RUN dotnet restore "Services/CourseService/CourseService.API/CourseService.API.csproj"

COPY . .
WORKDIR "/src/Services/CourseService/CourseService.API"
RUN dotnet build "CourseService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CourseService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CourseService.API.dll"]