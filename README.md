# Todo List Application

A full-stack todo list application with  **.NET 8** backend API and **React + TypeScript** frontend, built with **Clean Architecture** pattern and following **SOLID** design principles.

## Architecture Overview

This application implements **Clean Architecture** with clear separation between frontend and backend:

### Layers:

**Backend (.NET API):**
1. **Domain Layer** - Core business entities and interfaces (no dependencies)
2. **Application Layer** - Business logic, DTOs, and service interfaces
3. **Infrastructure Layer** - Data persistence and external dependencies
4. **API Layer** - REST endpoints, controllers, and middleware

**Frontend (React):**
1. Modern React 18 with TypeScript
2. Component-based architecture
3. Service layer for API communication
4. Type-safe development

## Features

**Backend:**
- Complete CRUD operations for todo items
- Mark items as complete/incomplete
- Filter todos by completion status and overdue items
- JSON file-based persistence
- Comprehensive input validation
- Global exception handling
- Unit tests with xUnit and Moq
- Swagger/OpenAPI documentation
- Docker containerization
- SOLID principles implementation

**Frontend:**
- Modern React UI with TypeScript
- Real-time filtering and sorting
- Inline editing of todos
- Responsive design
- Visual indicators for overdue items

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and npm
- [Docker](https://www.docker.com/get-started) (optional, for containerization)
- Your favorite IDE (Visual Studio, VS Code, or Rider)

### Quick Start with Docker (Recommended)

The easiest way to run both frontend and backend:

```bash
# Build and run both services
docker-compose up --build
```

**Access the application:**
- Frontend: http://localhost:3000
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000

### Running Locally (Development)

**Terminal 1 - Backend:**
```bash
# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run the API
cd src/TodoList.API
dotnet run
```

Backend will be available at http://localhost:5000

**Terminal 2 - Frontend:**
```bash
# Navigate to frontend directory
cd frontend

# Install dependencies
npm install

# Start development server
npm start
```

Frontend will be available at http://localhost:3000

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity detailed

# Run tests with coverage (requires coverlet)
dotnet test /p:CollectCoverage=true
```

## CI/CD Pipeline
This project uses GitHub Actions for continuous integration and automated testing.

Every push to main or develop triggers:

- Backend Build & Test Compiles .NET solution and runs all unit tests
- Frontend Build Builds React application
- Docker Validation Ensures both Docker images build successfully

Workflow Configuration
The CI/CD pipeline is defined in .github/workflows/ci-cd.yml and includes:
yaml Jobs:
  - Backend: .NET 8 build + xUnit tests
  - Frontend: Node.js 18 build
  - Docker: Multi-stage image builds

Benefits:
- Catches bugs before they reach production
- Ensures code quality on every commit
- Validates Docker deployments
- Provides immediate feedback on PRs

## API Endpoints

### Todo Items

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/todos` | Get all todos (supports filtering) |
| GET | `/api/todos/{id}` | Get a specific todo by ID |
| POST | `/api/todos` | Create a new todo |
| PUT | `/api/todos/{id}` | Update an existing todo |
| PATCH | `/api/todos/{id}/complete` | Mark todo as completed |
| PATCH | `/api/todos/{id}/incomplete` | Mark todo as incomplete |
| DELETE | `/api/todos/{id}` | Delete a todo |

### Query Parameters (GET /api/todos)

- `isCompleted` (boolean): Filter by completion status
- `isOverdue` (boolean): Filter by overdue status

### Request/Response Examples

**Create Todo (POST /api/todos)**
```json
{
  "title": "Complete project documentation",
  "description": "Write comprehensive README and API docs",
  "dueDate": "2024-12-31"
}
```

**Response**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Complete project documentation",
  "description": "Write comprehensive README and API docs",
  "dueDate": "2024-12-31T00:00:00Z",
  "isCompleted": false,
  "createdAt": "2024-12-04T10:30:00Z",
  "updatedAt": null,
  "isOverdue": false
}
```

**Update Todo (PUT /api/todos/{id})**
```json
{
  "title": "Updated title",
  "description": "Updated description",
  "dueDate": "2024-12-25"
}
```

## SOLID Principles Implementation

### Single Responsibility Principle (SRP)
- Each class has one responsibility
- `TodoItem`: Represents todo entity with business rules
- `TodoService`: Handles business logic
- `JsonFileRepository`: Manages data persistence
- `TodosController`: Handles HTTP requests/responses

### Open/Closed Principle (OCP)
- Classes are open for extension but closed for modification
- New storage implementations can be added without modifying existing code
- Interface-based design allows easy extension

### Liskov Substitution Principle (LSP)
- Any `ITodoRepository` implementation can replace another
- Any `ITodoService` implementation can replace another
- Derived classes maintain the behavior contract of base abstractions

### Interface Segregation Principle (ISP)
- Small, focused interfaces
- `ITodoRepository`: Contains only necessary CRUD operations
- `ITodoService`: Contains only business operations

### Dependency Inversion Principle (DIP)
- High-level modules depend on abstractions, not concrete implementations
- `TodoService` depends on `ITodoRepository` interface
- `TodosController` depends on `ITodoService` interface
- Dependencies are injected via constructor

## Testing Strategy

### Unit Tests Coverage

1. **Domain Tests** (`TodoList.Domain.Tests`)
   - Entity creation and validation
   - Business rule enforcement
   - State transitions

2. **Application Tests** (`TodoList.Application.Tests`)
   - Service business logic
   - DTO mapping
   - Error handling
   - Mock repository interactions

### Test Execution

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/TodoList.Domain.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Configuration

### Application Settings (appsettings.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "DataFilePath": "Data/todos.json",
  "AllowedHosts": "*"
}
```

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set environment (Development/Production)
- `DataFilePath`: Custom path for todos.json file
- `ASPNETCORE_URLS`: Override default URLs

## Design Decisions & Trade-offs

### Design Decisions

1. **Clean Architecture**: Ensures separation of concerns and maintainability
2. **React SPA Frontend**: Modern, responsive UI that consumes the REST API
3. **TypeScript**: Type-safe frontend development with better IDE support
4. **File-based Storage**: Simple JSON persistence suitable for the assignment scope
5. **Repository Pattern**: Abstracts data access for easy testing and future migrations
6. **DTOs**: Separates internal models from API contracts
7. **Middleware for Exception Handling**: Centralized error handling across the application
8. **Swagger Integration**: Built-in API documentation and testing interface
9. **Component-Based UI**: Reusable React components for maintainable frontend code

### Trade-offs

1. **File-based vs Database**: Chose file-based for simplicity and time constraints. Production would use a proper database.
2. **In-Memory Caching**: Repository caches data in memory for performance but reloads on startup.

## Tech Stack

**Backend:**
- .NET 8
- C# with nullable reference types
- xUnit & Moq for testing
- Swagger/OpenAPI
- Docker

**Frontend:**
- React 18
- TypeScript
- Axios for API calls
- Lucide React (icons)

