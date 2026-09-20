FROM mcr.microsoft.com/dotnet/sdk:10.0 AS migrator

WORKDIR /src

COPY backend/EventManager.slnx ./

COPY backend/src/EventManager.Domain/EventManager.Domain.csproj \
    src/EventManager.Domain/

COPY backend/src/EventManager.Application/EventManager.Application.csproj \
    src/EventManager.Application/

COPY backend/src/EventManager.Infrastructure/EventManager.Infrastructure.csproj \
    src/EventManager.Infrastructure/

COPY backend/src/EventManager.API/EventManager.API.csproj \
    src/EventManager.API/

RUN dotnet restore \
    src/EventManager.API/EventManager.API.csproj

COPY backend/. .

WORKDIR /src

RUN dotnet tool install \
    --global dotnet-ef \
    --version 10.0.12

ENV PATH="${PATH}:/root/.dotnet/tools"

ENTRYPOINT [ \
    "dotnet", \
    "ef", \
    "database", \
    "update", \
    "--project", \
    "src/EventManager.Infrastructure", \
    "--startup-project", \
    "src/EventManager.API" \
]