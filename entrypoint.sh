#!/bin/bash
until /root/.dotnet/tools/dotnet-ef database update --project /src/VehiclesControl.Data/VehiclesControl.Data.csproj --startup-project /src/VehiclesControl.API/VehiclesControl.API.csproj; do
    >&2 echo "SQL Server is starting up"
    sleep 1
done

>&2 echo "SQL Server is up - executing command"
exec dotnet VehiclesControl.API.dll