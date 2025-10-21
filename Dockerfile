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

# Install EF Core tools for migrations
RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

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

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
# Migrations will run automatically in Program.cs
ENTRYPOINT ["dotnet", "AssetManagement.UI.dll"]