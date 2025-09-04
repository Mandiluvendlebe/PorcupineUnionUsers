# Porcupine Union – Users Management (Fixed)

This package contains:
- ASP.NET Core (.NET 8) Web API in `PU.Users.Api/` with EF Core, code-first schema
- Minimal UI in `PU.Users.Api/wwwroot/index.html`
- xUnit tests (unit + integration) in `tests/PU.Users.Tests/`
- Top-level solution: `PU.Users.sln`

## Run (Dev)
```bash
cd PU.Users.Api
dotnet restore
dotnet ef database update
dotnet run
```

Open `http://localhost:5250/` (UI) or `/swagger` for API docs.

> First time only, create a migration if none exists:
> ```bash
> dotnet tool install --global dotnet-ef
> dotnet ef migrations add InitialCreate
> dotnet ef database update
> ```

## Tests
```bash
dotnet test
```

## Notes
- Fixed UI "Edit" button: uses event delegation with `closest('button')` and `classList.contains`.
- Includes seed data for Groups, Permissions and a few Users (see `Data/SeedData.cs`).
