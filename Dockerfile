# =========================
# BUILD STAGE
# =========================
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Salin file project dan restore dependensi
COPY EcommerceApp.csproj ./
RUN dotnet restore EcommerceApp.csproj  

# Salin seluruh file dan build
COPY . ./
RUN dotnet publish EcommerceApp.csproj -c Release -o /app/publish

# =========================
# RUNTIME STAGE
# =========================
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Salin hasil build dari build stage
COPY --from=build /app/publish ./

# Salin file Pyroscope Profiler
COPY --from=pyroscope/pyroscope-dotnet:0.13.0-glibc /Pyroscope.Profiler.Native.so /dotnet/Pyroscope.Profiler.Native.so
COPY --from=pyroscope/pyroscope-dotnet:0.13.0-glibc /Pyroscope.Linux.ApiWrapper.x64.so /dotnet/Pyroscope.Linux.ApiWrapper.x64.so

# =========================
# ENV — OBSERVABILITY
# =========================
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_EnableDiagnostics=1
ENV DOTNET_EnableDiagnostics_Profiler=1

# 🔥 Pyroscope profiling
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={BD1A650D-AC5D-4896-B64F-D6FA25D6B26A}
ENV CORECLR_PROFILER_PATH=/dotnet/Pyroscope.Profiler.Native.so
ENV LD_PRELOAD=/dotnet/Pyroscope.Linux.ApiWrapper.x64.so
ENV PYROSCOPE_APPLICATION_NAME=ecommerce-app
ENV PYROSCOPE_SERVER_ADDRESS=http://172.193.209.242:4040  
ENV PYROSCOPE_ENVIRONMENT=vm
ENV PYROSCOPE_PROFILING_RATE=100

EXPOSE 8080

# Menjalankan aplikasi .NET
ENTRYPOINT ["dotnet", "EcommerceApp.dll"]
