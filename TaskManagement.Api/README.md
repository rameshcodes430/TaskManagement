# TaskManagement.Api

A clean-architecture **Task Management API** built with ASP.NET Core and SQL Server.  
This API provides endpoints to create, update, assign, and track tasks.

---

## 🚀 Features
- RESTful API with ASP.NET Core
- Entity Framework Core for database access
- CRUD operations for task management
- DTOs for clean request/response models
- Service layer for business logic
- Middleware for centralized error handling
- Swagger/OpenAPI support for easy testing

---

## 🛠️ Tech Stack
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Database:** SQL Server (with EF Core Migrations)
- **ORM:** Entity Framework Core
- **API Testing:** Swagger / `.http` file support in IDE
- **Version Control:** Git + GitHub

---

## 📂 Project Structure
TaskManagement.Api/
├── Controllers/ # API endpoints (e.g., TaskItemsController.cs)
├── Data/ # AppDbContext, EF configurations
├── Dtos/ # Request/Response DTOs
├── Middleware/ # Error handling, custom middleware
├── Migrations/ # EF Core migrations
├── Models/ # Entity classes (e.g., TaskItem.cs)
├── Services/ # Business logic layer (TaskService.cs, interfaces)
├── Properties/ # Launch settings
├── appsettings.json # Configuration (DB connection, logging, etc.)
├── Program.cs # App startup and service configuration
├── TaskManagement.Api.http # Quick API test file
└── .gitignore # Ignore build artifacts