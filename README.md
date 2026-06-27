# TodoFullstack FastEndpoints & React

A fullstack Todo application demonstrating the use of **FastEndpoints** in .NET 10 and **React** with Vite on the frontend.

## 🏗 Project Structure

```text
.
├── client-todo-fastendpoints/    # Frontend Application
│   ├── src/
│   │   ├── api/                  # API integration (Axios)
│   │   ├── components/           # UI Components
│   │   ├── store/                # State management (Redux Toolkit)
│   │   ├── types/                # TypeScript definitions
│   │   └── App.tsx               # Main Application component
│   └── Dockerfile                # Frontend Docker configuration
│
├── net-todo-fastendpoints/       # Backend API (.NET 10)
│   ├── FirstFastEndpoints/
│   │   ├── Api/                  # Entry point & Program.cs
│   │   ├── Data/                 # AppDbContext & Migrations
│   │   ├── Domain/               # Entities (TodoItem)
│   │   ├── Features/             # Vertical Slices (Endpoints, Logic, Validators)
│   │   │   ├── Todo/             # Todo feature group
│   │   │   │   ├── CreateTodo/
│   │   │   │   ├── DeleteTodo/
│   │   │   │   ├── GetAllTodos/
│   │   │   │   └── UpdateTodo/
│   │   └── Shared/               # Common Middlewares/PreProcessors
│   ├── Dockerfile                # Backend Docker configuration
│   └── FirstFastEndpoints.slnx   # Solution file
│
└── docker-compose.yml            # Docker orchestration
```

## 🚀 Technologies

### Backend
- **Framework**: .NET 10 with [FastEndpoints](https://fast-endpoints.com/)
- **ORM**: Entity Framework Core (In-Memory for now)
- **Validation**: FluentValidation
- **API Documentation**: Scalar / Swagger
- **Architecture**: Vertical Slice Architecture

### Frontend
- **Framework**: React 18+ (Vite)
- **Language**: TypeScript
- **State Management**: Redux Toolkit
- **Styling**: Tailwind CSS (Check `index.css`)
- **API Client**: Axios

## 🛠 Getting Started

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (optional, for local development)
- [Node.js](https://nodejs.org/) (optional, for local development)

### Running with Docker
1. Clone the repository.
2. Run the following command in the root directory:
   ```bash
   docker-compose up --build
   ```
3. Access the applications:
   - **Frontend**: [http://localhost:5173](http://localhost:5173)
   - **Backend API**: [http://localhost:5000](http://localhost:5000)
   - **API Docs (Scalar)**: [http://localhost:5000/scalar/v1](http://localhost:5000/scalar/v1)

## 📖 Roadmap & Learning
See [roadmap-belajar-todo-docker-vm.md](./roadmap-belajar-todo-docker-vm.md) for the project's learning objectives and future plans.
