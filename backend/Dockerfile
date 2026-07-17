FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files relative to context (backend/)
COPY AgriLedger.sln ./
COPY src/AgriLedger.API/AgriLedger.API.csproj src/AgriLedger.API/
COPY src/AgriLedger.Application/AgriLedger.Application.csproj src/AgriLedger.Application/
COPY src/AgriLedger.Domain/AgriLedger.Domain.csproj src/AgriLedger.Domain/
COPY src/AgriLedger.Infrastructure/AgriLedger.Infrastructure.csproj src/AgriLedger.Infrastructure/

RUN dotnet restore AgriLedger.sln

# Copy all source files
COPY src/ src/

# Build and publish
RUN dotnet publish src/AgriLedger.API/AgriLedger.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
EXPOSE 8081

ENTRYPOINT ["dotnet", "AgriLedger.API.dll"]
