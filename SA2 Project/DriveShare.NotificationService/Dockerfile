FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DriveShare.NotificationService/DriveShare.NotificationService.csproj", "DriveShare.NotificationService/"]
COPY ["DriveShare.Shared/DriveShare.Shared.csproj", "DriveShare.Shared/"]

RUN dotnet restore "DriveShare.NotificationService/DriveShare.NotificationService.csproj"

COPY ["DriveShare.NotificationService/", "DriveShare.NotificationService/"]
COPY ["DriveShare.Shared/", "DriveShare.Shared/"]

WORKDIR "/src/DriveShare.NotificationService"
RUN dotnet build "DriveShare.NotificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DriveShare.NotificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DriveShare.NotificationService.dll"]