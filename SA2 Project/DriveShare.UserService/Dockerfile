FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy shared library and service project
COPY ["DriveShare.Shared/DriveShare.Shared.csproj", "DriveShare.Shared/"]
COPY ["DriveShare.UserService/DriveShare.UserService.csproj", "DriveShare.UserService/"]

# Restore
RUN dotnet restore "DriveShare.UserService/DriveShare.UserService.csproj"

# Copy full source
COPY . .
WORKDIR "/src/DriveShare.UserService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DriveShare.UserService.dll"]
