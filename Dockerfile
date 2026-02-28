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

# Copy app
COPY --from=build /app/publish ./

# =========================
# PYROSCOPE .NET NATIVE PROFILER (WAJIB)
# =========================
COPY --from=pyroscope/pyroscope-dotnet:0.13.0-glibc \
  /Pyroscope.Profiler.Native.so /dotnet/Pyroscope.Profiler.Native.so

COPY --from=pyroscope/pyroscope-dotnet:0.13.0-glibc \
  /Pyroscope.Linux.ApiWrapper.x64.so /dotnet/Pyroscope.Linux.ApiWrapper.x64.so

# =========================
# ENV
# =========================
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "EcommerceApp.dll"]

