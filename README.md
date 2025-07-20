# MyProject (Clean Architecture with ASP.NET Core, Blazor & Dapper)

This repository implements a **modular ASP.NET Core solution** using **Clean Architecture**, **Blazor WebAssembly**, and **Dapper** as the ORM. The codebase follows strict **coding standards**, with a layered architecture and separation of concerns.

---

## 🧱 Solution Architecture

- **MyProject.API** – RESTful Web API(not added yet)
- **MyProject.Admin** – Blazor WebAssembly app for Admin users
- **MyProject.Client** – Blazor WebAssembly app for Client users
- **MyProject.Core** – Business logic interfaces 
- **MyProject.Models** – Models/Domain entities and logic
- **MyProject.DataAccess** – Data access (Dapper), external services
- **MyProject.Utilities** – Shared models and result wrappers

---

## ✅ Tech Stack

- ASP.NET Core 8
- Blazor WebAssembly (Admin & Client)
- Dapper for data access
- SQL Server (or other DBs)
- Clean Architecture principles
- Dependency Injection
- `appsettings.json` for configuration
- `IConfiguration`, `IOptions`, and `ILogger`
- `.editorconfig` for style rules
- C# 12 features (file-scoped namespaces, expression-bodied members)

---

## ✨ Coding Standards (`.editorconfig`)

Highlights from our enforced rules:

- Use **file-scoped namespaces**
- Use **camelCase with _ prefix** for private fields
- Organize `using` directives: System first, outside namespace
- Prefer `var` where appropriate
- Remove trailing whitespace
- Final newline at EOF
- Group using directives logically

```csharp
namespace MyProject.Utilities.Extensions;

public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? input)
        => string.IsNullOrWhiteSpace(input);
}


## 🧩 Project Details

### 📌 ProjectName.API
The main entry point of the application. Exposes APIs or UI interfaces and handles incoming HTTP requests.

- Controllers or Pages (Blazor)
- Middlewares, Filters
- Dependency injection setup
- Exception handling

---

### 📌 ProjectName.Core
Contains the core business logic and application service interfaces.

- Services (application logic)
- Interfaces (e.g., `IUserService`, `INotificationService`)
- Use cases (if applying CQRS or similar patterns)

---

### 📌 ProjectName.Models
Holds plain C# classes used as data models across the solution. This includes:

- Entities (used by API, DB, and services)
- Enums and constants
- DTOs (if needed for external interfaces)

Note: This layer contains no logic or interfaces, just pure data structures.

---

### 📌 ProjectName.DataAccess
Responsible for data persistence and database operations.

- Dapper or Entity Framework repositories
- SQL queries or stored procedures
- DbContext, DbFactory, IDbConnection

---

### 📌 ProjectName.Utilities
Common utilities and reusable logic across all layers.

- Extension methods
- Helper classes (e.g., `DateHelper`, `JwtHelper`)
- App-wide constants
- Validation functions

---

## ✅ Naming Conventions

- Each project uses consistent and descriptive namespaces (e.g., `ProjectName.Models.Entities`)
- All logic is separated by responsibility to follow SOLID and Clean Architecture principles

---

## 🚀 Getting Started

1. Clone the repository
2. Open `ProjectName.sln` in Visual Studio or Rider
3. Set `ProjectName.API` as the startup project
4. Update `appsettings.json` and connection strings as needed
5. Run the application

---

## 🤝 Contributions

Please follow the existing project structure and naming conventions.
- Place models in `ProjectName.Models`
- Add shared logic to `ProjectName.Utilities`
- Follow separation of concerns when creating new services or features

---

## 📄 License

MIT License. See `LICENSE` file for details