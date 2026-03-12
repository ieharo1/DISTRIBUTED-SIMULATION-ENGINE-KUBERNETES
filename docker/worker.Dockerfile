FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY workers ./workers
RUN dotnet restore workers/Worker.csproj
RUN dotnet publish workers/Worker.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Worker.dll"]

