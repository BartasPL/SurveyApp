# Aplikacja - Tworzenie i wypełnianie ankiet online

## 🛠 Wykorzystane technologie
* .NET (ASP.NET Core MVC)
* SQLite (baza plikowa, nie wymaga instalacji dodatkowego serwera SQL)
* Entity Framework Core
* ASP.NET Core Identity

## Instrukcja uruchomienia

1. Otwórz terminal w głównym folderze projektu (tam, gdzie znajduje się plik `.csproj`).
2. Wpisz polecenie:
   `dotnet run`
3. Otwórz przeglądarkę internetową i przejdź pod adres, który wyświetli się w terminalu (np. `http://localhost:5125`).

## WAŻNE: Instrukcja testowania i nadawania ról

Zgodnie z wymaganiami, aplikacja posiada dwie role:

* **Ankieter:** może zakładać nowe ankiety.
* **Respondent:** może wyłącznie przeglądać listę, oddać jeden głos i sprawdzić wykres.

Domyślnie, nowo zarejestrowany użytkownik nie posiada uprawnień do tworzenia ankiet. Z racji braku pełnego panelu administracyjnego w wymogach projektu, przygotowałem specjalny endpoint ułatwiający sprawdzanie zadania.

### Jak uzyskać rolę "Ankieter" (krok po kroku):

1. **Zarejestruj się** w aplikacji i zaloguj na swoje nowo utworzone konto.
2. Będąc zalogowanym, **wpisz ręcznie w pasek adresu** przeglądarki następujący link:
   `/Survey/DajMiRole` 
   *(czyli np. `http://localhost:5125/Survey/DajMiRole`)*
3. Pojawi się biały ekran z potwierdzeniem: *"Gratulacje, jestes teraz Ankieterem!"*.
4. **Wyloguj się i zaloguj ponownie**. Jest to konieczne, aby ciasteczka uwierzytelniające (cookies) pobrały zaktualizowane uprawnienia z bazy danych.
5. Wejdź w zakładkę **"Ankiety"** na górnym pasku – zobaczysz teraz przycisk do tworzenia nowej ankiety.

### Jak testować głosowanie?
Po utworzeniu ankiety, polecam wylogować się i założyć drugie, osobne konto (lub otworzyć aplikację w trybie Incognito). Pozwoli to przetestować system głosowania z perspektywy zwykłego Respondenta oraz sprawdzić blokadę przed ponownym oddaniem głosu (system przekierowuje wtedy prosto do wykresu).