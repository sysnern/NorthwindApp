# 🏢 NorthwindApp - Modern Database Management System

A comprehensive .NET 9.0 application for managing the Northwind database with a modern React frontend.

## 🚀 Features

### **Backend (.NET 9.0)**
- ✅ **Three-Layer Architecture** (API, Business, Data)
- ✅ **Repository Pattern** with Generic Repository
- ✅ **Service Layer** with Business Logic Validation
- ✅ **Entity Framework Core** with Code-First approach
- ✅ **AutoMapper** for DTO mapping
- ✅ **FluentValidation** for input validation
- ✅ **Caching** with Memory Cache
- ✅ **Logging** with Serilog
- ✅ **Error Handling** with Global Exception Middleware
- ✅ **Soft Delete** functionality
- ✅ **Pagination** and **Sorting**
- ✅ **Filtering** with dynamic expressions

### **Frontend (React 19)**
- ✅ **Modern React** with Hooks
- ✅ **Responsive Design** with Bootstrap
- ✅ **Form Management** with Formik + Yup
- ✅ **State Management** with React Context
- ✅ **Error Boundaries** and Loading States
- ✅ **Toast Notifications**
- ✅ **CRUD Operations** for all entities
- ✅ **Advanced Filtering** and Search
- ✅ **Sorting** and **Pagination**

## 🏗️ Architecture

```
NorthwindApp/
├── NorthwindApp.API/          # Presentation Layer
│   ├── Controllers/           # REST API Controllers
│   ├── Middleware/           # Global Exception Handling
│   └── Extensions/           # Service Configuration
├── NorthwindApp.Business/     # Business Layer
│   ├── Services/             # Business Logic Services
│   ├── Validation/           # FluentValidation Rules
│   └── Mapping/              # AutoMapper Profiles
├── NorthwindApp.Data/         # Data Layer
│   ├── Context/              # Entity Framework Context
│   ├── Repositories/         # Repository Pattern
│   └── Extensions/           # Data Configuration
├── NorthwindApp.Core/         # Shared DTOs
├── NorthwindApp.Entities/     # Domain Models
└── NorthwindAppFrontend/      # React Frontend
```

## 🛠️ Technologies

### **Backend**
- **.NET 9.0** - Latest .NET framework
- **Entity Framework Core** - ORM
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Serilog** - Structured logging
- **Memory Cache** - Caching

### **Frontend**
- **React 19** - Modern React
- **React Bootstrap** - UI components
- **Formik + Yup** - Form management
- **Axios** - HTTP client
- **React Router** - Client-side routing

## 🚀 Quick Start

### **Prerequisites**
- .NET 9.0 SDK
- Node.js 16+
- SQL Server / SQLite

### **Backend Setup**
```bash
# Clone repository
git clone https://github.com/yourusername/NorthwindApp.git
cd NorthwindApp

# Restore packages
dotnet restore

# Update database
dotnet ef database update

# Run API
dotnet run --project NorthwindApp.API
```

### **Frontend Setup**
```bash
# Navigate to frontend
cd NorthwindAppFrontend

# Install dependencies
npm install

# Start development server
npm start
```

## 📊 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Product/list` | Get all products with filtering |
| POST | `/api/Product/create` | Create new product |
| PUT | `/api/Product/update` | Update existing product |
| DELETE | `/api/Product/delete/{id}` | Delete product (soft delete) |

Similar endpoints available for: Categories, Customers, Suppliers, Employees, Orders

## 🔧 Configuration

### **Database Connection**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Northwind;Trusted_Connection=true;"
  }
}
```

### **Caching**
```csharp
// Memory cache configuration
services.AddMemoryCache();
services.AddScoped<ICacheService, CacheService>();
```

### **Logging**
```csharp
// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

## 🎯 Key Features

### **Generic Service Pattern**
```csharp
// Eliminates 90% code duplication
public abstract class GenericService<TEntity, TDto, TCreateDto, TUpdateDto, TKey>
{
    // Common CRUD operations
    // Business rule validation
    // Caching mechanism
    // Error handling
}
```

### **Business Rule Validation**
```csharp
protected override async Task<BusinessValidationResult> ValidateBusinessRulesForCreate(TCreateDto dto)
{
    // Custom business logic validation
    return BusinessValidationResult.Success();
}
```

### **Repository Pattern**
```csharp
public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
{
    // Standard data access operations
    // Soft delete support
    // Async/await pattern
}
```

## 📈 Performance Optimizations

- ✅ **Caching** - Memory cache for frequently accessed data
- ✅ **Async/Await** - Non-blocking operations
- ✅ **Lazy Loading** - Entity Framework lazy loading
- ✅ **Pagination** - Server-side pagination
- ✅ **Filtering** - Dynamic expression-based filtering

## 🔒 Security Features

- ✅ **Input Validation** - FluentValidation rules
- ✅ **SQL Injection Protection** - Entity Framework parameterized queries
- ✅ **Error Handling** - Global exception middleware
- ✅ **Soft Delete** - Data integrity protection

## 🧪 Testing

```bash
# Run backend tests
dotnet test

# Run frontend tests
npm test
```

## 📝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add amazing feature'`)
4. Push to branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Support

For support, email support@northwindapp.com or create an issue on GitHub.

---

**Built with ❤️ using .NET 9.0 and React 19**
