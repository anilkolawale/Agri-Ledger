# AgriLedger — Smart Farm Expense & Farm Management System

A digital farm diary for small and medium-scale farmers: track farms, crops, daily
expenses, income, labor and inventory in one mobile-first web app.

## What's in this build

This is a **working Clean Architecture implementation** covering every core
module in the spec, built end-to-end (Domain → Application → Infrastructure →
API → React UI):

- Auth: Register, Login, Forgot/Reset Password, Change Password, Profile update, JWT
- Farm Management (CRUD)
- Crop Management (CRUD, status tracking)
- Expense Management (CRUD, categories incl. 12 seeded defaults + custom categories,
  search, sort, pagination)
- Income Management (CRUD, search, sort, pagination)
- Labor Management (CRUD, payment status tracking)
- Inventory (CRUD, low-stock flag against a reorder threshold, quantity adjust
  endpoint for "remaining quantity after usage")
- Receipts (camera/photo/bill upload via multipart form, preview/download/delete;
  local disk storage behind an `IFileStorageService` interface so Azure Blob/S3
  is a drop-in swap)
- Reports: expenses, income, crop-wise profit, category-wise expenses,
  farm-wise expense/income, labor payments, inventory, and profit & loss — each
  with Excel (ClosedXML) and PDF (QuestPDF) export
- Global Search across farms, crops, expense descriptions/categories, buyers, workers
- Dashboard summary (farms, today's/monthly expense, income, expense, net profit,
  crops ready for harvest, pending labor payments, low inventory, upcoming harvests)
- English / Marathi localization (i18next) with instant switching
- Mobile-first shell: bottom navigation, floating "Add Expense" button, a "More"
  menu (Labor/Inventory/Reports), global search dialog, MUI theme

### Not started (documented in the spec as "Future Enhancements")
PWA offline mode, weather integration, mandi price feed, AI recommendations/
categorization, crop disease detection, voice input, SMS/WhatsApp reminders,
GPS field mapping, barcode/QR inventory.

## Why nothing is compiled here

This build environment has no internet access and no .NET SDK, so the code was
authored by hand and could not be compiled or run in this sandbox. Follow the
setup steps below on your own machine.

## Project Structure

```
AgriLedger/
├── backend/
│   ├── AgriLedger.sln
│   └── src/
│       ├── AgriLedger.Domain/          # Entities, enums — no dependencies
│       ├── AgriLedger.Application/     # DTOs, interfaces, services, validators, AutoMapper
│       ├── AgriLedger.Infrastructure/  # EF Core DbContext, repositories, JWT, password hashing
│       └── AgriLedger.API/             # Controllers, Program.cs, Swagger, middleware
└── frontend/
    └── src/
        ├── api/            # Axios clients per module
        ├── context/        # AuthContext (JWT session)
        ├── components/     # Layout (bottom nav, FAB), shared widgets
        ├── pages/           # auth / dashboard / farms / crops / expenses / income
        ├── i18n/            # en.json, mr.json
        └── theme/           # MUI theme
```

## Backend Setup

Prerequisites: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0),
SQL Server (LocalDB, which ships with the .NET SDK/Visual Studio, works out
of the box — no separate SQL Server install needed), SSMS (optional, for
inspecting the DB).

```bash
cd backend

# 1. Restore all four projects
dotnet restore

# 2. (Optional) override the connection string / JWT key as user secrets
#    instead of editing appsettings.json directly. The default in
#    appsettings.json already points at LocalDB, so this step is optional
#    for local dev — only needed if you're using a different SQL Server.
cd src/AgriLedger.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=AgriLedgerDb;Integrated Security=True;TrustServerCertificate=True;"
dotnet user-secrets set "Jwt:Key" "<generate a random 32+ character secret>"

# 3. Install the EF Core CLI tool (once, globally)
dotnet tool install --global dotnet-ef

# 4. Create the initial migration and apply it
cd ../..                      # back to backend/
dotnet ef migrations add InitialCreate -p src/AgriLedger.Infrastructure -s src/AgriLedger.API
dotnet ef database update -p src/AgriLedger.Infrastructure -s src/AgriLedger.API

# 5. Run the API (uses the "http" launch profile by default — plain HTTP on
#    port 5000, no certificate trust step needed for local dev)
dotnet run --project src/AgriLedger.API
# Swagger UI: http://localhost:5000/swagger
# Want HTTPS instead? dotnet run --project src/AgriLedger.API --launch-profile https
# (then run `dotnet dev-certs https --trust` once, and point the frontend at
# https://localhost:7001/api instead)
```

