
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
RUN dotnet build "VehiclesControl.API.csproj" -c Release -o /app/build


FROM build AS publish
RUN dotnet publish "VehiclesControl.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src /src
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh


RUN apt-get update && apt-get install -y curl
RUN curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin -Channel 8.0 -InstallDir /usr/share/dotnet
ENV PATH="${PATH}:/usr/share/dotnet"
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT ["/app/entrypoint.sh"]