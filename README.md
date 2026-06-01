# Secure Session Auth & Role-Based Access Control API

A production-optimized ASP.NET Core Web API demonstrating modular clean architecture, cookie-based session state verification, Role-Based Access Control (RBAC), and automated database integration diagnostics.

## Features
* **Modular Engineering:** Complete split between Controllers, Entities, DTOs, Business Services, and Context Providers.
* **Stateful Session Governance:** Cookies utilizing `HttpOnly`, `SameSite=Strict`, and `Secure` tracking flags.
* **Granular RBAC Security Guards:** Route filtering driven by claim mapping arrays (`[Authorize(Roles = "Admin")]`).
* **Integrated Health Checks System:** Native automated diagnostics tracking SQL Server connectivity performance.

---

## Development Engine Initialization

### 1. Database Generation Framework
Verify that the connection parameters inside `appsettings.json` point to your running SQL Server instance, then run:
```bash
dotnet ef migrations add InitialSessionSchema
dotnet ef database update