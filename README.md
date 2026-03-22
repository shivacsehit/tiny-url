# 🔗 Tiny URL

A full-stack URL shortener built with Angular + Blazor + ASP.NET Core.


---

## 🏗️ Architecture
```
Frontend (Angular / Blazor) → Backend (ASP.NET Core Minimal API) → Azure SQL Database
                                          ↑
                               Azure Function (Cron - clears URLs every hour)
```

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Frontend 1 | Angular 17 (Standalone) |
| Frontend 2 | Blazor WebAssembly |
| Backend | ASP.NET Core 8 Minimal API |
| Database | Azure SQL / Local SQL Server |
| Logging | Serilog → File |

---

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org)
- [Angular CLI](https://angular.io/cli) — `npm install -g @angular/cli`
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [Visual Studio 2026](https://visualstudio.microsoft.com)

---

## 🚀 Run Locally
## ⚙️ Connection String

For SQL Express users:
```json
"Default": "Server=localhost\\SQLEXPRESS;Database=TinyUrlDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

For default SQL Server:
```json
"Default": "Server=.;Database=TinyUrlDb;Trusted_Connection=True;TrustServerCertificate=True;"
```
### 1. Clone the repo
```bash
git clone https://github.com/shivacsehit/tiny-url.git
cd tiny-url
```

### 2. Setup Database
- Open SQL Server Management Studio
- Connect with Windows Authentication
- Database `TinyUrlDb` will be created automatically on first run

### 3. Run Backend API
```bash
cd TinyUrl.API
dotnet restore
dotnet ef database update
dotnet run
# Runs on http://localhost:5000
# Swagger UI: http://localhost:5000/swagger
```

### 4. Run Angular Frontend
```bash
cd TinyUrl.Web/tinyurl.web.client
npm install
npm start
# Runs on https://localhost:XXXX
```

### 5. Run Blazor Frontend
```bash
cd TinyUrl.Blazor
dotnet run
# Runs on https://localhost:XXXX
```

### 6. Run All Together (Visual Studio)
- Open `TinyUrl.sln` in Visual Studio 2022
- Right-click Solution → Properties → Multiple Startup Projects
- Set startup projects:

| Project | Action |
|---|---|
| `TinyUrl.API` | Start |
| `TinyUrl.Web.Server` | Start (for Angular) |
| `TinyUrl.Blazor` | Start (for Blazor) |

- Press **F5**

---

## 📡 API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/add` | Create short URL |
| GET | `/api/public` | List all public URLs |
| GET | `/{code}` | Redirect to original URL |
| DELETE | `/api/delete/{code}` | Delete a short URL |
| DELETE | `/api/delete-all` | Delete all URLs |
| PUT | `/api/update/{code}` | Update a short URL |

### Example Request — Create Short URL
```json
POST http://localhost:5000/api/add
{
  "url": "https://google.com",
  "isPrivate": false
}
```

### Example Response
```json
{
  "shortUrl": "http://localhost:5000/abc123",
  "code": "abc123"
}
```

---


## 📁 Project Structure