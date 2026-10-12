# ProductsApi - Plantilla .NET 8 por capas (simplificada)

Flujo: Controller -> IProductService -> AppDbContext (SQL Server)

## Primeros pasos
Add-Migration InitialCreate
Update-Database
(o con CLI: dotnet ef migrations add InitialCreate / dotnet ef database update)

Swagger: https://localhost:<puerto>/swagger

## Para agregar otra entidad
1. Models/  2. DTOs/  3. Services/ (interfaz + impl)
4. Controllers/  5. Registrar en Program.cs y DbSet en AppDbContext
