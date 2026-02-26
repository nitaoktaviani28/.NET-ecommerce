# 🔵 ASP.NET Core E-Commerce - 1:1 Equivalent to Golang

Aplikasi e-commerce ASP.NET Core (.NET 8) yang secara **fungsional dan observability setara 1:1** dengan aplikasi Golang.

## 📁 Structural Equivalence

### Golang → ASP.NET Core Mapping

| Golang | ASP.NET Core | Purpose |
|--------|--------------|---------|
| `main.go` | `Program.cs` | Entry point |
| `handlers/product.go` | `Controllers/HomeController.cs` | Product handlers |
| `handlers/order.go` | `Controllers/CheckoutController.cs` | Order handlers |
| `repository/postgres.go` | `Repository/ProductRepository.cs`<br>`Repository/OrderRepository.cs` | Database layer |
| `observability/init.go` | `Observability/ObservabilityInit.cs` | Observability entry point |
| `observability/tracing.go` | `Observability/Tracing.cs` | OpenTelemetry setup |
| `observability/profiling.go` | `Observability/Profiling.cs` | Pyroscope setup |
| `observability/env.go` | `Observability/Env.cs` | Environment helpers |
| `templates/index.html` | `Views/Index.cshtml` | Product list template |
| `templates/success.html` | `Views/Success.cshtml` | Success template |
| `go.mod` | `EcommerceApp.csproj` | Dependencies |
| `Dockerfile` | `Dockerfile` | Container build |

## 🎯 Observability Parity (1:1)

### Single Entry Point (SAMA)

**Golang:**
```go
func main() {
    observability.Init()
    // ...
}
```

**ASP.NET Core:**
```csharp
var builder = WebApplication.CreateBuilder(args);
ObservabilityInit.Init(builder);
// ...
```

### Tracing (OpenTelemetry)

**Golang:**
- otelsql untuk PostgreSQL tracing
- OTLP HTTP exporter
- AlwaysSample

**ASP.NET Core:**
- Npgsql.OpenTelemetry untuk PostgreSQL tracing
- OTLP HTTP exporter
- AlwaysOnSampler

**Result:** SAMA - trace dari HTTP → DB query

### Metrics (Prometheus)

**Golang:**
```go
http_requests_total{method, endpoint, status}
http_request_duration_seconds{method, endpoint}
orders_created_total
```

**ASP.NET Core:**
```csharp
http_requests_total{method, endpoint, status}
http_request_duration_seconds{method, endpoint}
orders_created_total
```

**Result:** SAMA - metrics 1:1

### Profiling (Pyroscope)

**Golang:**
- pyroscope-go
- CPU + memory profiling

**ASP.NET Core:**
- Pyroscope .NET SDK
- CPU + allocation profiling

**Result:** SAMA - profiling otomatis

## 🔍 Business Logic Parity

### Clean Separation (IDENTIK)

**Golang:**
- Handlers: NO observability code
- Repository: NO observability code
- Observability: Isolated in `/observability`

**ASP.NET Core:**
- Controllers: NO observability code
- Repository: NO observability code
- Observability: Isolated in `/Observability`

**Result:** SAMA - clean separation

## 🚀 Usage

### Build & Run

```bash
# Restore dependencies
dotnet restore

# Run
dotnet run

# Docker
docker build -t dotnet-ecommerce .
docker run -p 8080:8080 dotnet-ecommerce
```

### Environment Variables (SAMA dengan Go)

```bash
OTEL_SERVICE_NAME=dotnet-ecommerce
OTEL_EXPORTER_OTLP_ENDPOINT=http://alloy.monitoring.svc.cluster.local:4318
PYROSCOPE_ENDPOINT=http://pyroscope-distributor.monitoring.svc.cluster.local:4040
DATABASE_DSN=Host=postgres.app.svc.cluster.local;Database=shop;Username=postgres;Password=postgres
```

## 📊 Request Flow (IDENTIK)

### Golang Flow
```
HTTP Request → main.go → handlers/product.go → repository/postgres.go → PostgreSQL
```

### ASP.NET Core Flow
```
HTTP Request → Program.cs → Controllers/HomeController.cs → Repository/ProductRepository.cs → PostgreSQL
```

## 🎓 Key Principles (SAMA)

### 1. Single Observability Entry Point
- **Go**: `observability.Init()` di `main.go`
- **.NET**: `ObservabilityInit.Init()` di `Program.cs`

### 2. Auto-Instrumentation
- **Go**: otelsql untuk PostgreSQL
- **.NET**: Npgsql.OpenTelemetry untuk PostgreSQL

### 3. Clean Business Logic
- **Go**: Handlers tidak tahu tentang tracing
- **.NET**: Controllers tidak tahu tentang tracing

### 4. Metrics Compatibility
- **Go**: Prometheus client
- **.NET**: prometheus-net
- **Format**: SAMA (Prometheus format)

### 5. Profiling
- **Go**: pyroscope-go
- **.NET**: Pyroscope .NET SDK
- **Result**: SAMA (CPU + memory)

## 🔧 Observability Stack (LGTM-P)

| Component | Endpoint | Purpose |
|-----------|----------|---------|
| **Loki** | stdout logs | Structured logging |
| **Grafana** | - | Visualization |
| **Tempo** | OTLP HTTP | Distributed tracing |
| **Mimir** | /metrics | Prometheus metrics |
| **Pyroscope** | HTTP | Continuous profiling |

## ✅ Functional Equivalence Checklist

- ✅ **Endpoints**: GET /, POST /checkout, GET /success, GET /metrics
- ✅ **Database**: PostgreSQL dengan auto-tracing
- ✅ **Tracing**: OpenTelemetry → Tempo via Alloy
- ✅ **Metrics**: Prometheus format, custom counters
- ✅ **Profiling**: Pyroscope CPU + memory
- ✅ **Logging**: Structured logs dengan trace correlation
- ✅ **Clean Code**: NO observability in business logic
- ✅ **Docker**: Multi-stage build, port 8080
- ✅ **AKS Ready**: Stateless, config via ENV, graceful shutdown

## 🔄 Migration Path

Jika Anda familiar dengan Golang version:

1. **Folder structure** → SAMA (translated idiomatically)
2. **Observability approach** → SAMA (single entry point)
3. **Tracing philosophy** → SAMA (auto-instrumentation)
4. **Metrics format** → SAMA (Prometheus compatible)
5. **Business logic** → SAMA (clean separation)

**Conclusion:** ASP.NET Core version adalah **FUNCTIONAL EQUIVALENT** dari Golang version!

---

**Aplikasi ASP.NET Core ini 1:1 equivalent dengan Golang untuk LGTM-P stack!** 🚀
