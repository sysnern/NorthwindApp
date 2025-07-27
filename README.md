# NorthwindApp Web API

> 🚀 A .NET 9 Web API for the classic Northwind sample – with clean architecture, validation, AutoMapper, uniform responses, logging & caching.

---

## Table of Contents

- [About](#about)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Migrations](#database-migrations)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Error Handling](#error-handling)
- [Logging](#logging)
- [Caching](#caching)
- [Tests](#tests)
- [Contributing](#contributing)
- [License](#license)

---

## About

This project implements the backend for NorthwindApp – an example e‑commerce system. It follows a clean architecture, uses EF Core for data access, FluentValidation for input validation, AutoMapper for DTO mappings, and wraps every response in a generic `ApiResponse<T>`.

---

## Tech Stack

- **Language & Framework:** .NET 9  
- **ORM:** Entity Framework Core  
- **Validation:** FluentValidation  
- **Mapping:** AutoMapper  
- **Database:** Microsoft SQL Server  
- **Logging:** Serilog  
- **Caching:** In-Memory Cache  
- **Exception Handling:** Global Exception Middleware  
- **API Docs:** Swagger / Swashbuckle  

---

## Features

- ✅ CRUD for Products, Categories, Customers, Suppliers, Employees, Orders  
- ✅ Soft-delete support  
- ✅ Filtering & pagination on list endpoints  
- ✅ FluentValidation rules  
- ✅ AutoMapper profiles  
- ✅ Uniform `ApiResponse<T>` envelope  
- ✅ Structured logging with Serilog  
- ✅ Response caching on GET list endpoints  
- ✅ Global error handling middleware  

---

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server)  
- (Optional) Visual Studio 2022 or VS Code  

---

## Installation

```bash
git clone https://github.com/sysnern/NorthwindApp.git
cd NorthwindApp/Backend
dotnet restore
```

---

## Configuration

1. Copy `appsettings.json.example` ➡️ `appsettings.json`  
2. Update your **connection string**:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=NorthwindDb;Trusted_Connection=True;"
     }
   }
   ```
---

## Database Migrations

Apply EF Core migrations to create the schema:

```bash
cd NorthwindApp.Infrastructure
dotnet ef database update
```
## Running the Application

```bash
cd NorthwindApp.Api
dotnet run
```
- API base URL: https://localhost:7137

- Swagger UI: https://localhost:7137/swagger

## API Endpoints

All routes are prefixed with `/api`.

### Products
| Method | URL                   | Description                          |
| ------ | --------------------- | ------------------------------------ |
| GET    | `/Product/list`       | List all products (filtered, paged)  |
| GET    | `/Product/{id}`       | Get product by ID                    |
| POST   | `/Product`            | Create a new product                 |
| PUT    | `/Product`            | Update an existing product           |
| DELETE | `/Product/{id}`       | Soft‑delete a product                |

### Categories / Customers / Suppliers / Employees / Orders  
> Same CRUD contract as Products:  
> `GET /{Entity}/list`  
> `GET /{Entity}/{id}`  
> `POST /{Entity}`  
> `PUT /{Entity}`  
> `DELETE /{Entity}/{id}`

## Error Handling

All responses come in a consistent shape:

```jsonc
{
  "success": true|false,
  "message": "Optional user message or error description",
  "data": { /* T or null */ }
}
```
Unhandled exceptions are caught by global middleware and returned as HTTP 400 or HTTP 500 with success=false.

## Logging

Serilog is configured to write to:
- Console  
- Rolling files in `logs/log-.txt`
    
## Caching

The GET‑list endpoints (e.g. `/Product/list`) use in‑memory caching for **60 seconds** to reduce database load.
Configuration settings live under the `Serilog` section in `appsettings.json`.

## Tests

Unit tests live in the `NorthwindApp.Tests` project. To run:

```bash
cd NorthwindApp.Tests
dotnet test
```
## Contributing

1. Fork the repository  
2. Create a feature branch
   ```bash
   git checkout -b feature/YourFeature
   ``` 
3. Commit your changes
   ```bash
   git commit -m "Add your feature description"
   ```
4. Push to your branch
   ```bash
   git push origin feature/YourFeature
   ```
5. Open a Pull Request on GitHub  

## License

This project is licensed under the [MIT License](LICENSE).
