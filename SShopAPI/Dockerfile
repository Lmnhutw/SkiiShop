# Stage 1: Build the API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy and restore dependencies
COPY SShopAPI/*.csproj ./SShopAPI/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY Core/*.csproj ./Core/
RUN dotnet restore ./SShopAPI/SShopAPI.csproj

# Copy the rest of the application code
COPY . .
WORKDIR /app/SShopAPI
RUN dotnet build -c Release --no-restore

# Publish application
RUN dotnet publish -c Release -o /out --no-build --self-contained false

# Stage 2: Build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

# Run the API
ENTRYPOINT ["dotnet", "SShopAPI.dll"]