# Luftfartshinder - Nasjonalt Register over Luftfartshindre

## Prosjektbeskrivelse

System for innmelding og kontroll av luftfartshindre til Nasjonalt register over luftfartshindre (NRL).

Denne applikasjonen er utviklet for å gi piloter og flybesetning fra NLA, Luftforsvaret og Politiets helikoptertjeneste mulighet til å registrere hindringer i luftrommet. Innmeldte data kan deretter gjennomgås og enten godkjennes eller avvises av registerfører i NRL. Målet er å styrke flysikkerheten gjennom et kontinuerlig oppdatert register over luftfartshindre.

## Teknologier

- ASP.NET Core 9.0 MVC
- Entity Framework Core
- ASP.NET Core Identity
- MySQL/MariaDB
- Leaflet (kartklient)
- Docker (containerisering)

## Arkitektur

Løsningen er bygd etter Model-View-Controller (MVC)-mønsteret:

- **Model** – Domeneklasser og valideringslogikk
- **View** – Razor Views benyttes for visning av skjemaer, oversiktssider, kart og tilbakemeldinger
- **Controller** – Ansvarlig for ruting og forretningslogikk
- **Database** – Data lagres i MySQL/MariaDB og håndteres via Entity Framework Core med migrasjoner gjennom ApplicationDbContext og AuthDbContext

## Dataflyt

1. Brukeren logger inn på systemet
2. Piloter kan registrere nye hindringer gjennom interaktivt kart (Home → Index)
3. Input blir validert med modellattributter
4. Hindre lagres i draft-session før innsending
5. Gyldige innsendelser publiseres til database
6. Registerfører kan behandle og validere innkomne rapporter direkte i systemet
7. Brukere får beskjed om godkjenning/avvisning

## Funksjonalitet

- Brukerregistrering med godkjenning av SuperAdmin
- Rollbasert tilgangskontroll (SuperAdmin, Registrar, FlightCrew)
- Rapportering av luftfartshindre via interaktivt kart (Leaflet)
- Draft-system for midlertidig lagring før innsending
- Godkjenning/avvisning av rapporterte hindre
- Brukeradministrasjon med filtrering og søk

## Drift og distribusjon

Applikasjonen er pakket som en Docker-løsning for enkel kjøring og skalerbarhet:

- Webdelen kjøres på `mcr.microsoft.com/dotnet/aspnet:9.0`
- Databasen kjører i en MariaDB container
- Komponentene kommuniserer via connection strings konfigurert i `appsettings.json`

## Kjøringsinstruksjoner

### Forutsetninger

**For Docker-kjøring:**
- Docker Desktop installert og kjørende
- Git (for å klone repository)
- Ingen annen programvare nødvendig

**For lokal utvikling:**
- .NET 9.0 SDK
- MySQL/MariaDB (or use Docker for database)
- Visual Studio 2022 or VS Code (recommended)

**For JavaScript testing:**
- Node.js (version 18 or newer recommended)
- npm (comes with Node.js)

### Kjøre med Docker (Anbefalt)

1. **Naviger til prosjektmappen:**
   ```bash
   cd Luftfartshinder
   ```

2. **Start applikasjonen:**
   ```bash
   docker-compose up
   ```

3. **Vent til applikasjonen starter** (kan ta 1-2 minutter første gang mens Docker bygger image og setter opp databasen)

4. **Åpne nettleser:**
   - Gå til `http://localhost:8080`

**Stoppe applikasjonen:**
```bash
docker-compose down
```

### Kjøre lokalt (dotnet run)

1. **Naviger til prosjektmappen:**
   ```bash
   cd Luftfartshinder/Luftfartshinder
   ```

2. **Sørg for at database kjører:**
   - Enten start MySQL/MariaDB lokalt
   - Eller kjør kun databasen med Docker: `docker-compose up db` (fra Luftfartshinder-mappen)

3. **Kjør applikasjonen:**
   ```bash
   dotnet run
   ```

4. **Åpne nettleser:**
   - Gå til `https://localhost:7258` eller `http://localhost:5062` (avhengig av konfigurasjon)

### Testbrukere

Etter første migrasjon er følgende testbrukere opprettet:

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

## Testing

### C# Tests

Run C# tests with:
```bash
cd Luftfartshinder/Luftfartshinder.Tests
dotnet test
```

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
