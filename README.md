# Banking Control Panel

## Setup Guide

### 1. Clone the Repository
### 2. Set Up the Database Connection String
### 3. Add and Apply Migrations

## Project Structure

- **API Project:**
  - Contains controllers, middleware, and configurations.
  
- **Core Project:**
  - Contains the core business logic, models, DTOs, services, and interfaces.
  
- **Infrastructure Project:**
  - Handles data access, repository implementations, and database context.

## Services

- **AuthService:**
  - Manages authentication, including login, registration, and JWT token generation.
  - Provides methods like `LoginAsync`, `RegisterUserAsync`, `GetCurrentUserAsync`, and others.
  
- **ClientsService:**
  - Handles client management, including adding, updating, deleting, and retrieving clients.
  - Implements methods like `CreateClientAsync`, `UpdateClientAsync`, `DeleteClientAsync`, `GetClientByIdAsync`, and `ListClientsAsync`.
  
- **AdminService:**
  - Manages admin-specific functionalities, including saving and retrieving search parameters.
  
- **ClientsRepository:**
  - Handles direct interactions with the database for client-related operations.
  - Provides methods like `AddAsync`, `UpdateAsync`, `DeleteAsync`, `GetClientsAsync`, and `GetTotalClientsAsync`.
  
- **SearchParametersRepository:**
  - Manages persistence and retrieval of search parameters used by admins.

## Controllers

- **AuthController:**
  - Endpoints for user registration, login, role management, and user management.
  - Examples: `Register`, `Login`, `GetUsers`, `DeleteUser`, `AddRole`, `UpdateRole`.
  
- **ClientsController:**
  - Manages CRUD operations for clients.
  - Examples: `CreateClient`, `GetClientById`, `ListClients`, `UpdateClient`, `DeleteClient`.

## Endpoints

- **AuthController Endpoints:**
  - `POST /api/v1/auth/register`: Register a new user (admin and general).
  - `POST /api/v1/auth/login`: Login and obtain a JWT token.
  - `GET /api/v1/auth/users`: Retrieve all users (admin only).
  - `DELETE /api/v1/auth/users/{userId}`: Delete a user (admin only).
  - `GET /api/v1/auth/roles`: Retrieve all roles (admin only).
  - `POST /api/v1/auth/roles`: Add a new role (admin only).
  - `PUT /api/v1/auth/roles/{roleId}`: Update an existing role (admin only).
  - `DELETE /api/v1/auth/roles/{roleId}`: Delete a role (admin only).
  
- **ClientsController Endpoints:**
  - `POST /api/v1/clients`: Create a new client.
  - `GET /api/v1/clients/{clientId}`: Retrieve a specific client by ID.
  - `GET /api/v1/clients`: List clients with filtering, sorting, and pagination.
  - `PUT /api/v1/clients/{clientId}`: Update an existing client.
  - `DELETE /api/v1/clients/{clientId}`: Delete a client.
  - `GET /api/v1/clients/searches`: Retrieve the last 3 search parameters used by the admin.

## Packages

- **Authentication:**
  - `Microsoft.AspNetCore.Authentication.JwtBearer`: For JWT-based authentication.
  
- **Validation:**
  - `libphonenumber-csharp`: For validating phone numbers.
  - Custom `PhoneNumberAttribute` class: For custom phone number validation.
  
- **Swagger:**
  - `Swashbuckle.AspNetCore`: For generating Swagger documentation.
  - `Swashbuckle.AspNetCore.Annotations`: For adding annotations to the Swagger documentation.
  
- **Entity Framework Core:**
  - `Microsoft.EntityFrameworkCore`: For database context and data access.
  - `Microsoft.EntityFrameworkCore.SqlServer`: For SQL Server database provider.
  - `Microsoft.EntityFrameworkCore.Tools`: For EF Core tools and migrations.

## Features Added

- **Global Error Handling:**
  - Implemented middleware for global error handling to return consistent JSON error responses.
  
- **Role-Based Access Control:**
  - Added role management and enforced role-based access control in controllers.
  
- **JWT Authentication:**
  - Implemented JWT token generation and validation for secured endpoints.
  
- **API Versioning:**
  - Configured API versioning to allow different versions of endpoints.
  
- **Swagger Documentation:**
  - Added detailed Swagger documentation for all endpoints.
  
- **Model Validations:**
  - Added extensive model validations using data annotations for DTOs.
  
- **Pagination and Sorting:**
  - Implemented pagination and sorting features for listing clients.
