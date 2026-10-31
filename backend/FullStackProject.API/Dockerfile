
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project files first for better Docker layer caching
COPY FullStackProject.Domain/FullStackProject.Domain.csproj FullStackProject.Domain/
COPY FullStackProject.Application/FullStackProject.Application.csproj FullStackProject.Application/
COPY FullStackProject.Infrastructure/FullStackProject.Infrastructure.csproj FullStackProject.Infrastructure/
COPY FullStackProject.API/FullStackProject.API.csproj FullStackProject.API/

# Restore dependencies
RUN dotnet restore FullStackProject.API/FullStackProject.API.csproj

# Copy the remaining backend source
COPY . .

# Publish API
RUN dotnet publish FullStackProject.API/FullStackProject.API.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "FullStackProject.API.dll"]
