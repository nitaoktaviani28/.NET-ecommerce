# Dockerfile untuk ASP.NET Core E-Commerce
# Equivalent to: Dockerfile (Golang version)

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy source code dan build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Environment variables (akan di-override oleh Kubernetes)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV OTEL_SERVICE_NAME=dotnet-ecommerce
ENV OTEL_EXPORTER_OTLP_ENDPOINT=http://alloy.monitoring.svc.cluster.local:4318
ENV PYROSCOPE_ENDPOINT=http://pyroscope-distributor.monitoring.svc.cluster.local:4040
ENV DATABASE_DSN=Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

# Run application
ENTRYPOINT ["dotnet", "EcommerceApp.dll"]
