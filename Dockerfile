# Build env
FROM mcr.microsoft.com/dotnet/sdk:5.0.201 AS builder
WORKDIR /app

# Install deps
COPY *.sln .
COPY ./API/*.csproj ./API/
COPY ./Application/*.csproj ./Application/
COPY ./Domain/*.csproj ./Domain/
COPY ./IntegrationTests/*.csproj ./IntegrationTests/
COPY ./Persistence/*.csproj ./Persistence/
COPY ./PersistenceInterface/*.csproj ./PersistenceInterface/
COPY ./UnitTests/*.csproj ./UnitTests/
RUN dotnet restore -r linux-musl-x64

# Copy code
COPY . .

# Fix the issue on Debian 10: https://github.com/dotnet/dotnet-docker/issues/2470
ENV COMPlus_EnableDiagnostics=0
RUN dotnet publish ./API -p:PublishSingleFile=true -r linux-musl-x64 --self-contained true -p:PublishTrimmed=True -p:TrimMode=Link -c release -o /app/bin --no-restore

# Run
FROM mcr.microsoft.com/dotnet/runtime-deps:5.0.4-alpine3.12-amd64
WORKDIR /app

# Copy binaries
COPY --from=builder /app/bin .

# Setup port
ENV ASPNETCORE_URLS http://*:80
EXPOSE 80

ENTRYPOINT ["/app/API"]