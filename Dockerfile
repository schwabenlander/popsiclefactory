# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first for better caching
COPY *.sln .
COPY PopsicleFactory.Api/*.csproj ./PopsicleFactory.Api/
COPY PopsicleFactory.Tests/*.csproj ./PopsicleFactory.Tests/

# Restore dependencies
RUN dotnet restore

# Copy source code
COPY PopsicleFactory.Api/ ./PopsicleFactory.Api/
COPY PopsicleFactory.Tests/ ./PopsicleFactory.Tests/

# Build the application
RUN dotnet build -c Release --no-restore

# Publish the application
RUN dotnet publish PopsicleFactory.Api/PopsicleFactory.Api.csproj -c Release -o /app/publish --no-build

# Use the official .NET 8 runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose HTTP port only
EXPOSE 8080

# Set environment variables to listen on HTTP only
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "PopsicleFactory.Api.dll"]