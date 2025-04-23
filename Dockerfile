FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .

# Publish API and Workflows
RUN dotnet publish BookMate.API/BookMate.API.csproj -c Release -o /app/BookMate.API
RUN dotnet publish BookMate.Workflows/BookMate.Workflows.csproj -c Release -o /app/BookMate.Workflows

FROM base AS final
WORKDIR /app

# Copy published apps
COPY --from=build /app/BookMate.API ./BookMate.API
COPY --from=build /app/BookMate.Workflows ./BookMate.Workflows

# Copy the startup script and make executable
COPY start.sh /app/start.sh
RUN chmod +x /app/start.sh

# Working directory for BookMate.API
WORKDIR /app/BookMate.API

# Entrypoint is the script
ENTRYPOINT ["/app/start.sh"]