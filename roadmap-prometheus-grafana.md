# Phase 8 - Monitoring (Prometheus & Grafana)

## Goal

- Expose metrics dari ASP.NET
- Scrape metrics menggunakan Prometheus
- Visualisasi metrics menggunakan Grafana

---

# 1. Install Package

```bash
dotnet add package prometheus-net.AspNetCore
```

Digunakan untuk menambahkan endpoint `/metrics`.

---

# 2. Program.cs

```csharp
app.UseHttpMetrics();

app.MapMetrics();
```

- `UseHttpMetrics()` → mencatat metrics HTTP request.
- `MapMetrics()` → membuat endpoint `/metrics`.

---

# 3. Test Metrics

```
GET /metrics
```

Harus muncul daftar metrics Prometheus.

---

# 4. docker-compose.yml

```yaml
prometheus:
  image: prom/prometheus
  ports:
    - "9090:9090"
  volumes:
    - ./prometheus/prometheus.yml:/etc/prometheus/prometheus.yml
```

Menjalankan Prometheus.

---

# 5. prometheus.yml

```yaml
global:
  scrape_interval: 15s

scrape_configs:
  - job_name: backend

    static_configs:
      - targets:
          - backend-1:5030
          - backend-2:5030
```

Konfigurasi target scraping.

---

# 6. Check Targets

```
http://localhost:9090/targets
```

Pastikan status **UP**.

---

# 7. Useful PromQL

Request per second

```promql
sum(rate(http_requests_received_total[1m]))
```

Total request

```promql
sum(http_requests_received_total)
```

GET request

```promql
sum(http_requests_received_total{method="GET"})
```

Error 5xx

```promql
sum(rate(http_requests_received_total{code=~"5.."}[1m]))
```

Average latency

```promql
rate(http_request_duration_seconds_sum[5m])
/
rate(http_request_duration_seconds_count[5m])
```

---

# 8. Grafana

Datasource

```
http://prometheus:9090
```

Bukan `localhost`, karena Grafana berjalan di dalam container.

---

# 9. Dashboard

Request/sec

```promql
sum(rate(http_requests_received_total[1m]))
```

Total Requests

```promql
sum(http_requests_received_total)
```

Backend Status

```promql
up
```

---

# 10. Troubleshooting

## Grafana tidak bisa connect

❌

```
http://localhost:9090
```

✅

```
http://prometheus:9090
```

---

## Target DOWN

- Cek `docker ps`
- Cek `/metrics`
- Cek `prometheus.yml`

---

## Grafana gagal start

```
database or disk is full (13)
```

Penyebab:

- Disk Linux penuh.

Cek:

```bash
df -h
docker system df
```
