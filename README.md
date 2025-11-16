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

## Dokumentasjon

- **Kjøreinstruksjoner:** Se [README.md](Luftfartshinder/README.md) i Luftfartshinder-mappen
- **Testing:** Se [TESTING.md](Luftfartshinder/TESTING.md) for testdokumentasjon og resultater
