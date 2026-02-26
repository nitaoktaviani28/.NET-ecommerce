# Dockerfile untuk ASP.NET Core E-Commerce
# Multi-stage build, AKS ready

# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj secara eksplisit (PENTING)
COPY EcommerceApp.csproj ./

# Restore dependencies
RUN dotnet restore EcommerceApp.csproj

# Copy seluruh source code
COPY . ./

# Build & publish aplikasi
RUN dotnet publish EcommerceApp.csproj -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy hasil publish dari build stage
COPY --from=build /app/publish ./

# Environment variables (override di Kubernetes)
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV OTEL_SERVICE_NAME=dotnet-ecommerce
ENV OTEL_EXPORTER_OTLP_ENDPOINT=http://alloy.monitoring.svc.cluster.local:4318
ENV PYROSCOPE_ENDPOINT=http://pyroscope-distributor.monitoring.svc.cluster.local:4040
ENV DATABASE_DSN=Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres

# Expose port aplikasi
EXPOSE 8080

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "EcommerceApp.dll"]
