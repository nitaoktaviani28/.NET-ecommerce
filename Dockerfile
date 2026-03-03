# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY EcommerceApp.csproj ./
RUN dotnet restore EcommerceApp.csproj

COPY . ./
RUN dotnet publish EcommerceApp.csproj -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080

ENV DOTNET_EnableDiagnostics=1

ENV SERVICE_NAME=ecommerce-app
ENV SERVICE_VERSION=1.0.0

ENV LOKI_ENDPOINT=http://4.149.158.209/loki/api/v1/push

ENV ALLOY_OTLP_GRPC_ENDPOINT=http://20.115.192.181:4317
ENV ALLOY_OTLP_HTTP_ENDPOINT=http://20.115.192.181:4318

EXPOSE 8080

ENTRYPOINT ["dotnet", "EcommerceApp.dll"]
