# AgriLedger — Deployment Guide

## 1. Prerequisites

- .NET 8 SDK (build machine only — the deployed server just needs the ASP.NET
  Core 8 runtime)
- Node.js 18+ (build machine only — the deployed frontend is static files)
- A SQL Server instance reachable from the API (Azure SQL, an on-prem SQL
  Server, or SQL Server in a container)
- (Optional but recommended for production) Azure Blob Storage or AWS S3
  bucket for receipts, instead of local disk

## 2. Backend deployment

### Build

```bash
cd backend
dotnet publish src/AgriLedger.API -c Release -o ./publish
```

### Configure

Do **not** ship `appsettings.json` with real secrets. Set these as
environment variables (or Azure App Service "Application settings" / a
secrets manager) on the target server instead:

| Setting | Purpose |
|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string |
| `Jwt__Key` | Long, random secret (32+ chars) used to sign JWTs |
| `Jwt__Issuer` / `Jwt__Audience` | Should match what the frontend/API agree on |
| `Cors__AllowedOrigins__0` | Your deployed frontend's origin, e.g. `https://app.agriledger.example.com` |
| `FileStorage__RootPath` | Leave as-is only if you're keeping local disk storage (not recommended beyond a single-instance deployment) |

### Apply migrations against the production database

Run this once from a machine that can reach the production SQL Server (a
CI/CD job is the usual place for this):

```bash
dotnet ef database update -p src/AgriLedger.Infrastructure -s src/AgriLedger.API --connection "<production connection string>"
```

### Host it

Pick one:
- **Azure App Service (Linux, .NET 8)** — deploy the `publish/` folder via
  `az webapp deploy` or a GitHub Actions workflow
- **IIS on Windows Server** — install the ASP.NET Core Hosting Bundle, point
  an IIS site at `publish/`
- **Docker** — build from the official `mcr.microsoft.com/dotnet/aspnet:8.0`
  base image, `COPY` the publish output in, expose port 8080/443

### File storage in production

The default `FileStorageService` writes receipts to local disk
(`wwwroot/uploads`). That's fine for a single-instance demo, but it does
**not** survive container restarts or scale across multiple instances.
Before going to production, implement `IFileStorageService` against Azure
Blob Storage or AWS S3 (the interface already isolates this — nothing else
in the codebase needs to change) and register it in `Program.cs` in place of
`FileStorageService`.

## 3. Frontend deployment

### Build

```bash
cd frontend
npm install
REACT_APP_API_BASE_URL=https://api.agriledger.example.com/api npm run build
```

This produces a static `build/` folder.

### Host it

Any static host works:
- **Azure Static Web Apps** — `swa deploy ./build`
- **Netlify / Vercel** — connect the repo, set the build command to
  `npm run build` and the output directory to `build`
- **Nginx / any static file server / S3 + CloudFront** — copy `build/` up

Make sure the host serves `index.html` for all unknown routes (SPA
fallback), since this is a client-side-routed React app — otherwise a
refresh on `/expenses` will 404.

## 4. Post-deployment checklist

- [ ] HTTPS enforced end-to-end (backend already calls `UseHttpsRedirection()`)
- [ ] `Jwt:Key` is a real random secret, not the placeholder in `appsettings.json`
- [ ] CORS `AllowedOrigins` matches the deployed frontend's exact origin
- [ ] Database migrations applied (`dotnet ef database update`)
- [ ] File storage is durable (Blob/S3), not local disk, if running more than
      one API instance
- [ ] Rate limiting added in front of `/api/auth/login` and
      `/api/auth/forgot-password` (not included in this build — add
      `Microsoft.AspNetCore.RateLimiting` middleware or a WAF rule)
- [ ] Backups configured on the SQL Server database
- [ ] Application Insights / a log sink configured (Serilog is already
      writing to console + rolling files; add a cloud sink for production)
