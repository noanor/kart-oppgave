# Luftfartshinder - Kartverket

## Running the Application with Docker

### Prerequisites
- **Docker Desktop** installed and running
- **Git** (for cloning the repository)
- No other software required (no Visual Studio, .NET SDK, or MySQL)

### Getting the Application from GitHub

1. **Clone repository:**
   ```bash
   git clone <github-repo-url>
   cd Luftfartshinder
   ```

   **Explanation:** This retrieves the entire project from GitHub, including all code, configurations, and Docker files.

### Simple Running

1. **Navigate to the project folder** (where `docker-compose.yml` is located)

2. **Start the application:**
   ```bash
   docker-compose up
   ```

3. **Wait for the application to start** (may take 1-2 minutes the first time while Docker builds the image and sets up the database)

4. **Open browser:**
   - Go to `http://localhost:8080`

**Explanation:** `docker-compose up` automatically builds the Docker image, starts the MariaDB database, runs migrations, and starts the application. Everything is configured and ready to use without manual setup.

### Stopping the Application

Press `Ctrl+C` in the terminal, or run:
```bash
docker-compose down
```

### Default Users

After the first migration, the following users are created:

**SuperAdmin:**
- **Username:** `superadmin@kartverket.no`
- **Password:** `Superadmin123!`
- **Role:** SuperAdmin (has access to all functions)

**Registrar:**
- **Username:** `registrar`
- **Password:** `Passord123!`
- **Role:** Registrar (can approve/reject reported obstacles)

**Pilot (FlightCrew):**
- **Username:** `pilot`
- **Password:** `Passord123!`
- **Role:** FlightCrew (can report new airspace obstacles)



## System Architecture

### Architecture Patterns
- **MVC (Model-View-Controller):** Separation between data, view, and logic
- **Repository Pattern:** Abstracts the data access layer
- **Dependency Injection:** Loose coupling between components

### Components

**Controllers:**
- `HomeController` - Homepage and role-based routing
- `AccountController` - Authentication and registration
- `SuperAdminController` - User administration
- `ObstaclesController` - Handling of airspace obstacles
- `RegistrarController` - Approval of reports

**Repositories:**
- `ObstacleRepository` - CRUD operations for obstacles
- `ReportRepository` - Handling of reports
- `UserRepository` - User data
- `AccountRepository` - User reports

**Databases:**
- `KartverketDb` - Main database (Data context: ApplicationContext) for obstacles and reports
- `AuthKartverketDb` - Authentication database (Data context: AuthDbContext) for users and roles

**Authentication:**
- ASP.NET Core Identity with roles: SuperAdmin, Registrar, FlightCrew
- Role-based authorization with `[Authorize(Roles = "...")]`

---

## Operations

### Database Migrations
Migrations run automatically on startup via `Program.cs`. If manual migration is needed:

```bash
dotnet ef database update --context ApplicationContext
dotnet ef database update --context AuthDbContext
```

### Logging
The application logs database connections and errors to the console.

### Environment Variables
Connection strings are configured in `appsettings.json`:
```json
"ConnectionStrings": {
  "DbConnection": "Server=127.0.0.1;Port=3307;Database=Kartverketdb;User=root;Password=root123",
  "AuthConnection": "Server=localhost;Port=3307;Database=AuthKartverketdb;User=root;Password=root123"
}
```

---

## Testing

### Test Scenarios

**Login Testing:**
1.  Login GET returns view
2.  Invalid credentials are rejected
3.  Non-approved users cannot log in
4.  Valid credentials redirect correctly
5.  SuperAdmin redirects to SuperAdminHome

**SuperAdmin Testing:**
1.  List shows all users
2.  Filtering by role works
3.  Approve updates user status
4.  Approve handles non-existing users
5.  Delete removes users
6.  Decline removes pending users

### Test Results

All tests run with `dotnet test` from the `Luftfartshinder.Tests` folder.

**Latest test run:**
```
Test summary: total: 78; failed: 0; succeeded: 78; skipped: 0; duration: 2,1s
Build succeeded with 40 warning(s)
```

**Test Categories:**
- **Unit Testing:** 3 tests - Tests individual components in isolation
- **System Testing:** 2 tests - Tests that the entire system works together  
- **Security Testing:** 2 tests - Tests security aspects
- **Usability Testing:** 2 tests - Tests usability
- **Controller Testing:** 50 tests - Tests all controllers (AccountController, HomeController, ObstaclesController, ReportController, RegistrarController, SuperAdminController, DashboardController)
- **Repository Testing:** 19 tests - ObstacleRepositoryTests (9 tests) and ReportRepositoryTests (10 tests) with InMemory database

**Test Implementation:**
- Uses `TestSession` class for testing session functionality (replaces Moq ISession mocking to avoid extension method issues)
- All tests have documentation with GOAL, LOGIC, and RESULT
- Repository tests use InMemory database for isolation and fast execution
- All 78 tests pass

**Run tests:**
```bash
cd Luftfartshinder/Luftfartshinder.Tests
dotnet test
```

**Note:** There are 40 warnings related to null checks in the test code. These do not affect functionality, but should be fixed for optimal code quality.

### JavaScript Testing

The project also contains JavaScript tests for frontend functionality. These tests use **Jest** as the testing framework.

#### Prerequisites

To run JavaScript tests, you need:
- **Node.js** installed (version 18 or newer recommended)
- **npm** (comes with Node.js)

#### Installing Node.js

1. **Download Node.js:**
   - Go to [https://nodejs.org/](https://nodejs.org/)
   - Download the LTS version (Long Term Support)
   - Install with default settings

2. **Verify installation:**
   ```bash
   node --version
   npm --version
   ```

#### Installing Dependencies

Before running JavaScript tests for the first time, you must install dependencies:

```bash
# Navigate to project root (where package.json is located)
cd <project-root>

# Install dependencies
npm install
```

This installs Jest and jest-environment-jsdom as defined in `package.json`.

#### Running JavaScript Tests

After dependencies are installed, you can run the tests with:

```bash
# From project root
npm test
```

This runs all JavaScript tests in the `wwwroot/js/` folder:
- `testHelperMap.test.js` - Tests map functionality
- `testHelperUserAdmin.test.js` - Tests user administration functionality

#### Test Structure

JavaScript tests are organized in `wwwroot/js/`:
- **Test files:** `*.test.js` - Contains the tests
- **Helper files:** `testHelper*.js` - Contains functions being tested

**Important:** JavaScript tests require Node.js and cannot be run without it. If you don't have Node.js installed, you must install it first (see instructions above).

## Roles

- **SuperAdmin:** Full access, user administration
- **Registrar:** Approves/rejects reported obstacles
- **FlightCrew:** Reports new airspace obstacles
