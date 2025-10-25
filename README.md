# Application

This repository contains a sample Event Management System with a .NET 9 backend, EF Core-based persistence, and an Angular 19 frontend.

## Manual Start (without Docker)
If you prefer running the app locally without Docker:
1. Configure the backend
   - Rename `appsettings.json.example` → `appsettings.json`
   - Update the database connection string and any required secrets.
2. Run the backend
   ```powershell
   cd Backend/Event_Management_System
   dotnet restore
   dotnet run
   ```
3. Run the frontend
 ```powershell
cd ../../Frontend
npm install
ng serve
```
## Quickstart (Docker)

Prerequisites:
- Docker & Docker Compose installed on your machine

1. Copy the example environment file and set your secrets:

	Copy `.env.example` to `.env` and update values (do NOT commit `.env` with secrets):

	```powershell
	Copy-Item .env.example .env
	# then edit .env and replace 'change' and 'replace_with_a_strong_secret'
	```

2. Build and start all services using Docker Compose:

	```powershell
	Set-Location -LiteralPath 'E:\Event Management System'
	docker-compose up --build
	```

	Services started by compose:
	- Postgres database on host port 5432 (service name `db`)
	- Backend API on http://localhost:5107 (service name `backend`)
	- Frontend served by nginx on http://localhost:4200 (service name `frontend`)
	- `migrator` one-shot service applies EF Core migrations on startup

3. Open the frontend in a browser: http://localhost:4200

## Run migrations manually (optional)
If `migrator` did not apply migrations automatically, you can run them from a .NET SDK container or locally with the .NET SDK installed.

From host (requires dotnet SDK):

```powershell
Set-Location -LiteralPath 'E:\Event Management System\Backend\Event_Management_System'
dotnet restore
dotnet ef database update --project Infrastructure/Infrastructure.csproj --startup-project Event_Management_System/Event_Management_System.csproj
```


## Environment variables
The repository includes `.env.example` listing variables required for local Docker runs. Copy to `.env` and set real secrets there. Example variables:

- `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_DB`
- `CONNECTION_STRING` (used by backend)
- `JWT_SECRET`

