# syntax=docker/dockerfile:1
# Build context is the repository root:
#   docker build -f src/Host/src/Modular.API/Dockerfile -t modular-api:local .

# Runtime base. Also used by Visual Studio "fast mode" debugging, which needs a stage named "base".
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
# HTTP only: TLS terminates at the ingress / load balancer.
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
# Appended to VersionPrefix: "main.57" -> 1.0.0-main.57. Pass an empty value for a release build.
ARG VERSION_SUFFIX=local
# .git is excluded from the build context, so CI passes the commit for InformationalVersion (1.0.0-main.57+<sha>).
ARG GIT_SHA=
WORKDIR /src
COPY . .
# The cache mount keeps NuGet packages between builds, so no per-csproj COPY list is needed:
# adding a project never requires editing this file.
# Analyzers are skipped here; the CI build job already enforces them.
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish "src/Host/src/Modular.API/Modular.API.csproj" \
        --configuration "$BUILD_CONFIGURATION" \
        --output /app/publish \
        -p:UseAppHost=false \
        -p:RunAnalyzers=false \
        -p:VersionSuffix="$VERSION_SUFFIX" \
        -p:SourceRevisionId="$GIT_SHA"

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Modular.API.dll"]
