# TaskBoard

A RESTful task management API built with ASP.NET Core and Clean Architecture. Supports Docker for containerised deployment.

---

## Tech Stack

| Layer | Technology |
|---|---|
| API | ASP.NET Core Web API, C# |
| Architecture | Clean Architecture |
| Containerisation | Docker, Docker Compose |
| Testing | xUnit (Unit Tests) |
| Frontend | HTML, JavaScript |

---

## Project Structure

```
TaskBoard/
├── TaskBoard.Domain/           # Entities, domain interfaces, value objects
├── TaskBoard.Application/      # Use cases, DTOs, service interfaces
├── TaskBoard.Infrastructure/   # Data access, repository implementations
├── TaskBoard.API/              # Controllers, middleware, DI setup, Dockerfile
├── TaskBoard.UnitTests/        # Unit tests for application layer logic
├── docker-compose.yml
└── docker-compose.override.yml
```

Each layer depends only inward — the Domain layer has zero external dependencies, keeping all business logic isolated and independently testable.

---

## Getting Started

### Option 1 — Docker (recommended)

**Prerequisites:** [Docker Desktop](https://www.docker.com/products/docker-desktop/)

```bash
git clone https://github.com/uwunish/TaskBoard.git
cd TaskBoard
docker-compose up --build
```

The API will be available at `http://localhost:<port>` — check `docker-compose.override.yml` for the mapped port.

---

### Option 2 — Local (.NET)

**Prerequisites:**
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

1. **Clone the repository**
   ```bash
   git clone https://github.com/uwunish/TaskBoard.git
   cd TaskBoard
   ```

2. **Configure the connection string**

   Update `TaskBoard.API/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=TaskBoardDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project TaskBoard.Infrastructure --startup-project TaskBoard.API
   ```

4. **Run the API**
   ```bash
   dotnet run --project TaskBoard.API
   ```

   Navigate to `https://localhost:<port>/swagger` for the interactive API docs.

---

## Running Tests

```bash
dotnet test TaskBoard.UnitTests
```

---

## Architecture

This project follows **Clean Architecture**, enforcing strict dependency rules across layers:

```
         ┌─────────────────────┐
         │     TaskBoard.API   │  ← Entry point, DI wiring, controllers
         └────────┬────────────┘
                  │
         ┌────────▼────────────┐
         │  TaskBoard.         │  ← Use cases, application services, DTOs
         │  Application        │
         └────────┬────────────┘
                  │
         ┌────────▼────────────┐
         │  TaskBoard.Domain   │  ← Entities, domain interfaces (no dependencies)
         └─────────────────────┘
                  ▲
         ┌────────┴────────────┐
         │  TaskBoard.         │  ← EF Core, repositories, DB migrations
         │  Infrastructure     │
         └─────────────────────┘
```

Infrastructure implements interfaces defined in the Domain/Application layers — the core logic never depends on database or framework details.

---

## License

MIT