### Optional: seed sample/demo data

To explore the app with realistic data instead of an empty account, enable
the demo seeder for one run:

```bash
dotnet run --project src/AgriLedger.API -- --Seed:Demo=true
```

This creates one demo user (`demo@agriledger.test` / `Demo@12345`) with two
farms, two crops, several expenses, an income sale, a labor entry, and one
inventory item — enough to see the dashboard, reports, and search populated.
It's idempotent (checks if the demo user already exists) and does nothing
unless `Seed:Demo` is explicitly set to `true`.

## Frontend Setup

Prerequisites: Node.js 18+.

```bash
cd frontend
npm install
cp .env.example .env
# edit .env: REACT_APP_API_BASE_URL should match the backend's Swagger URL + /api
npm start
# opens http://localhost:3000
```

Make sure `Cors:AllowedOrigins` in `backend/src/AgriLedger.API/appsettings.json`
includes your frontend origin (defaults to `http://localhost:3000`).

## Extending the app

Every module in this codebase follows the same five-step pattern — use
`LaborController`/`LaborService`/`LaborListPage` (the smallest complete module)
as your template for anything new (e.g. a Government Schemes module):

1. **Domain** — add the entity under `Domain/Entities/`
2. **Application** — add DTOs under `DTOs/<Module>/`, add the service interface
   to `Interfaces/IServices.cs` (or `IExtendedServices.cs`), implement the
   service under `Services/`, add an AutoMapper mapping and a FluentValidation
   validator
3. **Infrastructure** — expose the repository via `IUnitOfWork`/`UnitOfWork`,
   add an `IEntityTypeConfiguration<T>` for any extra constraints/indexes
4. **API** — register the service in `Program.cs`, add a controller following
   `LaborController.cs` or `ExpensesController.cs` as a template
5. **Frontend** — add an `xxxApi.js` client, list/form pages, and a route in
   `App.js` + nav entry in `AppLayout.js`

## Security notes for production

- Replace the placeholder `Jwt:Key` in appsettings with a securely generated
  secret (never commit real secrets — use user secrets / Key Vault / env vars)
- Serve behind HTTPS only; `UseHttpsRedirection()` is already enabled
- File uploads should validate content-type + magic bytes, not just extension,
  before accepting receipts
- Enable rate limiting on `/auth/login` and `/auth/forgot-password`
- SQL injection is mitigated by EF Core parameterization; avoid raw SQL
- XSS is mitigated by React's default escaping; avoid `dangerouslySetInnerHTML`

## Changelog

**Fixed in this revision** (backend wouldn't compile before this — if you hit
errors around the Expenses/Receipts code, this is almost certainly why):

1. **`AgriLedger.API.csproj` referenced no FluentValidation package at all**,
   yet `Program.cs` called `AddFluentValidationAutoValidation()` — a method
   that doesn't exist without it. Fixed by adding
   `SharpGrip.FluentValidation.AutoValidation.Mvc` (the correct, current
   package for FluentValidation + ASP.NET Core auto-validation on .NET 8;
   the older `FluentValidation.AspNetCore` package is deprecated and exposes
   a different method name) and updating the `using` in `Program.cs`.
2. **`AgriLedger.Application` and `AgriLedger.Infrastructure` referenced
   `Microsoft.AspNetCore.Http.Abstractions` v2.2.0** to get `IFormFile` (used
   by the Receipts upload feature, wired into the Expense/Income forms) and
   `IHttpContextAccessor`. That package predates .NET Core 3 and doesn't
   belong in a net8.0 class library — it either fails to resolve or produces
   type conflicts against the real ASP.NET Core types the API project already
   has. Fixed by replacing it with `<FrameworkReference
   Include="Microsoft.AspNetCore.App" />` in both projects, which is the
   supported way for a plain class library to use ASP.NET Core types.
