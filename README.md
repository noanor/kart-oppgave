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
- MySQL/MariaDB (eller bruk Docker for database)
- Visual Studio 2022 eller VS Code (anbefalt)

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

### C# Testing (Backend)

Kjør C#-testene med:
```bash
cd Luftfartshinder/Luftfartshinder.Tests
dotnet test
```

### JavaScript Testing (Frontend)

Prosjektet inneholder også JavaScript-tester for frontend-funksjonalitet. Disse testene bruker **Jest** som testrammeverk.

#### Forutsetninger

For å kjøre JavaScript-testene må du ha:
- **Node.js** installert (versjon 18 eller nyere anbefalt)
- **npm** (kommer med Node.js)

#### Installere Node.js

1. **Last ned Node.js:**
   - Gå til [https://nodejs.org/](https://nodejs.org/)
   - Last ned LTS-versjonen (Long Term Support)
   - Installer med standard innstillinger

2. **Verifiser installasjon:**
   ```bash
   node --version
   npm --version
   ```

#### Installere avhengigheter

Før første gang du kjører JavaScript-testene, må du installere avhengighetene:

```bash
# Naviger til prosjektets rotmappe (der package.json ligger)
npm install
```

Dette installerer Jest og jest-environment-jsdom som er definert i `package.json`.

#### Kjøre JavaScript-testene

Etter at avhengighetene er installert, kan du kjøre testene med:

```bash
# Fra prosjektets rotmappe
npm test
```

Dette kjører alle JavaScript-testene i `Luftfartshinder/Luftfartshinder/wwwroot/js/`-mappen:
- `testHelperMap.test.js` - Tester kart-funksjonalitet
- `testHelperUserAdmin.test.js` - Tester brukeradministrasjons-funksjonalitet

#### Teststruktur

JavaScript-testene er organisert i `wwwroot/js/`:
- **Testfiler:** `*.test.js` - Inneholder testene
- **Helper-filer:** `testHelper*.js` - Inneholder funksjoner som testes

**Viktig:** JavaScript-testene krever Node.js og kan ikke kjøres uten det. Hvis du ikke har Node.js installert, må du installere det først (se instruksjoner over).

## Dokumentasjon

- **Detaljert dokumentasjon:** Se [README.md](Luftfartshinder/Luftfartshinder/README.md) i Luftfartshinder/Luftfartshinder-mappen for systemarkitektur, drift og testing
- **Testing:** Se [Testing-seksjonen](Luftfartshinder/Luftfartshinder/README.md#testing) i README.md for testdokumentasjon og resultater
