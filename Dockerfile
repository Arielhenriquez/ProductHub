# ──────────────────────────────────────────
# Stage 1: Build
# ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files first (enables layer caching for restore)
COPY ProductHub.Domain/ProductHub.Domain.csproj             ProductHub.Domain/
COPY ProductHub.Application/ProductHub.Application.csproj   ProductHub.Application/
COPY ProductHub.Infrastructure/ProductHub.Infrastructure.csproj ProductHub.Infrastructure/
COPY ProductHub/ProductHub.csproj                           ProductHub/

# Restore dependencies
RUN dotnet restore ProductHub/ProductHub.csproj

# Copy the rest of the source code
COPY ProductHub.Domain/        ProductHub.Domain/
COPY ProductHub.Application/   ProductHub.Application/
COPY ProductHub.Infrastructure/ ProductHub.Infrastructure/
COPY ProductHub/               ProductHub/

# Publish in Release mode
RUN dotnet publish ProductHub/ProductHub.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ──────────────────────────────────────────
# Stage 2: Runtime image
# ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProductHub.dll"]
