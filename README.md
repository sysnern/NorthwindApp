# NorthwindApp Web API

> 🚀 A .NET 7 Web API for the classic Northwind sample – with clean architecture, validation, AutoMapper, uniform responses, logging & caching.

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

- **Language & Framework:** .NET 7  
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

- [.NET 7 SDK](https://dotnet.microsoft.com/download)  
- [SQL Server 2019+](https://www.microsoft.com/en-us/sql-server)  
- (Optional) Visual Studio 2022 or VS Code  

---

## Installation

```bash
git clone https://github.com/sysnern/NorthwindApp.git
cd NorthwindApp/Backend
dotnet restore

