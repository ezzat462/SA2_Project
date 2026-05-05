FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy shared library and service project
COPY ["DriveShare.Shared/DriveShare.Shared.csproj", "DriveShare.Shared/"]
COPY ["DriveShare.RentalService/DriveShare.RentalService.csproj", "DriveShare.RentalService/"]

# Restore
RUN dotnet restore "DriveShare.RentalService/DriveShare.RentalService.csproj"

# Copy full source
COPY . .
WORKDIR "/src/DriveShare.RentalService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DriveShare.RentalService.dll"]
