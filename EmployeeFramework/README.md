# Employee Management API - .NET Framework

A RESTful Web API built with .NET Framework 4.7.2 following Clean Architecture principles for managing employee data. The API uses `HttpListener` for self-hosting and includes **Swagger UI documentation**.

## ??? Architecture Overview

Clean Architecture implementation with clear separation of concerns:

1. **Domain** - Core business entities and repository interfaces
2. **Application** - Business logic, services, and DTOs
3. **Infrastructure** - Data access implementation
4. **Documentation** - API documentation generator
5. **Presentation** - HTTP API handling

## ?? Features

- ? Complete CRUD operations for employee management
- ? Input validation and error handling
- ? CORS support for web clients
- ? Pagination support
- ? **Swagger UI documentation**
- ? **OpenAPI 3.0.1 specification**
- ? **Interactive API testing**

## ?? Employee Data Model

- **ID** (Guid) - Unique identifier (auto-generated)
- **Name** (string) - Employee's full name *(required)*
- **Address** (string) - Employee's address *(required)*
- **Age** (int) - Employee's age *(1-120)*
- **Salary** (decimal) - Employee's salary *(>= 0)*
- **CreatedAt** (DateTime) - Creation timestamp
- **UpdatedAt** (DateTime?) - Last update timestamp

## ??? Technical Stack

- **.NET Framework 4.7.2** - Target framework
- **HttpListener** - Self-hosted HTTP server
- **Custom JSON Serializer** - No external dependencies
- **In-Memory Repository** - Easily replaceable with Entity Framework
- **Clean Architecture** - Architectural pattern
- **Swagger UI** - Interactive API documentation
- **OpenAPI 3.0.1** - API specification standard

## ?? API Endpoints & Documentation

### **?? Interactive Documentation**
- **Swagger UI**: `http://localhost:8080/swagger` - Full interactive documentation
- **OpenAPI Spec**: `http://localhost:8080/swagger.json` - Raw OpenAPI specification
- **Welcome Page**: `http://localhost:8080/` - API overview

### **?? REST Endpoints**

| Method | Endpoint | Description | Query Parameters |
|--------|----------|-------------|-----------------|
| GET | `/api/employees` | Get all employees | `?page=1&pageSize=10` |
| GET | `/api/employees/{id}` | Get employee by ID | - |
| POST | `/api/employees` | Create new employee | - |
| PUT | `/api/employees/{id}` | Update employee | - |
| DELETE | `/api/employees/{id}` | Delete employee | - |

## ?? Getting Started

### Prerequisites
- .NET Framework 4.7.2 or higher
- Visual Studio 2017 or higher
- **Administrator privileges** (required for HttpListener)

### Installation

1. **Build the solution**
   ```bash
   dotnet build
   ```

2. **Run as Administrator**
   - Right-click on the executable or Visual Studio
   - Select "Run as Administrator"

### Usage

The API starts on `http://localhost:8080` by default.

Console output:
```
Employee Management API running at http://localhost:8080
Swagger UI: http://localhost:8080/swagger
Press CTRL+C to stop...
```

## ?? **How to Access the API**

### **Option 1: Swagger UI (Recommended)**
1. Navigate to: `http://localhost:8080/swagger`
2. Interactive documentation with try-it-out functionality

### **Option 2: Direct API Calls**

**Create employee:**
```http
POST http://localhost:8080/api/employees
Content-Type: application/json

{
  "name": "John Doe",
  "address": "123 Main St, New York, NY",
  "age": 30,
  "salary": 75000
}
```

**Get all employees:**
```http
GET http://localhost:8080/api/employees
```

**Get employee by ID:**
```http
GET http://localhost:8080/api/employees/{guid-id}
```

**Update employee:**
```http
PUT http://localhost:8080/api/employees/{guid-id}
Content-Type: application/json

{
  "name": "John Doe Updated",
  "address": "123 Main St, New York, NY",
  "age": 31,
  "salary": 80000
}
```

**Delete employee:**
```http
DELETE http://localhost:8080/api/employees/{guid-id}
```

## ??? Project Structure

```
EmployeeFramework/
??? Domain/
?   ??? Entities/Employee.cs
?   ??? Repositories/IEmployeeRepository.cs
??? Application/
?   ??? DTOs/EmployeeDto.cs
?   ??? Services/
?       ??? IEmployeeService.cs
?       ??? EmployeeService.cs
??? Infrastructure/
?   ??? Repositories/InMemoryEmployeeRepository.cs
??? Documentation/
?   ??? SwaggerDocumentGenerator.cs
??? Program.cs
??? SimpleJsonSerializer.cs
??? EmployeeApi.http
```

## ?? Development Notes

### Data Storage
Uses in-memory repository. To add database persistence:

1. Install Entity Framework
2. Create DbContext class
3. Implement `IEmployeeRepository` with Entity Framework
4. Update Program.cs to use the new repository

### Validation Rules
- Employee name and address are required
- Age must be between 1 and 120
- Salary cannot be negative
- Validation performed in service layer

### CORS Support
- Enabled for all origins
- Supports GET, POST, PUT, DELETE, OPTIONS methods
- Allows Content-Type header

### Swagger Documentation
- **OpenAPI 3.0.1** compliant specification
- **Interactive Swagger UI** with try-it-out functionality
- Complete schema definitions
- **No external dependencies**

## ?? Important Notes

1. **Administrator Privileges**: Required for HttpListener on Windows
2. **Firewall**: Windows may prompt to allow the application
3. **Port Configuration**: Default port is 8080
4. **Production Use**: Consider ASP.NET Web API with IIS for production

## ?? Testing

### **Best Option: Swagger UI**
1. Run the application
2. Open `http://localhost:8080/swagger`
3. Use the interactive interface to test all endpoints

### **Alternative Options:**
- Use the included `EmployeeApi.http` file
- Use Postman or any HTTP client

## ?? **Production Ready Features**

? **Complete Employee Management API**  
? **Interactive Swagger Documentation**  
? **Clean Architecture Implementation**  
? **Real-time API Testing**  
? **Professional API Documentation**  
? **No External Dependencies**  
? **Self-Hosted Solution**  
? **Ready for Extension**