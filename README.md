# TaskBoard

A real-time collaborative task board — think Trello — where multiple users see card movements live without refreshing. Built with ASP.NET Core Web API, SignalR, Angular, and JWT authentication.

**Live demo:** https://taskboard-anishkoirala.netlify.app

---

## Features

- **Real-time sync** — card movements, renames, and deletions broadcast instantly to all connected users via SignalR — no polling, no refresh
- **Drag-and-drop** — move cards across columns; every other user sees it happen live
- **JWT authentication** — secure login with access tokens and refresh token rotation
- **Board & column management** — create boards, add columns, reorder them
- **Card management** — create, edit, move, and delete cards with live updates
- **Tested service layer** — core business logic covered by xUnit tests

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API, C# |
| Real-time | SignalR |
| Auth | JWT (access + refresh tokens) |
| Database | MSSQL + Entity Framework Core |
| Frontend | Angular |
| Testing | xUnit |
| Deployment | (your host here — e.g. Railway, Render, Azure) |

---

## Architecture

```
TaskBoard/
├── TaskBoard.API/              # ASP.NET Core Web API
│   ├── Controllers/            # REST endpoints (boards, columns, cards, auth)
│   ├── Hubs/                   # SignalR BoardHub
│   └── Middleware/             # JWT validation, error handling
├── TaskBoard.Application/      # Use cases, service interfaces, DTOs
├── TaskBoard.Domain/           # Entities: Board, Column, Card, User
├── TaskBoard.Infrastructure/   # EF Core, repositories, JWT service
├── TaskBoard.Tests/            # xUnit tests for the Application layer
└── taskboard-client/           # Angular frontend
    ├── src/app/
    │   ├── board/              # Board and column components
    │   ├── card/               # Card components + drag-and-drop
    │   ├── auth/               # Login, register, token interceptor
    │   └── services/           # SignalR service, API service
```

---

## How the real-time sync works

1. When a user moves a card, the Angular client sends a `PATCH /api/cards/{id}` request to the API
2. The API updates the database, then calls `BoardHub.Clients.Group(boardId).SendAsync("CardMoved", payload)`
3. All other clients connected to that board receive the `CardMoved` event via their SignalR connection
4. The Angular SignalR service patches the local board state — no full reload needed

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) and [Angular CLI](https://angular.io/cli)
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

### Backend setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/uwunish/TaskBoard.git
   cd TaskBoard
   ```

2. **Configure settings**

   Update `TaskBoard.API/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=TaskBoardDb;Trusted_Connection=True;"
     },
     "Jwt": {
       "Key": "your-secret-key-min-32-chars",
       "Issuer": "taskboard-api",
       "Audience": "taskboard-client",
       "AccessTokenExpiryMinutes": 15,
       "RefreshTokenExpiryDays": 7
     }
   }
   ```

3. **Apply migrations and run**
   ```bash
   dotnet ef database update --project TaskBoard.Infrastructure --startup-project TaskBoard.API
   dotnet run --project TaskBoard.API
   ```

   API runs at `https://localhost:5001`

### Frontend setup

```bash
cd taskboard-client
npm install
ng serve
```

Client runs at `http://localhost:4200`

---

## Running the tests

```bash
dotnet test TaskBoard.Tests
```

Tests cover the service layer — board creation, card movement logic, column reordering, and JWT token generation/validation.

---

## API Reference

### Auth
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Login, returns access + refresh token |
| POST | `/api/auth/refresh` | Exchange refresh token for new access token |

### Boards
| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/boards` | Get all boards for current user |
| POST | `/api/boards` | Create a new board |
| DELETE | `/api/boards/{id}` | Delete a board |

### Columns
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/boards/{boardId}/columns` | Add a column to a board |
| PATCH | `/api/columns/{id}` | Rename or reorder a column |
| DELETE | `/api/columns/{id}` | Delete a column |

### Cards
| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/columns/{columnId}/cards` | Add a card to a column |
| PATCH | `/api/cards/{id}` | Move or edit a card (triggers SignalR broadcast) |
| DELETE | `/api/cards/{id}` | Delete a card |

### SignalR Hub — `/hubs/board`

| Client method to call | Description |
|---|---|
| `JoinBoard(boardId)` | Subscribe to live updates for a board |
| `LeaveBoard(boardId)` | Unsubscribe |

| Server event (listen for) | Payload |
|---|---|
| `CardMoved` | `{ cardId, fromColumnId, toColumnId, newIndex }` |
| `CardCreated` | `{ card }` |
| `CardDeleted` | `{ cardId }` |
| `ColumnAdded` | `{ column }` |

---

## Environment variables (production)

```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=...
Jwt__Key=...
Jwt__Issuer=...
Jwt__Audience=...
ALLOWED_ORIGINS=https://your-frontend-url.com
```

---

## License

MIT
