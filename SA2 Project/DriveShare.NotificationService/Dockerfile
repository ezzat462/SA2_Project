FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy shared library and service project
COPY ["DriveShare.Shared/DriveShare.Shared.csproj", "DriveShare.Shared/"]
COPY ["DriveShare.NotificationService/DriveShare.NotificationService.csproj", "DriveShare.NotificationService/"]

# Restore
RUN dotnet restore "DriveShare.NotificationService/DriveShare.NotificationService.csproj"

# Copy full source
COPY . .
WORKDIR "/src/DriveShare.NotificationService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DriveShare.NotificationService.dll"]
