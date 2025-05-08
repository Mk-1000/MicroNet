# MicroNetHub - AccountService

AccountService is a .NET Core microservice for handling user account management, authentication, and security monitoring as part of the MicroNetHub ecosystem.

## Features

- **Complete Account Management**: Create, read, update, and delete user accounts
- **Authentication**: JWT-based authentication with refresh tokens
- **OAuth Integration**: Google authentication support
- **Security Monitoring**: Track login attempts, password changes, and other security events
- **Access Logging**: Record user access details (IP address, device, location)
- **Role-based Authorization**: Admin and User role differentiation
- **API Documentation**: Swagger integration for easy API exploration
- **Health Checks**: Database connectivity monitoring

## Technology Stack

- **Framework**: ASP.NET Core
- **Database**: MySQL with Entity Framework Core
- **Authentication**: JWT Bearer Authentication
- **API Documentation**: Swagger/OpenAPI
- **ORM**: Entity Framework Core with Code-First approach

## Project Structure

```
AccountService/
├── AccountService.API/           # API controllers and startup configuration
├── AccountService.Core/          # Business logic, services, and domain models
│   ├── Entities/                 # Domain entities
│   ├── Interfaces/               # Service and repository interfaces
│   └── Services/                 # Service implementations
├── AccountService.Infrastructure/# Data access implementation
│   ├── Data/                     # Database context and migrations
│   └── Repositories/             # Repository implementations
└── AccountService.Shared/        # DTOs and shared models
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- MySQL Server 8.0 or later
- Git

### Installation and Setup

#### 1. Clone the Repository

```bash
git clone https://github.com/YourUsername/MicroNetHub.git
cd MicroNetHub/AccountService
```

#### 2. Restore Dependencies

```bash
# Restore all project dependencies
dotnet restore
```

#### 3. Configure Database Connection

Update the connection string in `AccountService.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=micronet_account;user=root;password=your_password"
  },
  "Jwt": {
    "Secret": "YourSecretKeyHere_MakeItLongAndComplex_AtLeast32Characters",
    "ExpiryMinutes": 60,
    "Issuer": "MicroNetHub",
    "Audience": "MicroNetHub"
  }
}
```

#### 4. Create the Database

Ensure your MySQL server is running, then create the database:

```sql
CREATE DATABASE micronet_account;
```

#### 5. Apply Migrations

Install the EF Core tools (if not already installed):
```bash
dotnet tool install --global dotnet-ef
```

Apply the migrations to create the database schema:

```bash
# Navigate to the API project directory
cd AccountService.API

# Apply existing migrations
dotnet ef database update --project ../AccountService.Infrastructure/AccountService.Infrastructure.csproj
```

#### 6. Run the Application

```bash
dotnet run
```

The application will automatically:
1. Connect to the MySQL database
2. Apply any pending migrations
3. Seed initial data (admin account)
4. Start the API service

You can access the Swagger UI at: https://localhost:5001/swagger

### Managing Database Migrations

The application is configured to automatically apply migrations at startup, but you can also manage them manually:

```bash
# List existing migrations
dotnet ef migrations list --project ../AccountService.Infrastructure/AccountService.Infrastructure.csproj

# Remove the last migration (if not applied to database)
dotnet ef migrations remove --project ../AccountService.Infrastructure/AccountService.Infrastructure.csproj

# Generate SQL script for migrations
dotnet ef migrations script --output migration.sql --project ../AccountService.Infrastructure/AccountService.Infrastructure.csproj
```

## API Endpoints

### Authentication

- `POST /api/auth/register` - Register a new user
- `POST /api/auth/login` - Login with email and password
- `POST /api/auth/google-login` - Login with Google
- `POST /api/auth/refresh-token` - Refresh the authentication token
- `POST /api/auth/logout` - Logout (requires authentication)
- `POST /api/auth/change-password` - Change password (requires authentication)

### Account Management

- `GET /api/accounts` - Get all accounts (admin only)
- `GET /api/accounts/{id}` - Get account by ID
- `PUT /api/accounts/{id}` - Update account
- `DELETE /api/accounts/{id}` - Delete account
- `GET /api/accounts/{id}/security-events` - Get security events for an account
- `GET /api/accounts/{id}/access-logs` - Get access logs for an account
- `POST /api/accounts/{id}/access-logs` - Add access log for an account

## Health Check

- `GET /health` - Check service health status

## Security

- JWT authentication with configurable expiry
- Password hashing (implementation required in AccountService)
- Role-based access control
- IP and device tracking for security monitoring
