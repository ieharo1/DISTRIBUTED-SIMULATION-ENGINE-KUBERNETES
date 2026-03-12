FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY api ./api
RUN dotnet restore api/SimulationApi.csproj
RUN dotnet publish api/SimulationApi.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://0.0.0.0:8000
ENTRYPOINT ["dotnet", "SimulationApi.dll"]

