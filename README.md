PadelBooking är ett REST-API för ett bokningssystem till en padelhall med tre banor. Detta system används för att hantera kunder och bokningar.
SYSTEMETS FUNKTIONALITET:
-skapa kunder
-hämta alla kunder
-uppdatera kunder
-ta bort kunder
-skapa bokningar
- hämta bokningar
- ta bort bokningar
- uppdatera bokningar
- visa bokningar för specifikt datum och bana
- visa bokningar mellan start- och slutdatum
- visa lediga tider för specifik dag
.................

SYSTEMETS REGLER:
-bokningar får göras mellan 07.00 till 22.00
-bokningar måste vara hela timmar
-inga dubbelbokningar
-endast 3 banor
-samma epost får inte registreras på olika kunder
..................

ARKITEKTUR:
API
-tar emot HTTP-anrop (GET, POST, PUT, DELETE)
-skickar vidare till service

CORE
-models: Booking och Customer
-service: logik (regler)
-repository och interface

TEST
-enhetstester: MsTest och Moq
.................

FUNKTIONEN:
När request skickas:
postman --> Controller --> Service --> Repository --> Database
.................

TEST:
-services
-controllers
-repositories
Enhetstester visar:
-bokningar fungerar som de ska
-fel tider godkänns inte
-dubbelbokning går inte
-kund-CRUD fungerar (kund kan skapas, läsas, uppdateras, tas bort)
Moq används för att simulera databasen så inte den riktiga datan avänds i testerna
.................

API-endpoints:
Booking:
-GET/api/bookings
-GEt/api/bookings/{id}
-POST/api/bookings
-PUT/api/bookings/{id}
-DELETE/api/bookings/{id}
-GET /api/bookings/date
-GET /api/bookings/between
-GET /api/bookings/available
-GET /api/bookings/date-and-court
Customer:
-GET/api/customers
-GET /api/customers/{id}
-POST/api/customers
-PUT/api/customers/{id}
-DEÖETE/api/customers/{id}

Projektet använder SQL server och Entity Framework Core

RELATIONER:
En kund kan ha flera bokningar (one-to-many)

Dependency Injection används för att koppla ihop repository, services, controllers.

FLÖDE:
Klient --> Controller --> Service --> Repository --> Database
-svaret skickas tillbaka samma väg.
