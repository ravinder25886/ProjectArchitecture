# MyProject (Clean Architecture with ASP.NET Core, Blazor & Dapper)

This repository implements a **modular ASP.NET Core solution** using **Clean Architecture**, **Blazor WebAssembly**, and **Dapper** as the ORM. The codebase follows strict **coding standards**, with a layered architecture and separation of concerns.

---

## 🧱 Solution Architecture

- **MyProject.API** – RESTful Web API(not added yet)
- **MyProject.Admin** – Blazor WebAssembly app for Admin users
- **MyProject.Client** – Blazor WebAssembly app for Client users
- **MyProject.Application** – Business logic interfaces and DTOs
- **MyProject.Domain** – Domain entities and logic
- **MyProject.Infrastructure** – Data access (Dapper), external services
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