3. **The Expense "time" field used `TimeSpan?` in the DTOs**, but an HTML
   `<input type="time">` sends `"14:30"`, and .NET's built-in JSON `TimeSpan`
   converter requires the full `"14:30:00"` format — so saving an expense
   with a time set would throw a deserialization error. Fixed by changing
   `ExpenseTime` to a plain `string?` ("HH:mm") throughout the DTOs, with
   explicit parsing/formatting in `MappingProfile.cs` and
   `ExpenseService.UpdateAsync`.

If you still see a build error after pulling this version, please paste the
exact `dotnet build` output (as text, not an archive — I can't open `.7z`
files in this environment, only `.zip`) and I'll fix it directly.

**Also fixed:** no `launchSettings.json` shipped originally, so `dotnet run`
fell back to .NET's bare defaults — `http://localhost:5000` in environment
`Production` — while the frontend defaulted to `https://localhost:7001`,
causing `ERR_CONNECTION_REFUSED`. Added `Properties/launchSettings.json`
with an `http` profile fixed to port 5000 and
`ASPNETCORE_ENVIRONMENT=Development` (so Swagger and detailed errors work
without editing `Program.cs`), and updated the frontend's default
`REACT_APP_API_BASE_URL` to match. HTTPS is still available via
`--launch-profile https` if you prefer it. Also switched the default
connection string to LocalDB, since that ships with the .NET SDK and needs
no separate SQL Server install.

*(The `The WebRootPath was not found: ...\wwwroot` warning in the startup
log is harmless — that folder is only needed for receipt uploads and gets
created automatically on the first upload.)*

## Troubleshooting

**Browser console shows `ERR_CONNECTION_REFUSED` calling the API**
Almost always a port mismatch — the frontend is calling a port nothing is
listening on. This build's default `dotnet run --project src/AgriLedger.API`
uses the `http` launch profile: plain HTTP on **port 5000**, environment
`Development` (no certificate trust step needed). The frontend's default
`REACT_APP_API_BASE_URL` already matches (`http://localhost:5000/api`).
- Check the actual `dotnet run` console output — it prints `Now listening
  on: http://localhost:5000`. If you see a *different* port, either that
  line or your `frontend/.env` needs to change to match it exactly.
- If the console instead says `Hosting environment: Production` with no
  `launchSettings.json`-driven port, you're running the compiled `.exe`
  directly (or an IDE run configuration that isn't applying the profile)
  rather than `dotnet run` from `src/AgriLedger.API/` — `cd` into that
  folder (or pass `--project`) so the CLI picks up
  `Properties/launchSettings.json`.
- Restart the frontend (`npm start`) after changing `.env` — Create React
  App only reads `.env` at startup.

**Want HTTPS instead of HTTP?**
```bash
dotnet run --project src/AgriLedger.API --launch-profile https
dotnet dev-certs https --trust   # once, so the browser accepts the local cert
```
Then set `frontend/.env`'s `REACT_APP_API_BASE_URL=https://localhost:7001/api`
and restart `npm start`. (On Linux, `--trust` doesn't work out of the box —
trust the cert via your browser/OS certificate store instead, or just stick
with the HTTP profile for local dev.)

**CORS error in the console (`No 'Access-Control-Allow-Origin' header`)**
`Cors:AllowedOrigins` in `appsettings.json` must exactly match the
frontend's origin (scheme + host + port) — `http://localhost:3000` by
default. Update it and restart the API if you're running the frontend
elsewhere.

## Deployment (high-level)

See [`docs/DEPLOYMENT.md`](docs/DEPLOYMENT.md) for the full deployment guide
(build steps, environment configuration, hosting options, and a
post-deployment checklist). Summary:

- **Backend**: publish with `dotnet publish -c Release`, deploy to Azure App
  Service / IIS / a Linux container; point `DefaultConnection` at a managed
  SQL Server (Azure SQL) instance
- **Frontend**: `npm run build`, deploy the static `build/` folder to Azure
  Static Web Apps / Netlify / Vercel / an Nginx container; set
  `REACT_APP_API_BASE_URL` to the deployed API's URL at build time
- **File storage**: swap the local `wwwroot/uploads` static file store for
  Azure Blob Storage or AWS S3 once you add the Receipts module (interface
  already anticipated in `Domain/Entities/Receipt.cs`)
#   A g r i - L e d g e r  
 