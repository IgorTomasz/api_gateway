# API Gateway Microservice

## 📝 Opis projektu
API Gateway to główny punkt wejścia do systemu kasyna online, odpowiedzialny za routing żądań, autoryzację oraz komunikację między mikroserwisami.

Serwis jest jednym z czterech mikroserwisów tworzących kompletny system:
- 🎮 Game Service - obsługa logiki gier
- 👤 Account Service - zarządzanie kontami użytkowników
- 💰 Payment Service - obsługa płatności
- 🔀 API Gateway (ten projekt) - zarządzanie komunikacją między serwisami

## 🛠 Technologie
- ASP.NET Core
- JWT (JSON Web Tokens)
- Entity Framework Core
- MS SQL Server
- Docker
- REST API

## 🔒 Zabezpieczenia

### Poziom autoryzacji
1. **JWT Token**
   - Wymagany dla zabezpieczonych endpointów
   - Dostarczany w nagłówku Authorization
   - Format: `Bearer {token}`
   - Czas ważności: 15 minut

2. **Refresh Token**
   - Umożliwia odnowienie tokenu JWT
   - Przechowywany w sesji użytkownika
   - Brak określonego czasu ważności

3. **Role Based Access Control**
   - Dostępne role: Admin, Client
   - Domyślna rola dla nowych użytkowników: Client
   - Specjalne endpointy wymagające roli Admin

### Komunikacja międzyserwisowa
1. **Secret Header**
   - Nagłówek `X-Int-Secret` dodawany do każdego żądania między mikroserwisami
   - Wartość konfigurowana w `appsettings.json` (ApiKey:Secret)
   - Wymagany dla wszystkich wewnętrznych wywołań API

## 🚀 Endpointy API

### Zarządzanie użytkownikami
```http
# Rejestracja użytkownika
POST /api/gateway/user/register
```
Request body:
```json
{
    "username": "string",
    "password": "string",
    "email": "string",
    "name": "string",
    "lastname": "string",
    "dateOfBirth": "string (YYYY-MM-DD)"
}
```

```http
# Logowanie użytkownika
POST /api/gateway/user/login
```
Request body:
```json
{
    "username": "string",
    "password": "string"
}
```
Response:
```json
{
    "success": true,
    "message": {
        "sessionId": "guid",
        "token": "jwt-token",
        "refToken": "refresh-token"
    }
}
```

### Zarządzanie profilem
```http
# Zmiana hasła [Authorize]
POST /api/gateway/profile/update-password
```
Request body:
```json
{
    "sessionId": "guid",
    "password": "string"
}
```

```http
# Pobranie profilu użytkownika [Authorize]
GET /api/gateway/profile/{sessionId}

# Odświeżenie tokenu
POST /api/gateway/profile/refresh
```
Request body dla odświeżenia tokenu:
```json
{
    "sessionId": "guid",
    "refToken": "string"
}
```

### Zarządzanie płatnościami
```http
# Obsługa wpłaty [Authorize]
POST /api/gateway/payments/handle-deposit

# Obsługa wypłaty [Authorize]
POST /api/gateway/payments/handle-withdraw
```
Request body dla płatności:
```json
{
    "sessionId": "guid",
    "amount": "decimal",
    "paymentMethod": "string (Card|Paypal|Blik)",
    "metaData": {
        "key": "value"
    }
}
```

```http
# Pobranie salda [Authorize]
GET /api/gateway/profile/balance/{sessionId}

# Pobranie historii transakcji [Authorize]
GET /api/gateway/profile/transactions/{sessionId}
```

### Procesowanie gry
```http
# Obsługa akcji w grze [Authorize]
POST /api/gateway/games/{game}
```
Request body dla akcji w grze:
```json
{
    "userSessionId": "guid",
    "gameSessionId": "guid (wymagane dla Move i End)",
    "action": "string (Start|Move|End)",
    "betAmount": "decimal",
    "data": {
        // Dane specyficzne dla danej gry i akcji
    }
}
```

Przykład dla rozpoczęcia gry Mines:
```json
{
    "userSessionId": "guid",
    "action": "Start",
    "betAmount": 100.00,
    "data": {
        "minesCount": 5
    }
}
```

Przykład dla ruchu w grze Mines:
```json
{
    "userSessionId": "guid",
    "gameSessionId": "guid",
    "action": "Move",
    "betAmount": 100.00,
    "data": {
        "X": 2,
        "Y": 3
    }
}
```

Przykład dla zakończenia gry:
```json
{
    "userSessionId": "guid",
    "gameSessionId": "guid",
    "action": "End",
    "betAmount": 100.00,
    "data": {}
}
```

### Panel administracyjny
```http
# Pobranie wszystkich użytkowników [Authorize(Roles = "Admin")]
GET /api/gateway/adm/users

# Aktualizacja gier [Authorize(Roles = "Admin")]
PUT /api/gateway/adm/games/update

# Pobranie wszystkich gier (panel admina) [Authorize(Roles = "Admin")]
GET /api/gateway/adm/games
```

## 📤 Struktura odpowiedzi API
Każdy endpoint zwraca ujednoliconą strukturę odpowiedzi w formacie:

```csharp
public class HttpResponseModel
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public object? Message { get; set; }
}
```

## 🔐 Proces autoryzacji

### Flow logowania:
1. Użytkownik wysyła dane logowania
2. System weryfikuje dane w Account Service
3. Generowany jest token JWT i refresh token
4. Tworzona jest nowa sesja użytkownika
5. Zwracane są tokeny i ID sesji

### Flow odświeżania tokenu:
1. Użytkownik wysyła refresh token i ID sesji
2. System weryfikuje refresh token
3. Generowany jest nowy token JWT i refresh token
4. Aktualizowana jest sesja użytkownika
5. Zwracane są nowe tokeny

## ⚙️ Konfiguracja i uruchomienie

### Przy użyciu Dockera:
```bash
# Sklonuj repozytorium
git clone https://github.com/IgorTomasz/api-gateway.git

# Przejdź do katalogu projektu
cd api_gateway

# Zbuduj i uruchom kontenery
docker-compose up --build
```

### Lokalne uruchomienie:
1. Sklonuj repozytorium
2. Zaktualizuj connection string w `appsettings.json`
3. Wykonaj migracje bazy danych:
```bash
dotnet ef database update
```
4. Uruchom aplikację:
```bash
dotnet run
```

## 🔄 Komunikacja między serwisami

### Konfiguracja klienta HTTP
```csharp
services.AddHttpClient<IPaymentService, PaymentService>(c =>
{
    c.BaseAddress = new Uri(configuration["Services:PaymentService"]);
}).ConfigureHttpClient((provider, c) =>
{
    c.DefaultRequestHeaders.Add("X-Int-Secret", configuration["ApiKey:Secret"]);
});
```

### Funkcjonalności
- Routing żądań do odpowiednich mikroserwisów
- Automatyczne dodawanie nagłówka `X-Int-Secret` do komunikacji międzyserwisowej
- Agregacja odpowiedzi z różnych serwisów
- Obsługa błędów i timeout'ów
- Zarządzanie sesjami i tokenami

### Adresy mikroserwisów
Konfigurowane w `appsettings.json`:
```json
{
  "Services": {
    "PaymentService": "http://payment-service:5001",
    "AccountService": "http://account-service:5002",
    "GameService": "http://game-service:5003"
  },
  "ApiKey": {
    "Secret": "your-secret-key"
  }
}
```

## 👨‍💻 Autor
Igor Tomaszewski
