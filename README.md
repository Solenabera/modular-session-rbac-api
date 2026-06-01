# 🛡️ Enterprise-Grade Modular Session Authentication & RBAC Engine

An enterprise-ready, security-first backend infrastructure engine engineered with **ASP.NET Core Web API** and **Microsoft SQL Server**. This architecture provides a stateful, cookie-based session lifecycle management model tightly coupled with zero-trust, granular **Role-Based Access Control (RBAC)** security layers.

---

## 🏗️ Architectural Layout & System Topography

The workspace enforces a strict **Controller-Service-Repository (Layered Architecture)** design pattern, isolating responsibilities into distinct technical boundaries:

```text
SecureSessionApi/
│
├── Controllers/                 # TRANSPORT LAYER (HTTP Routing & Security Filters)
├── Services/                    # LOGIC LAYER (Pure C# Decoupled Business Core)
│   ├── Interfaces/              # Operational abstraction boundaries
│   └── Implementations/         # Cryptographic executions and validation pipelines
├── Models/                      # CONTRACT DOMAIN (Schematic Entities & Network DTOs)
│   ├── Entities/                # Database mapping structures (1:1 with SQL Tables)
│   └── Dtos/                    # Immutable transfer contracts preventing over-posting
├── Data/                        # INFRASTRUCTURE LAYER (EF Core Context & Indexes)
├── Migrations/                  # Version-controlled database schema scripts
└── Properties/                  # Environment launch metrics (launchSettings.json)


📖 System Architecture & Security Principles
1. Separation of Concerns
The Transport Border (Controllers/): Functions as an HTTP gatekeeper. It parses incoming network primitives, runs schema validations, and applies declarative authorization attributes, keeping HTTP contexts entirely out of core logic.

The Orchestration Core (Services/): A sandbox of pure, testable C# logic. It has zero awareness of HttpContext, routing endpoints, or cookies. This absolute isolation guarantees the core identity workflows can be dropped into alternative transport wrappers (gRPC, message brokers, background workers) with zero modifications.

Data Isolation via DTOs (Models/Dtos/): Isolates the internal database entities from external internet boundaries. By mapping incoming payloads through explicit, immutable Data Transfer Objects (DTOs), the API completely neutralizes Over-Posting (Mass Assignment) data vulnerabilities.

2. Defensive Security Engineering (Mitigating XSS & CSRF)
Rather than using stateless, hijackable token strings (like JWTs in LocalStorage), this engine uses a Stateful Session Identity Pattern via specialized browser cookies (SecureAuthSessionApp) carrying strict environment flags:

HttpOnly = true: Blocks runtime JavaScript context execution, completely neutralizing token theft via Cross-Site Scripting (XSS).

SameSite = SameSiteMode.Strict: Restricts cookie transmission to top-level source queries initiated within your domain boundary, fully eliminating Cross-Site Request Forgery (CSRF) vectors.

SecurePolicy = CookieSecurePolicy.Always: Enforces transport security by blocking cookie transmission over unencrypted connections, requiring an HTTPS pipeline.

3. Cryptographic Standards & Database OptimizationsAdaptive Hashing (BCrypt): Uses high-entropy, adaptive-cost password processing with cryptographically unique salts to resist massive, hardware-parallel brute-force attacks.Fail-Fast Existential Scans: Uses non-tracking queries (.AsNoTracking()) for duplicate email checks to minimize memory allocations and thread overhead during onboarding.Database Index Scaling: Maps a unique database-level index onto the user email storage schema via the Fluent API, ensuring database lookups maintain high-speed $O(1)$ constant time complexity at scale.

🛠️ Operational Setup & Execution Run-Book
1. Prerequisites & Tooling
Runtime: .NET SDK 8.0 or 9.0 Core Environment

Database Instance: Microsoft SQL Server (Express, Developer, or localdb)

2. Implementation Steps
Apply Connection String: Update your database target path within appsettings.json.

Provision local environment profiles: Set up target ports and environment variables inside Properties/launchSettings.json.

3. Execute Database Blueprints:
    dotnet restore
    dotnet tool install --global dotnet-ef
    dotnet ef migrations add InitialSessionTables
    dotnet ef database update

4. Launch Application Engine:
    dotnet run

🧭 Interactive Swagger Verification Matrix
Testing is streamlined directly through Swagger UI 
(http://localhost:5050/swagger/index.html) using automated context settings (withCredentials = true) to handle session cookies seamlessly.

Step,Target Endpoint,HTTP Verb,Operational Scenario,Expected System Response
1,/health,GET,Telemetry System Integrity Assessment,200 OK with Healthy string payload. Validates SQL Server connectivity.
2,/api/Auth/signup,POST,Onboard standard User and elevated Admin test profiles.,200 OK confirming structural data persistency.
3,/api/Resource/shared-feed,GET,Attempt access as an anonymous request.,401 Unauthorized. Pipeline block confirms security guardrails are active.
4,/api/Auth/login,POST,Authenticate using standard User credentials.,200 OK. Server issues cryptographically isolated cookie to browser.
5,/api/Resource/shared-feed,GET,Access authorized profile assets with active session.,200 OK. Data stream unlocks successfully.
6,/api/Admin/dashboard,GET,Access administrative assets using standard user session.,403 Forbidden. RBAC engine blocks access due to insufficient privileges.
7,/api/Auth/logout,POST,Terminate active user credentials session.,200 OK. Sever-side map is purged; browser cookie container is wiped.
8,/api/Auth/login,POST,Authenticate using elevated Admin credentials.,200 OK. Establishes new elevated session tracking scope.
9,/api/Admin/dashboard,GET,Access restricted administrative dashboard data.,200 OK. RBAC permission rule grants full system access.


🔄 Scheme Evolutions & Migration Safety Guide
Entity Framework Core migrations work incrementally. When you modify your C# entities, the engine tracks differential changes instead of dropping your database.

Safe Modifications: Adding properties or tables maps to additive SQL statements (ALTER TABLE ADD). Existing data row objects are preserved.

Destructive Operations: Dropping a property or changing its structural data type results in structural drops (ALTER TABLE DROP). Always review your generated Up() code blocks within your migration tracking files to ensure data integrity before applying schema updates.