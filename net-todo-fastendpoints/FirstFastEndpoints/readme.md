FastEndpoints Starter 


### List Package
- FastEndpoints (8.1.0)
- FastEndpoints.Swagger (8.1.0)
- Scalar.AspNetCore (2.14.14)

### List What i have already Done
- **Project Infrastructure**:
    - Configured FastEndpoints and Response Caching in `Program.cs`.
    - Integrated Scalar for OpenAPI documentation and UI.
    - Implemented `LoggiingPreProcessor` for request logging.
- **Domain**:
    - Defined `TodoItem` entity.
- **Features (Todo)**:
    - `CreateTodo`: Implemented endpoint with request/response mapping and validator.
    - `GetAllTodos`: Implemented endpoint to retrieve all todo items.
    - `DeleteTodo` & `UpdateTodo`: Created folder structure for upcoming features.


## Structure Folder

PROJECT_ARCHITECTURE_GUIDE.md

Fullstack ASP.NET + Flutter Learning Roadmap

Goal

Membangun project yang:

- Mudah dipelajari
- Mengikuti praktik industri modern
- Tidak over-engineering
- Cocok untuk portfolio
- Mudah berkembang menjadi production-ready

---

Technology Stack

Backend

- ASP.NET Core 9 Web API
- Minimal API
- Entity Framework Core
- PostgreSQL
- FluentValidation
- JWT Authentication
- Refresh Token
- Serilog
- Swagger/OpenAPI
- Docker

Mobile

- Flutter
- flutter_bloc
- dio
- get_it
- go_router
- freezed
- json_serializable
- flutter_secure_storage

---

Backend Architecture

Menggunakan kombinasi:

- Clean Architecture (simplified)
- Feature-Based Structure
- Vertical Slice Architecture

Folder Structure

src/

├── Api/
│   ├── Extensions/
│   ├── Middlewares/
│   └── Program.cs
│
├── Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── ValueObjects/
│   └── Common/
│
├── Infrastructure/
│   ├── Persistence/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/
│   │
│   ├── Services/
│   ├── Repositories/
│   └── Identity/
│
└── Features/
    ├── Auth/
    │
    │   ├── Login/
    │   ├── Register/
    │   └── RefreshToken/
    │
    ├── Users/
    │   ├── CreateUser/
    │   ├── GetUser/
    │   └── UpdateUser/
    │
    └── Products/

---

Layer Responsibilities

Api

Berisi:

- Middleware
- Dependency Injection Registration
- Endpoint Mapping
- Application Startup

Tidak berisi business logic.

---

Domain

Berisi:

- Entities
- Enums
- Value Objects
- Business Rules

Contoh:

public class User
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
}

Domain tidak boleh mengetahui database.

---

Infrastructure

Berisi:

- EF Core
- PostgreSQL
- Authentication
- Logging
- External Services

Contoh:

AppDbContext
JwtService
EmailService
FileStorageService

---

Features

Setiap fitur memiliki folder sendiri.

Contoh:

Features/

└── Users/
    └── CreateUser/

Isi:

CreateUser/

├── Endpoint.cs
├── Request.cs
├── Response.cs
├── Validator.cs
└── Handler.cs

Semua kode terkait use case berada dalam satu tempat.

---

Endpoint Pattern

Request

public record CreateUserRequest(
    string Name,
    string Email
);

---

Validator

public class CreateUserValidator
{
}

---

Handler

public class CreateUserHandler
{
}

---

Response

public record CreateUserResponse(
    Guid Id,
    string Name
);

---

Repository Pattern

Untuk tahap belajar:

Gunakan AppDbContext secara langsung.

Contoh:

public class CreateUserHandler
{
    private readonly AppDbContext _db;

    public CreateUserHandler(AppDbContext db)
    {
        _db = db;
    }
}

Hindari membuat repository hanya untuk membungkus:

_db.Users

Gunakan repository hanya ketika benar-benar diperlukan.

---

Recommended NuGet Packages

Database

Microsoft.EntityFrameworkCore
Npgsql.EntityFrameworkCore.PostgreSQL

Validation

FluentValidation

Authentication

Microsoft.AspNetCore.Authentication.JwtBearer

Logging

Serilog.AspNetCore
Serilog.Sinks.Console

Documentation

Swashbuckle.AspNetCore

---

Flutter Architecture

Menggunakan:

- Feature First
- Clean Architecture

Folder Structure

lib/

├── core/
│
├── features/
│
│   ├── auth/
│   ├── users/
│   └── products/
│
└── main.dart

---

Feature Structure

Contoh:

auth/

├── data/
│
├── domain/
│
└── presentation/

---

Data

Berisi:

Datasource
Repository Implementation
Models

---

Domain

Berisi:

Entities
UseCases
Repository Contracts

---

Presentation

Berisi:

Pages
Widgets
Bloc/Cubit

---

Flutter Packages

Networking

dio:

---

State Management

flutter_bloc:

---

Dependency Injection

get_it:

---

Routing

go_router:

---

Serialization

json_serializable:
freezed:

---

Local Secure Storage

flutter_secure_storage:

---

Authentic