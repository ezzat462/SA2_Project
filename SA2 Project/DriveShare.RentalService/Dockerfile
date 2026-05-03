FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["DriveShare.RentalService/DriveShare.RentalService.csproj", "DriveShare.RentalService/"]
COPY ["DriveShare.Shared/DriveShare.Shared.csproj", "DriveShare.Shared/"]

RUN dotnet restore "DriveShare.RentalService/DriveShare.RentalService.csproj"

COPY . .

WORKDIR "/src/DriveShare.RentalService"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DriveShare.RentalService.dll"]