FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AssetManagement.UI/AssetManagement.UI.csproj", "AssetManagement.UI/"]
COPY ["AssetManagement.Business/AssetManagement.Business.csproj", "AssetManagement.Business/"]
COPY ["AssetManagement.Data/AssetManagement.Data.csproj", "AssetManagement.Data/"]

# Restore dependencies
RUN dotnet restore "AssetManagement.UI/AssetManagement.UI.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/AssetManagement.UI"
RUN dotnet build "AssetManagement.UI.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "AssetManagement.UI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# The application will use PORT environment variable from Program.cs
# No need to set ASPNETCORE_URLS here as it's handled in code

ENTRYPOINT ["dotnet", "AssetManagement.UI.dll"]