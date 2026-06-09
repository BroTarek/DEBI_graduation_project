# Use the official .NET 8.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the solution and project files first to restore dependencies
COPY BackendV2.sln ./
COPY Core/Core.csproj ./Core/
COPY Infrastructure/Infrastructure.csproj ./Infrastructure/
COPY Web/Web/Web.csproj ./Web/Web/

# Restore NuGet packages
RUN dotnet restore

# Copy the rest of the source code and build
COPY . ./
RUN dotnet publish Web/Web/Web.csproj -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Define the entrypoint for the container
ENTRYPOINT ["dotnet", "Web.dll"]
