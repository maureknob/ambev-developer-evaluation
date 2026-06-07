# Step 6.4 — MongoDB: DI Registration and Configuration

**Branch:** `feature/SALE-6-mongo-readmodel`  
**Commit:** `feat(ioc): register MongoDB client and MongoSaleRepository in DI`

---

## Files to Modify

- `src/Ambev.DeveloperEvaluation.IoC/ModuleInitializers/InfrastructureModuleInitializer.cs` (or equivalent)
- `src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`
- `src/Ambev.DeveloperEvaluation.WebApi/appsettings.Development.json`

---

## appsettings.json — add MongoDB section

```json
"MongoDB": {
  "ConnectionString": "mongodb://developer:ev%40luAt10n@ambev.developerevaluation.nosql:27017",
  "DatabaseName": "developer_evaluation"
}
```

> `appsettings.json` uses the **Docker service hostname** (`ambev.developerevaluation.nosql`) — this is what works inside the container network.  
> Override in `appsettings.Development.json` for local-without-Docker development:

```json
"MongoDB": {
  "ConnectionString": "mongodb://developer:ev%40luAt10n@localhost:27017",
  "DatabaseName": "developer_evaluation"
}
```

---

## DI Registration

```csharp
// In the infrastructure module initializer:
var mongoConnectionString = configuration["MongoDB:ConnectionString"];
var mongoDatabaseName = configuration["MongoDB:DatabaseName"];

services.AddSingleton<IMongoClient>(new MongoClient(mongoConnectionString));
services.AddScoped<IMongoDatabase>(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(mongoDatabaseName));
services.AddScoped<IMongoSaleRepository, MongoSaleRepository>();
```

---

## Notes

- `IMongoClient` should be singleton (connection-pooled by the driver).
- `IMongoDatabase` and the repository can be scoped.
- Ensure the MongoDB NuGet package is added to the IoC/WebApi projects if not already present.

---

## Definition of Done

- [ ] `IMongoClient`, `IMongoDatabase`, and `IMongoSaleRepository` registered
- [ ] Connection string in `appsettings.json` matches docker-compose credentials
- [ ] App starts without errors
- [ ] Reviewed by developer
- [ ] Committed with `feat(ioc): register MongoDB client and MongoSaleRepository in DI`
