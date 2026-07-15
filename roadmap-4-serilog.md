# Serilog Notes (ASP.NET Core 8 + FastEndpoints)

## Tujuan

Serilog digunakan sebagai **logging provider** untuk ASP.NET Core dengan
dukungan **structured logging**.

## Mengapa Serilog?

- Structured logging
- Integrasi dengan `ILogger<T>`
- Konfigurasi melalui `appsettings.json`
- Mudah mengganti sink tanpa mengubah kode

## Arsitektur

`builder.Host.UseSerilog()` digunakan karena logging adalah konfigurasi
**Host**, bukan service DI.

- `builder.Host` → konfigurasi Host (Logging, Configuration, Lifetime)
- `builder.Services` → Dependency Injection

## Package

```bash
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Settings.Configuration
dotnet add package Serilog.Sinks.Console
```

## Program.cs

```csharp
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();
```

Tambahkan middleware:

```csharp
app.UseSerilogRequestLogging();
```

Letakkan sebelum middleware utama agar seluruh request tercatat.

## appsettings.json

```json
"Serilog": {
  "Using": [
    "Serilog.Sinks.Console"
  ],
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "WriteTo": [
    {
      "Name": "Console",
      "Args": {
        "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
      }
    }
  ]
}
```

### Penjelasan

- **Using** : assembly sink yang digunakan.
- **MinimumLevel** : level log minimum.
- **Override** : mengurangi log dari namespace tertentu (mis.
  Microsoft).
- **WriteTo** : daftar tujuan (sink).
- **outputTemplate** : format tampilan log.

## Structured Logging

Jangan:

```csharp
_logger.LogInformation($"User {user.Id} login");
```

Gunakan:

```csharp
_logger.LogInformation(
    "User {UserId} login",
    user.Id);
```

Keuntungan: - Property tersimpan sebagai data terstruktur. - Lebih mudah
difilter dan dianalisis. - Tidak hanya berupa string.

### Logging object

```csharp
_logger.LogInformation(
    "Request {@Request}",
    request);
```

Gunakan `@` hanya untuk object yang memang perlu dicatat.

## Kapan Logging?

### Endpoint

- Login
- Register
- Create / Update / Delete Todo

### Service

- Business process
- External API

### Middleware

- Validasi API Key
- Custom middleware

### Background Service

- Job mulai / selesai

### Startup

- Startup application

## Hindari Logging

- Setter sederhana
- Perhitungan biasa
- Loop yang tidak penting
- Setiap operasi CRUD internal repository tanpa nilai tambah

## Level Log

- Verbose
- Debug
- Information (default yang direkomendasikan)
- Warning
- Error
- Fatal

## Best Practice

- Tetap gunakan `ILogger<T>` di Endpoint/Service.
- Gunakan `Log.Information()` hanya untuk startup jika diperlukan.
- Gunakan placeholder (`{UserId}`), bukan string interpolation.
- Jangan log data sensitif.
- Log hanya business event penting.

## Status Pembelajaran

Selesai: - Integrasi Serilog - Konfigurasi - Request Logging -
Structured Logging - Best Practice penggunaan
