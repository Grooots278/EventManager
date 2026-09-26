FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY backend/EventManager.slnx ./

COPY backend/src/EventManager.Domain/EventManager.Domain.csproj src/EventManager.Domain/
COPY backend/src/EventManager.Infrastructure/EventManager.Infrastructure.csproj src/EventManager.Infrastructure/
COPY backend/src/EventManager.Application/EventManager.Application.csproj src/EventManager.Application/
COPY backend/src/EventManager.API/EventManager.API.csproj src/EventManager.API/

RUN dotnet restore src/EventManager.API/EventManager.API.csproj

COPY backend/. .

WORKDIR /src/src/EventManager.API

RUN dotnet publish EventManager.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EventManager.API.dll"]