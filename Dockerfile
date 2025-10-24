# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["AssetManagement.Data/AssetManagement.Data.csproj", "AssetManagement.Data/"]
COPY ["AssetManagement.Business/AssetManagement.Business.csproj", "AssetManagement.Business/"]
COPY ["AssetManagement.UI/AssetManagement.UI.csproj", "AssetManagement.UI/"]

# Restore dependencies for all projects
RUN dotnet restore "AssetManagement.Data/AssetManagement.Data.csproj"
RUN dotnet restore "AssetManagement.Business/AssetManagement.Business.csproj"
RUN dotnet restore "AssetManagement.UI/AssetManagement.UI.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/AssetManagement.UI"
RUN dotnet build "AssetManagement.UI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "AssetManagement.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Set environment
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Run the application
ENTRYPOINT ["dotnet", "AssetManagement.UI.dll"]