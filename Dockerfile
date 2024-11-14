# Base image for .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0-worker-runtime-amd64 AS base

# Set working directory
WORKDIR /app

# Copy project files from context
COPY . .

# Restore dependencies
RUN dotnet restore Tcc_DbTracker_API.csproj

# Publish the application
RUN dotnet publish Tcc_DbTracker_API.csproj -c Release -o out

# Expose port for the API
EXPOSE 80

# Copy published files to container entry point
COPY out/ /app/out

# Entrypoint to run the application
ENTRYPOINT ["dotnet", "Tcc_DbTracker_API.dll"]

# Alternative Entrypoint (for debugging)
# ENTRYPOINT dotnet watch /app/Tcc_DbTracker_API.csproj run