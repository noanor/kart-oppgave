# Luftfartshinder - Kartverket

## Kjøre Applikasjonen med Docker

### Forutsetninger
- **Docker Desktop** installert og kjørende
- **Git** (for å klone repository)
- Ingen annen programvare nødvendig (ikke Visual Studio, .NET SDK, eller MySQL)

### Hente Applikasjonen fra GitHub

1. **Klone repository:**
   ```bash
   git clone <github-repo-url>
   cd Luftfartshinder
   ```

   **Forklaring:** Dette henter hele prosjektet fra GitHub, inkludert all kode, konfigurasjoner og Docker-filer.

### Enkel Kjøring

1. **Naviger til prosjektmappen** (der `docker-compose.yml` ligger)

2. **Start applikasjonen:**
   ```bash
   docker-compose up
   ```

3. **Vent til applikasjonen starter** (kan ta 1-2 minutter første gang mens Docker bygger image og setter opp databasen)

4. **Åpne nettleser:**
   - Gå til `http://localhost:8080`

**Forklaring:** `docker-compose up` bygger automatisk Docker image, starter MariaDB database, kjører migrasjoner, og starter applikasjonen. Alt er konfigurert og klar til bruk uten manuell oppsett.

### Stoppe Applikasjonen

Trykk `Ctrl+C` i terminalen, eller kjør:
```bash
docker-compose down
```

### Standard Brukere

Etter første migrasjon er følgende brukere opprettet:

**SuperAdmin:**
- **Brukernavn:** `superadmin@kartverket.no`
- **Passord:** `Superadmin123!`
- **Rolle:** SuperAdmin (har tilgang til alle funksjoner)

**Registrar:**
- **Brukernavn:** `registrar`
- **Passord:** `Passord123!`
- **Rolle:** Registrar (kan godkjenne/avvise rapporterte hindre)

**Pilot (FlightCrew):**
- **Brukernavn:** `pilot`
- **Passord:** `Passord123!`
- **Rolle:** FlightCrew (kan rapportere nye luftfartshinder)



## Systemarkitektur

### Arkitekturmønstre
- **MVC (Model-View-Controller):** Separasjon mellom data, visning og logikk
- **Repository Pattern:** Abstraherer dataaksesslaget
- **Dependency Injection:** Løs kobling mellom komponenter

### Komponenter

**Controllers:**
- `HomeController` - Hjemmeside og rollbasert routing
- `AccountController` - Autentisering og registrering
- `SuperAdminController` - Brukeradministrasjon
- `ObstaclesController` - Håndtering av luftfartshinder
- `RegistrarController` - Godkjenning av rapporter

**Repositories:**
- `ObstacleRepository` - CRUD-operasjoner for hindre
- `ReportRepository` - Håndtering av rapporter
- `UserRepository` - Brukerdata
- `AccountRepository` - Brukerrapporter

**Databaser:**
- `KartverketDb` - Hoveddatabase (Datacontext: ApplicationContext) for hindre og rapporter
- `AuthKartverketDb` - Autentiseringsdatabase (Datacontext: AuthDbContext) for brukere og roller

**Autentisering:**
- ASP.NET Core Identity med roller: SuperAdmin, Registrar, FlightCrew
- Rollbasert autorisering med `[Authorize(Roles = "...")]`

---

## Drift

### Database Migrasjoner
Migrasjoner kjøres automatisk ved oppstart via `Program.cs`. Hvis manuell migrasjon trengs:

```bash
dotnet ef database update --context ApplicationContext
dotnet ef database update --context AuthDbContext
```

### Logging
Applikasjonen logger database-tilkoblinger og feil til konsollen.

### Miljøvariabler
Connection strings konfigureres i `appsettings.json`:
```json
"ConnectionStrings": {
  "DbConnection": "Server=127.0.0.1;Port=3307;Database=Kartverketdb;User=root;Password=root123",
  "AuthConnection": "Server=localhost;Port=3307;Database=AuthKartverketdb;User=root;Password=root123"
}
```

---

## Testing

### Testscenarier

**Login Testing:**
1.  Login GET returnerer view
2.  Ugyldige credentials avvises
3.  Ikke-godkjente brukere kan ikke logge inn
4.  Gyldige credentials redirecter korrekt
5.  SuperAdmin redirecter til SuperAdminHome

**SuperAdmin Testing:**
1.  Liste viser alle brukere
2.  Filtrering etter rolle fungerer
3.  Approve oppdaterer brukerstatus
4.  Approve håndterer ikke-eksisterende brukere
5.  Delete sletter brukere
6.  Decline sletter ventende brukere

### Testresultater

Alle tester kjører med `dotnet test` fra `Luftfartshinder.Tests`-mappen.

**Siste testkjøring:**
```
Test summary: total: 17; failed: 0; succeeded: 17; skipped: 0; duration: 2,3s
Build succeeded with 14 warning(s)
```

**Testkategorier:**
- **Enhetstesting (Unit Testing):** 3 tester - Tester enkeltkomponenter isolert
- **Systemstesting (System Testing):** 2 tester - Tester at hele systemet fungerer sammen  
- **Sikkerhetstesting (Security Testing):** 2 tester - Tester sikkerhetsaspekter
- **Brukervennlighetstesting (Usability Testing):** 2 tester - Tester brukervennlighet
- **Controller Testing:** 8 tester - Tester AccountController og HomeController

**Run tests:**
```bash
cd Luftfartshinder/Luftfartshinder.Tests
dotnet test
```

**Note:** There are 14 warnings related to null checks in the test code. These do not affect functionality, but should be fixed for optimal code quality.

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

## Roller

- **SuperAdmin:** Full tilgang, brukeradministrasjon
- **Registrar:** Godkjenner/avviser rapporterte hindre
- **FlightCrew:** Rapporterer nye luftfartshinder
