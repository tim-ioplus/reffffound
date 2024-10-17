# Base stage: Use ASP.NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage: Build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["reffffound.csproj", "."]
RUN dotnet restore "./reffffound.csproj"

# Copy the remaining files and build the app
COPY . .
RUN dotnet build "./reffffound.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

# copy sqlite db
# COPY app.db /app/
# RUN chmod 777 /app/app.db

# Publish stage: Publish the service project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./reffffound.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

# Final stage: Run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the app.db file from the output directory of the build
# Make sure this path matches the actual output path
#COPY bin/Release/net8.0/app.db /app/app.db
COPY app.db /app/app.db
RUN chmod 777 /app/app.db

ENTRYPOINT ["dotnet", "reffffound.dll"]
