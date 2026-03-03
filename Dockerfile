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

COPY --from=pyroscope/pyroscope-dotnet:0.13.0-glibc \
  /Pyroscope.Profiler.Native.so /dotnet/Pyroscope.Profiler.Native.so

COPY --from=pyroscope/pyroscope-dotnet:0.13.0-glibc \
  /Pyroscope.Linux.ApiWrapper.x64.so /dotnet/Pyroscope.Linux.ApiWrapper.x64.so

ENV ASPNETCORE_URLS=http://+:8080

ENV DOTNET_EnableDiagnostics=1
ENV DOTNET_EnableDiagnostics_Profiler=1

ENV SERVICE_NAME=ecommerce-app
ENV SERVICE_VERSION=1.0.0

ENV LOKI_ENDPOINT=http://4.149.158.209/loki/api/v1/push

ENV ALLOY_OTLP_GRPC_ENDPOINT=http://20.115.192.181:4317
ENV ALLOY_OTLP_HTTP_ENDPOINT=http://20.115.192.181:4318

ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
ENV CORECLR_PROFILER_PATH=/dotnet/Pyroscope.Profiler.Native.so

ENV PYROSCOPE_APPLICATION_NAME=ecommerce-app
ENV PYROSCOPE_SERVER_ADDRESS=http://172.193.209.242:4040
ENV PYROSCOPE_ENVIRONMENT=vm
ENV PYROSCOPE_PROFILING_RATE=100

EXPOSE 8080

ENTRYPOINT ["dotnet", "EcommerceApp.dll"]
