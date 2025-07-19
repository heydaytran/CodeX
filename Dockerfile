# -------- Stage 1: Build --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY Directory.Packages.props ./src/
COPY Lsg.Core.Api.sln ./
COPY src/ ./src/

# Restore using the solution file
RUN dotnet restore Lsg.Core.Api.sln

# Publish the API project
RUN dotnet publish src/WebApi/WebApi.csproj -c Release -o /app/publish

# -------- Stage 2: Runtime --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base

WORKDIR /app

COPY --from=build /app/publish .

RUN apk add --no-cache icu-libs curl
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

HEALTHCHECK --interval=5s --timeout=10s --retries=3 CMD curl --fail http://localhost:80/hc/liveness || exit 1

EXPOSE 8080

ENTRYPOINT ["dotnet", "WebApi.dll"]
