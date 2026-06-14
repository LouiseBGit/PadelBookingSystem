PadelBooking är ett REST-API för ett bokningssystem till en padelhall med tre banor. Detta system används för att hantera kunder och bokningar.
SYSTEMETS FUNKTIONALITET:
-skapa kunder
-se alla kunder
-uppdatera kunder
-ta bort kunder
-skapa bokningar
- se bokningar
- ta bort bokningar
- uppdatera bokningar
.................

SYSTEMETS REGLER:
-bokningar får göras mellan 07.00 till 22.00
-bokningar måste vara hela timmar
-inga dubbelbokningar
-endast 3 banor
..................

ARKITEKTUR:
API
-tar emot HTTP-anrop (GET, POST, PUT, DELETE)
-skickar vidare till service

CORE
-models: Booking och Customer
-logik: regler för bokningar
-repository och interface

TEST
-enhetstester: MsTest och Moq
.................

FUNKTIONEN:
När request skickas:
postman --> Controller --> Service --> Repository --> Database
.................

TEST:
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
-POST/api/bookings
-PUT/api/bookings/{id}
-DELETE/api/bookings/{id}
Customer:
-GET/api/customers
-POST/api/customers
-PUT/api/customers/{id}
-DEÖETE/api/customers/{id}

-
