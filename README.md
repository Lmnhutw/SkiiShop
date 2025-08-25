# SkiiShop Clean Architecture

Welcome to **SkiiShop**, a sample e-commerce project demonstrating Clean Architecture principles in .NET.  
This repository separates application concerns into Core, Infrastructure, and API layers for maintainability, testability, and scalability.

---

## Project Structure -- CURRENTLY

```
SkiiShop/
├── Core/             # Domain entities, business rules, and abstractions
├── Infrastructure/   # Implementations: data access, external integrations, services
├── SShopAPI/         # Presentation layer: API endpoints, DTOs, DI setup
```

### Layer Responsibilities

- **Core**
  - Domain entities (e.g., `Product`)
  - Interfaces/abstractions (`IRepository<T>`)
  - Business rules

- **Infrastructure**
  - Implements Core abstractions (e.g., `Repository<T>`)
  - Handles data persistence (e.g., Entity Framework Core, DbContext)
  - External service integrations

- **SShopAPI**
  - Minimal API endpoints (e.g., `/products`)
  - Accepts and returns DTOs
  - Configures dependency injection

---

## How It Works

- **Core** defines "what" the application does.
- **Infrastructure** defines "how" it does it—connecting to databases, services, etc.
- **SShopAPI** exposes the functionality to clients through HTTP endpoints.

**Dependency direction:**  
- Infrastructure and API both depend on Core—but Core depends on nothing.

---

## Getting Started

1. **Clone the repository**
    ```sh
    git clone https://github.com/Lmnhutw/SkiiShop.git
    ```

2. **Configure the Database**
    - Update the connection string in `SShopAPI/appsettings.json`:
      ```json
      "ConnectionStrings": {
        "DefaultConnection": "YourDatabaseConnectionString"
      }
      ```

3. **Run Migrations**
    ```sh
    cd SShopAPI
    dotnet ef database update
    ```

4. **Run the API**
    ```sh
    dotnet run --project SShopAPI
    ```

5. **Try Endpoints**
    - Example: [GET] `/products`

---

## Key Concepts

- **Generic Repository Pattern**
  - `IRepository<T>` in Core defines CRUD contracts.
  - `Repository<T>` in Infrastructure implements them using `SS_DbContext`.

- **DbContext**
  - Defined in Infrastructure as `SS_DbContext`, registers domain models (e.g., `DbSet<Product>`).

- **Minimal API Endpoints**
  - Defined in `SShopAPI.Endpoints`. Use dependency-injected `IRepository<Product>` for data access.
  - DTOs are mapped to/from domain entities as needed.

---

## Example API Endpoint

```csharp
app.MapGet("/products", async (IRepository<Product> repo) =>
{
    var products = await repo.GetAllAsync();
    // Optionally map to DTOs
    return Results.Ok(products);
});
```

---

## Clean Architecture Diagram

```
+-------------------+
|    SShopAPI       |  <-- Presentation (API Endpoints, DTOs)
+-------------------+
          |
          v
+-------------------+
|      Core         |  <-- Entities, Interfaces, Business Logic
+-------------------+
          ^
          |
+-------------------+
|  Infrastructure   |  <-- Data, Services, Integrations
+-------------------+
```

- **Dependencies always point inward.**
- Core is independent and reusable.

---

