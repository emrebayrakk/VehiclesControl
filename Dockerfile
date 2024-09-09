FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VehiclesControl.API/VehiclesControl.API.csproj", "VehiclesControl.API/"]
COPY ["VehiclesControl.Application/VehiclesControl.Application.csproj", "VehiclesControl.Application/"]
COPY ["VehiclesControl.Data/VehiclesControl.Data.csproj", "VehiclesControl.Data/"]
COPY ["VehiclesControl.Domain/VehiclesControl.Domain.csproj", "VehiclesControl.Domain/"]
COPY ["VehiclesControl.Infrastructure/VehiclesControl.Infrastructure.csproj", "VehiclesControl.Infrastructure/"]
COPY ["VehiclesControl.Tests/VehiclesControl.Tests.csproj", "VehiclesControl.Tests/"]
RUN dotnet restore "VehiclesControl.API/VehiclesControl.API.csproj"
COPY . .
WORKDIR "/src/VehiclesControl.API"
RUN dotnet build "VehiclesControl.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "VehiclesControl.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final aşamada .NET SDK'nın da olduğu bir katman kullanıyoruz
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VehiclesControl.API.dll"]
