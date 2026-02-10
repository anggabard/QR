# Build stage: Linux .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project into container (build from repo context)
COPY . .

# Restore, build, then publish (each step once; no duplicate work)
RUN dotnet restore QR-Generator.csproj
RUN dotnet build QR-Generator.csproj -c Release --no-restore
RUN dotnet publish QR-Generator.csproj -c Release -o /app/publish --no-restore --no-build

# Runtime stage: Azure Functions isolated worker (Linux)
FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0 AS base
WORKDIR /home/site/wwwroot

# Install Azure Functions Core Tools (not in PATH in this image)
RUN apt-get update && apt-get install -y curl \
    && curl -sL https://deb.nodesource.com/setup_20.x | bash - \
    && apt-get install -y nodejs \
    && npm install -g azure-functions-core-tools@4 --unsafe-perm \
    && apt-get clean && rm -rf /var/lib/apt/lists/*

# Copy published app
COPY --from=build /app/publish .

# Copy index.html so the root function can serve it
COPY --from=build /src/index.html .

ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true

EXPOSE 80

ENTRYPOINT ["func", "start", "--dotnet-isolated", "--port", "80"]
