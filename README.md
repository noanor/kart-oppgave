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

Du trenger bare:
- **Docker Desktop** installert og kjørende
- **Git** installert
- **Terminal** (Command Prompt på Windows, Terminal på Mac/Linux)

**Installer Docker Desktop:**
- Last ned fra: https://www.docker.com/products/docker-desktop/
- Installer og start Docker Desktop
- Test: Åpne terminal og kjør `docker --version`

**Installer Git:**
- Windows: https://git-scm.com/download/win
- Mac/Linux: Følg instruksjoner for ditt operativsystem
- Test: Åpne terminal og kjør `git --version`

---

### Kjøre applikasjonen (3 enkle steg)

**Steg 1: Klon prosjektet**
```bash
git clone https://github.com/noanor/kart-oppgave.git
```

**Steg 2: Gå inn i prosjektmappen**
```bash
cd kart-oppgave/Luftfartshinder
```

**Steg 3: Start applikasjonen**
```bash
docker-compose up
```

**Vent 2-5 minutter** (første gang) mens Docker setter opp alt.

**Når du ser:**
```
luftfartshinder  | Now listening on: http://[::]:8080
```

**Åpne nettleser:** `http://localhost:8080`

---

### Stoppe applikasjonen

Trykk `Ctrl+C` i terminalen, eller kjør:
```bash
docker-compose down
```

---

### Feilsøking

**"Cannot find path" feil:**
- Sjekk at du er i riktig mappe: Kjør `dir` (Windows) eller `ls` (Mac/Linux)
- Du skal se `docker-compose.yml` filen
- Hvis ikke, naviger til: `cd kart-oppgave/Luftfartshinder`

**"docker-compose: command not found":**
- Prøv: `docker compose` (uten bindestrek)

**Port 8080 allerede i bruk:**
- Stopp andre applikasjoner som bruker port 8080
- Eller endre port i `docker-compose.yml`

**Ser ikke de nyeste endringene:**
- Docker cacher ofte gamle versjoner. For å få de nyeste endringene:
  1. Stopp applikasjonen: `docker-compose down`
  2. Bygg på nytt uten cache: `docker-compose build --no-cache`
  3. Start på nytt: `docker-compose up`
  
  **Eller i ett steg:**
  ```bash
  docker-compose down
  docker-compose up --build --force-recreate
  ```

### Testbrukere

Etter første oppstart kan du logge inn med:

**SuperAdmin:**
- Brukernavn: `superadmin@kartverket.no`
- Passord: `Superadmin123!`

**Registrar:**
- Brukernavn: `registrar`
- Passord: `Passord123!`

**Pilot:**
- Brukernavn: `pilot`
- Passord: `Passord123!`

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
