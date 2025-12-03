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

### Forutsetninger for Docker-kjøring

Før du kan kjøre applikasjonen, må du ha installert:

1. **Docker Desktop**
   - Last ned fra: https://www.docker.com/products/docker-desktop/
   - Installer og start Docker Desktop
   - Verifiser at Docker kjører (ikonet skal være synlig i system tray)
   - Test installasjon: Åpne terminal og kjør `docker --version`

2. **Git**
   - Windows: Last ned fra https://git-scm.com/download/win
   - Mac: Installer via Homebrew: `brew install git`
   - Linux: `sudo apt-get install git` (Ubuntu/Debian) eller `sudo yum install git` (RedHat/CentOS)
   - Verifiser installasjon: Åpne terminal og kjør `git --version`

**Ingen annen programvare er nødvendig** - Docker håndterer alt!

**Feilsøking:**
- Hvis `docker-compose` ikke fungerer, prøv `docker compose` (uten bindestrek) - nyere versjoner av Docker bruker dette
- Hvis Docker Desktop ikke starter, sjekk at virtualisering er aktivert i BIOS

### Kjøre med Docker (Anbefalt)

Følg disse stegene i terminalen (Command Prompt på Windows, Terminal på Mac/Linux):

**Hvis du ikke har prosjektet ennå:**

1. **Klon prosjektet fra GitHub:**
   ```bash
   git clone https://github.com/noanor/kart-oppgave.git
   ```
   
   Dette lager en mappe kalt `kart-oppgave` i den mappen du er i nå.

**Hvis du allerede har prosjektet klonet:**

1. **Finn hvor prosjektet er:**
   - Søk etter `kart-oppgave` mappen på datamaskinen din
   - Eller naviger til mappen der du klonet prosjektet tidligere

2. **Naviger til prosjektmappen:**
   
   Først, sjekk hvor du er:
   ```bash
   pwd
   ```
   (På Windows PowerShell: `Get-Location` eller bare se på prompten)
   
   Deretter naviger til prosjektmappen. Du må være i samme mappe som `kart-oppgave` mappen:
   ```bash
   cd kart-oppgave
   cd Luftfartshinder
   ```
   
   **Eller i ett steg:**
   ```bash
   cd kart-oppgave/Luftfartshinder
   ```
   
   **Eksempel:** Hvis du klonet i `C:\Users\Madalitso\Documents`:
   ```bash
   cd Documents
   cd kart-oppgave
   cd Luftfartshinder
   ```
   
   **Verifiser at du er i riktig mappe:** Du skal se `docker-compose.yml` filen hvis du kjører `ls` (eller `dir` på Windows).

3. **Start applikasjonen med Docker:**
   ```bash
   docker-compose up
   ```
   
   **Første gang kan ta 2-5 minutter** mens Docker:
   - Laster ned nødvendige images
   - Bygger applikasjonen
   - Setter opp databasen
   - Kjører migrasjoner

4. **Vent til du ser meldinger som:**
   ```
   luftfartshinder  | Now listening on: http://[::]:8080
   ```
   
   Dette betyr at applikasjonen er klar!

5. **Åpne nettleser og gå til:**
   - `http://localhost:8080`

**Stoppe applikasjonen:**
- Trykk `Ctrl+C` i terminalen, eller
- I en ny terminal, naviger til `kart-oppgave/Luftfartshinder` og kjør:
  ```bash
  docker-compose down
  ```

**Tips:**
- **Første gang tar lengre tid** - Docker må laste ned images og bygge applikasjonen
- Hvis port 8080 allerede er i bruk, kan du endre porten i `docker-compose.yml`
- For å se logger: `docker-compose up` viser logger i terminalen
- For å kjøre i bakgrunnen: `docker-compose up -d`
- For å stoppe og fjerne alt (inkludert data): `docker-compose down -v`
- Hvis noe går galt, prøv: `docker-compose down` og deretter `docker-compose up --build`

**Feilsøking:**
- **"Cannot find path" feil:** 
  - Sjekk at du er i riktig mappe. Kjør `pwd` (eller `Get-Location` på Windows) for å se hvor du er
  - Sjekk at `kart-oppgave` mappen eksisterer: Kjør `ls` (eller `dir` på Windows) for å se hvilke mapper som finnes
  - Hvis `kart-oppgave` ikke finnes, må du først klone prosjektet med `git clone`
  - Hvis du allerede har klonet prosjektet et annet sted, naviger dit først
- **"docker-compose: command not found":** Prøv `docker compose` (uten bindestrek) - nyere versjoner av Docker bruker dette
- **Port allerede i bruk:** Endre port 8080 til noe annet i `docker-compose.yml`, eller stopp den andre applikasjonen som bruker porten

### Kjøre lokalt (for utviklere)

**Kun for utviklere som vil kjøre applikasjonen uten Docker.**

**Forutsetninger:**
- .NET 9.0 SDK installert
- MySQL/MariaDB installert lokalt, eller Docker for database

**Instruksjoner:**

1. **Naviger til prosjektmappen:**
   ```bash
   cd kart-oppgave/Luftfartshinder/Luftfartshinder
   ```

2. **Sørg for at database kjører:**
   - Enten start MySQL/MariaDB lokalt
   - Eller kjør kun databasen med Docker: `docker-compose up db` (fra `kart-oppgave/Luftfartshinder` mappen)

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
