# Kjøreinstruksjoner

## Forutsetninger

- .NET 9.0 SDK
- MariaDB/MySQL (versjon 11.8 eller nyere)
- Visual Studio 2022 eller nyere (anbefalt)
- Docker Desktop (valgfritt, for containerisering)

## Database Setup

1. Start MariaDB/MySQL server på port 3307

2. Opprett to databaser:
   ```sql
   CREATE DATABASE Kartverketdb;
   CREATE DATABASE AuthKartverketdb;
   ```

3. Oppdater connection strings i `appsettings.json` hvis nødvendig:
   ```json
   "ConnectionStrings": {
     "DbConnection": "Server=127.0.0.1;Port=3307;Database=Kartverketdb;User=root;Password=root123",
     "AuthConnection": "Server=localhost;Port=3307;Database=AuthKartverketdb;User=root;Password=root123"
   }
   ```

4. Kjør migrasjoner:
   ```bash
   dotnet ef database update --context ApplicationContext
   dotnet ef database update --context AuthDbContext
   ```

## Kjøre Applikasjonen

### Metode 1: Visual Studio

1. Åpne `Luftfartshinder.sln` i Visual Studio
2. Trykk `F5` eller klikk "Run"
3. Applikasjonen starter på `https://localhost:7258` (eller porten som er konfigurert)

### Metode 2: .NET CLI

1. Naviger til prosjektmappen:
   ```bash
   cd Luftfartshinder
   ```

2. Restore pakker:
   ```bash
   dotnet restore
   ```

3. Kjør applikasjonen:
   ```bash
   dotnet run
   ```

4. Åpne nettleseren og gå til URL-en som vises i konsollen (vanligvis `https://localhost:7258`)

### Metode 3: Docker

1. Bygg Docker image:
   ```bash
   docker build -t luftfartshinder:latest .
   ```

2. Kjør container:
   ```bash
   docker run -p 8080:8080 luftfartshinder:latest
   ```

3. Åpne nettleseren og gå til `http://localhost:8080`

**Merk:** Med Docker må du også kjøre MariaDB i en egen container og oppdatere connection strings.

## Standard Bruker

Etter første migrasjon er følgende SuperAdmin-bruker opprettet:

- **Brukernavn:** `superadmin@kartverket.no`
- **Passord:** `Superadmin123`

Denne brukeren har tilgang til alle funksjoner og kan opprette nye brukere.

## Roller

- **SuperAdmin:** Full tilgang, kan administrere brukere
- **Registrar:** Kan godkjenne/avvise rapporterte hindre
- **FlightCrew:** Kan rapportere nye hindre

## Testing

For detaljert testing-dokumentasjon, se [TESTING.md](TESTING.md).

Testene kan kjøres med:
```bash
cd Luftfartshinder.Tests
dotnet test
```
