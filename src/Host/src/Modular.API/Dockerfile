# syntax=docker/dockerfile:1

# ==============================================================================
# USAGE INSTRUCTIONS
# ==============================================================================
# Important: Execute the build from the repository root directory (Build Context).
#
# Local Build Command:
#   docker build -f src/Host/src/Modular.API/Dockerfile -t modular-api:local .
#
# CI/CD Pipeline Build Command (Example):
#   docker build -f src/Host/src/Modular.API/Dockerfile \
#     --build-arg VERSION_SUFFIX="main.57" \
#     --build-arg GIT_SHA="a1b2c3d4" \
#     -t modular-api:1.0.0-main.57 .
# ==============================================================================

# ==============================================================================
# STAGE 1: Runtime Base (Lightweight & Secure)
# ==============================================================================
# Uses Alpine Linux for minimal attack surface and small footprint (~100MB).
# Named "base" to support Visual Studio Fast Mode debugging (F5 experience).
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base

# Run as non-root user for security best practices (Least Privilege Principle)
USER $APP_UID
WORKDIR /app

# HTTP only: TLS terminates at the ingress / load balancer.
EXPOSE 8080

# ==============================================================================
# STAGE 2: Build & Publish (SDK Environment)
# ==============================================================================
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

# Build configuration (Default: Release)
ARG BUILD_CONFIGURATION=Release

# Package Versioning Strategy:
#   - VERSION_SUFFIX: Appended to VersionPrefix (e.g., "main.57" -> 1.0.0-main.57). Set empty for official releases.
#   - GIT_SHA: Injected by CI runner for Assembly InformationalVersion tracking (e.g., 1.0.0-main.57+a1b2c3d).
ARG VERSION_SUFFIX=local
ARG GIT_SHA=

# Copy full repository source tree into the build container.
# Note: Sensitive files, bin/obj folders, and .git directory are excluded via .dockerignore.
WORKDIR /src

COPY . .

# Buildkit Persistent Cache Mount Architecture:
# - Target '/root/.nuget/packages' preserves downloaded packages across builds on the host.
# - Eliminates the need for explicit per-csproj COPY & 'dotnet restore' steps.
# - Zero Dockerfile modifications required when adding/modifying internal modular projects.
# - '-p:RunAnalyzers=false' disables Roslyn analyzers to boost build speed (analyzers are enforced in upstream CI linting jobs).
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "src/Host/src/Modular.API/Modular.API.csproj" \
        --configuration "$BUILD_CONFIGURATION" \
        --output /app/publish \
        -p:UseAppHost=false \
        -p:RunAnalyzers=false \
        -p:VersionSuffix="$VERSION_SUFFIX" \
        -p:SourceRevisionId="$GIT_SHA"

# ==============================================================================
# STAGE 3: Final Production Image
# ==============================================================================
FROM base AS final

# Copy compiled production artifacts from the build stage
COPY --from=build /app/publish .

# Application entry point
ENTRYPOINT ["dotnet", "Modular.API.dll"]
