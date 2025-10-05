# kart-oppgave
Oppgave 1 IS-202



## Kartverket – Hinder registrering,
System for innmelding og kontroll av luftromshindringer

Denne applikasjonen er utviklet for å gi piloter mulighet til å registrere hindringer i luftrommet. Innmeldte data kan deretter gjennomgås og enten godkjennes eller avvises av registerfører i NRL. Målet er å styrke flysikkerheten gjennom et kontinuerlig oppdatert register over luftromshindringer.

Teknologi brukt:

ASP.NET Core MVC,
Entity Framework Core,
MySQL/MariaDB,
Docker for drift og portabilitet,

---

### Arkitektur
Løsningen er bygd etter Model–View–Controller (MVC)-mønsteret:

Model – Domeneklasser og valideringslogikk
View – Razor Views benyttes for visning av skjemaer, oversiktssider, kart og tilbakemeldinger.
Controller – Ansvarlig for ruting og forretningslogikk,
Database – Data vil i fremtidig versjon av appen lagres i MySQL/MariaDB og håndteres via Entity Framework Core med migrasjoner gjennom ApplicationDbContext.

---

### Dataflyt
Brukeren åpner startsiden (Home → Index).
Piloter kan registrere nye hindringer gjennom skjemaet (Index → ObstacleData → Vises  i OverView).
Input blir validert med modellattributter
Gyldige innsendelser publiseres til oversiktssiden.

Fremtidig funksjonalitet inkluderer at registerfører kan behandle og validere innkomne rapporter direkte i systemet.,

---

### Drift og distribusjon
Applikasjonen er pakket som en Docker-løsning for enkel kjøring og skalerbarhet. Med docker-compose kan både webapplikasjon og database startes i separate containere:

Webdelen kjøres på mcr.microsoft.com/dotnet/aspnet:9.0.
Databasen kjører i en mariadbcontainer.
Komponentene kommuniserer via et felles Docker-nettverk.
