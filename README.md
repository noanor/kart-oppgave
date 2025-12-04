# Luftfartshinder - National Register of Airspace Obstacles

## Project Description

System for reporting and control of airspace obstacles to the National Register of Airspace Obstacles (NRL).

This application is developed to give pilots and flight crews from NLA, the Air Force, and the Police Helicopter Service the ability to register obstacles in airspace. Reported data can then be reviewed and either approved or rejected by the registrar in NRL. The goal is to strengthen flight safety through a continuously updated register of airspace obstacles.

## Technologies

- ASP.NET Core 9.0 MVC
- Entity Framework Core
- ASP.NET Core Identity
- MySQL/MariaDB
- Leaflet (map client)
- Docker (containerization)

## Architecture

The solution is built following the Model-View-Controller (MVC) pattern:

- **Model** – Domain classes and validation logic
- **View** – Razor Views are used for displaying forms, overview pages, maps, and feedback
- **Controller** – Responsible for routing and business logic
- **Database** – Data is stored in MySQL/MariaDB and handled via Entity Framework Core with migrations through ApplicationDbContext and AuthDbContext

## Data Flow

1. User logs into the system
2. Pilots can register new obstacles through an interactive map (Home → Index)
3. Input is validated with model attributes
4. Obstacles are saved in a draft session before submission
5. Valid submissions are published to the database
6. Registrar can process and validate incoming reports directly in the system
7. Users receive notification of approval/rejection

## Functionality

- User registration with SuperAdmin approval
- Role-based access control (SuperAdmin, Registrar, FlightCrew)
- Reporting of airspace obstacles via interactive map (Leaflet)
- Draft system for temporary storage before submission
- Approval/rejection of reported obstacles
- User administration with filtering and search

## Operations and Deployment

The application is packaged as a Docker solution for easy running and scalability:

- The web part runs on `mcr.microsoft.com/dotnet/aspnet:9.0`
- The database runs in a MariaDB container
- Components communicate via connection strings configured in `appsettings.json`

## Running Instructions

### Prerequisites

You only need:
- **Docker Desktop** installed and running
- **Git** installed
- **Terminal** (Command Prompt on Windows, Terminal on Mac/Linux)

**Install Docker Desktop:**
- Download from: https://www.docker.com/products/docker-desktop/
- Install and start Docker Desktop
- Test: Open terminal and run `docker --version`

**Install Git:**
- Windows: https://git-scm.com/download/win
- Mac/Linux: Follow instructions for your operating system
- Test: Open terminal and run `git --version`

---

### Running the Application (3 Simple Steps)

**Step 1: Clone the project**
```bash
git clone https://github.com/noanor/kart-oppgave.git
```

**Step 2: Navigate to the project folder**
```bash
cd kart-oppgave/Luftfartshinder
```

**Step 3: Start the application**
```bash
docker-compose up --build
```

**Wait 2-5 minutes** (first time) while Docker sets up everything.

**Why `--build`?**
Docker caches (stores) images to save time. Without `--build`, Docker may use an old cached version if the files haven't changed much. With `--build`, we ensure that Docker always rebuilds with the latest changes from GitHub.

**When you see:**
```
luftfartshinder  | Now listening on: http://[::]:8080
```

**Open browser:** `http://localhost:8080`

---

### Stopping the Application

Press `Ctrl+C` in the terminal, or run:
```bash
docker-compose down
```

---

### Troubleshooting

**"Cannot find path" error:**
- Check that you are in the correct folder: Run `dir` (Windows) or `ls` (Mac/Linux)
- You should see the `docker-compose.yml` file
- If not, navigate to: `cd kart-oppgave/Luftfartshinder`

**"docker-compose: command not found":**
- Try: `docker compose` (without hyphen)

**Port 8080 already in use:**
- Stop other applications using port 8080
- Or change the port in `docker-compose.yml`

**If you still see old changes:**
- Check that you are using `docker-compose up --build` (not just `docker-compose up`)
- If the problem persists, force a complete rebuild:
  ```bash
  docker-compose down
  docker-compose build --no-cache
  docker-compose up --build
  ```

### Test Users

After the first startup, you can log in with:

**SuperAdmin:**
- Username: `superadmin@kartverket.no`
- Password: `Superadmin123!`

**Registrar:**
- Username: `registrar`
- Password: `Passord123!`

**Pilot:**
- Username: `pilot`
- Password: `Passord123!`

## Testing

### C# Tests

Run C# tests with:
```bash
cd Luftfartshinder/Luftfartshinder.Tests
dotnet test
```

<<<<<<< HEAD
**Test Implementation:**
- All 78 tests pass successfully
- Uses `TestSession` class for testing session functionality (replaces Moq ISession mocking to avoid extension method issues)
- Tests cover controllers: ObstaclesController, ReportController, AccountController, DashboardController, HomeController, RegistrarController, and SuperAdminController
- Repository tests: ObstacleRepositoryTests (9 tests) and ReportRepositoryTests (10 tests) using InMemory database
- All tests include documentation with GOAL, LOGIC, and RESULT

=======
>>>>>>> 9abab9b (Refaktorisering av registrar sider og superadmin list)
### JavaScript Tests

The project also contains JavaScript tests for frontend functionality using **Jest**.

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

Before running JavaScript tests for the first time, install dependencies:

```bash
# Navigate to project root (where package.json is located)
npm install
```

#### Running JavaScript Tests

After dependencies are installed, run the tests with:

```bash
# From project root
npm test
```

This runs all JavaScript tests in `Luftfartshinder/Luftfartshinder/wwwroot/js/`:
- `testHelperMap.test.js` - Tests map functionality
- `testHelperUserAdmin.test.js` - Tests user administration functionality

**Important:** JavaScript tests require Node.js and cannot be run without it. If you don't have Node.js installed, you must install it first.

## Documentation

- **Detailed documentation:** See [README.md](Luftfartshinder/Luftfartshinder/README.md) in the Luftfartshinder/Luftfartshinder folder for system architecture, operations, and testing
- **Testing:** See the [Testing section](Luftfartshinder/Luftfartshinder/README.md#testing) in README.md for test documentation and results
