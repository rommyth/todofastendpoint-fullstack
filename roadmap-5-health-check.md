# Health Checks Roadmap (ASP.NET Core + FastEndpoints)

## Tujuan

Health Checks digunakan untuk mengetahui apakah aplikasi **masih siap melayani request**, bukan hanya sekadar masih berjalan.

Health Checks melengkapi:

- **Serilog** → Apa yang terjadi?
- **Prometheus** → Bagaimana performa aplikasi?
- **Health Checks** → Apakah aplikasi dan seluruh dependency masih sehat?

---

# Konsep Dasar

## Health Check ≠ Ping

HTTP 200 pada root endpoint belum tentu berarti aplikasi sehat.

Contoh:

```
Application     ✅
Database        ❌
Redis           ❌
```

Aplikasi masih berjalan, tetapi endpoint bisnis dapat gagal.

Health Check bertugas memverifikasi seluruh dependency penting.

---

# Liveness vs Readiness

## Liveness

Menjawab:

> Apakah proses aplikasi masih hidup?

Contoh endpoint:

```
GET /health/live
```

Biasanya hanya mengecek bahwa aplikasi masih berjalan.

---

## Readiness

Menjawab:

> Apakah aplikasi siap menerima request?

Contoh endpoint:

```
GET /health/ready
```

Biasanya mengecek:

- Database
- Redis
- Message Broker
- External API

Jika salah satu dependency gagal, status berubah menjadi **Unhealthy**.

---

# Package

Health Checks merupakan fitur bawaan ASP.NET Core.

Untuk dependency tertentu digunakan package tambahan.

Contoh:

- AspNetCore.HealthChecks.Redis
- AspNetCore.HealthChecks.NpgSql
- AspNetCore.HealthChecks.SqlServer
- AspNetCore.HealthChecks.MongoDb
- AspNetCore.HealthChecks.RabbitMQ

---

# Registrasi

```csharp
builder.Services
    .AddHealthChecks()
    .AddRedis(
        builder.Configuration["Redis:Host"]!,
        name: "redis");
```

---

# Endpoint

```csharp
app.MapHealthChecks("/health");
```

Endpoint:

```
GET /health
```

---

# Status Health

Terdapat tiga status utama.

## Healthy

Seluruh dependency berjalan normal.

---

## Degraded

Aplikasi masih dapat digunakan, tetapi ada masalah yang belum menyebabkan kegagalan total.

Contoh:

- External API lambat
- Cache tidak tersedia tetapi aplikasi masih bisa menggunakan database

---

## Unhealthy

Dependency penting gagal.

Contoh:

- PostgreSQL mati
- Redis tidak dapat dihubungi
- RabbitMQ gagal terkoneksi

---

# Custom Response

Response bawaan ASP.NET Core hanya menghasilkan:

```
Healthy
```

Untuk kebutuhan production disarankan membuat response JSON yang lebih informatif.

Contoh:

```json
{
  "status": "Healthy",
  "timestamp": "2026-07-14T14:20:30Z",
  "totalDuration": "00:00:00.005",
  "checks": {
    "redis": {
      "status": "Healthy",
      "duration": "00:00:00.002"
    },
    "postgres": {
      "status": "Healthy",
      "duration": "00:00:00.003"
    }
  }
}
```

Jika salah satu dependency gagal:

```json
{
  "status": "Unhealthy",
  "checks": {
    "redis": {
      "status": "Healthy"
    },
    "postgres": {
      "status": "Unhealthy",
      "description": "Unable to connect."
    }
  }
}
```

---

# Best Practice

- Gunakan package yang sudah matang untuk dependency umum.
- Berikan nama (`name`) pada setiap Health Check agar mudah diidentifikasi.
- Buat response JSON yang informatif, bukan hanya "Healthy".
- Pisahkan endpoint Liveness dan Readiness jika aplikasi mulai berkembang.
- Jangan mengecek dependency yang tidak memengaruhi kemampuan aplikasi melayani request.

---

# Status Pembelajaran

Selesai dipelajari:

- Konsep Health Checks
- Perbedaan dengan Serilog dan Prometheus
- Liveness vs Readiness
- Registrasi Health Checks
- Package Health Checks
- Redis Health Check
- Custom Response JSON
- Struktur response yang direkomendasikan
- Best Practice penggunaan

---

# Roadmap Berikutnya

1. Configuration Management (Options Pattern)
2. Global Exception Handling
3. Rate Limiting
4. API Versioning
5. Security Hardening
6. Performance Optimization
7. Testing (Unit & Integration)
8. Production Readiness
