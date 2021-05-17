#!/bin/bash
echo Enter NuGet key:
read key

dotnet clean -c Release
dotnet build -c Release
dotnet nuget push ./TcpSubLib/bin/Release/*.nupkg -k $key -s https://api.nuget.org/v3/index.json
