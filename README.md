
# TravelBookingPlatform

![.NET](https://img.shields.io/badge/.NET-7.0-blueviolet)
![EF Core](https://img.shields.io/badge/EF%20Core-7.0-informational)
![Swagger](https://img.shields.io/badge/Swagger-UI-green)
![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)

> **TravelBookingPlatform** is an open-source, multi-layered backend API for hotel, room, booking, and review management.  
> Built with **ASP.NET Core 7**, **Entity Framework Core**, JWT authentication, and follows clean architecture.

---

## 🚀 Features

- **User registration & JWT authentication**
- **Hotel search, filter, and details**
- **Bookings & reservations**
- **Hotel reviews and ratings**
- **Admin panel for CRUD operations (hotels, rooms, cities, room types, discounts)**
- **Swagger UI for API exploration**
- **Unit tests with xUnit & Moq**

---

## 🗂️ Project Structure

```
TravelBookingPlatform/
  TravelBooking.API/             # ASP.NET Core Web API (controllers, Program.cs)
  TravelBooking.Application/     # Services, DTOs, interfaces, business logic
  TravelBooking.Domain/          # Entities/models
  TravelBooking.Infrastructure/  # EF Core repositories, DbContext, migrations
  TravelBooking.Tests/           # xUnit test project
  appsettings.json               # API settings & JWT secret (in API project)
```

---

## ⚡ Getting Started

### 1. **Clone the repository**

```sh
git clone https://github.com/SaifKhalifa/TravelBookingPlatform.git
cd TravelBookingPlatform
```

### 2. **Configure settings**

- Edit `TravelBooking.API/appsettings.json`  
  Set your **JWT secret**, email server config, DB connection, etc.

  ```json
  {
    "Jwt": {
      "Key": "your-secret-key",
      "Issuer": "TravelBookingAPI",
      "Audience": "TravelBookingAPIUsers"
    },
    "Smtp": {
      "Email": "your.email@example.com",
      "Password": "email-app-password",
      "Host": "smtp.mail.com",
      "Port": 587
    },
    "ConnectionStrings": {
      "DefaultConnection": "Server=.;Database=TravelBookingDb;Trusted_Connection=True;"
    }
  }
  ```

> [!CAUTION]
> **Never commit real secrets to public repos!**.
> Such as JWT secret key, or your email.
  
### 3. **Run migrations**

```sh
cd TravelBooking.API
dotnet ef database update
```

### 4. **Build & run the API**

```sh
dotnet run --project TravelBooking.API
```

API should be available at:  
`https://localhost:7035`
<br>
`http://localhost:5099`
<br>
or any other port you configured in `TravelBooking.API/Properties/launchSettings.json`.

---

## 🛡️ Authentication (JWT)

- Register/login via `/api/auth/register` and `/api/auth/login` to receive a JWT.
- Add this header to every authorized request:

  ```
  Authorization: Bearer <your-jwt-token>
  ```
- Or you can use the Swagger UI and click "Authorize" to enter your JWT token.
> [!NOTE]]
> JWT tokens are valid for 1 hour by default. <br>
> You can change this in `TravelBooking.API/Controllers/AuthController.cs/CreateToken()`.

---

## 🧭 API Endpoints

### **Auth**

#### `POST /api/auth/register` — Register a new user

**Request JSON:**
```json
{
  "username": "saifkhalifa",
  "email": "saif@example.com",
  "password": "yourPassword123!"
}
```
**Response:**
- 200 OK (user created)
- 400 Bad Request (validation error)

#### `POST /api/auth/login` — Obtain JWT token

**Request JSON:**
```json
{
  "email": "saif@example.com",
  "password": "yourPassword123!"
}
```
**Response:**
```json
{
  "token": "jwt-token-here"
}
```

---

### **Hotels**

#### `GET /api/hotels` — List all hotels (supports filter query)

**Query Parameters:**  
- `city` (optional) — filter by city name
- `stars` (optional) — filter by star rating

**Example:**
```
GET /api/hotels?city=Nablus&stars=5
```

**Response:**
```json
[
  {
    "id": 1,
    "name": "Grand Nablus",
    "starRate": 5,
    "location": "City Center",
    "city": "Nablus"
  }
]
```

#### `GET /api/hotels/{id}` — Get hotel details with rooms

**Response:**
```json
{
  "id": 1,
  "name": "Grand Nablus",
  "starRate": 5,
  "location": "City Center",
  "city": "Nablus",
  "rooms": [
    {
      "id": 101,
      "roomNumber": "101",
      "type": "Suite",
      "pricePerNight": 150,
      "isAvailable": true
    }
  ]
}
```

---

### **Bookings**

> **Requires JWT**

#### `POST /api/bookings` — Create a booking

**Request JSON:**
```json
{
  "hotelId": 1,
  "roomId": 101,
  "checkIn": "2025-08-20",
  "checkOut": "2025-08-25",
  "guests": 2
}
```

#### `GET /api/bookings` — Get your bookings

**Response:**
```json
[
  {
    "id": 3,
    "hotelName": "Grand Nablus",
    "roomNumber": "101",
    "checkIn": "2025-08-20T00:00:00",
    "checkOut": "2025-08-25T00:00:00",
    "status": "Confirmed"
  }
]
```

#### `DELETE /api/bookings/{id}` — Cancel a booking

**Response:**  
- 200 OK on success  
- 404 Not Found if not found

---

### **Reviews**

> **Requires JWT**

#### `POST /api/reviews` — Leave a hotel review

**Request JSON:**
```json
{
  "hotelId": 1,
  "rating": 5,
  "comment": "Amazing hotel!"
}
```

**Response:**
- 200 OK ("Review submitted successfully")
- 400 Bad Request (already reviewed or invalid hotel)

#### `GET /api/reviews/hotel/{hotelId}` — Get all reviews for a hotel

**Response:**
```json
[
  {
    "id": 1,
    "userId": 12,
    "rating": 5,
    "comment": "Great stay!",
    "createdAt": "2025-08-08T10:15:00"
  }
]
```

#### `DELETE /api/reviews/{id}` — Delete your review

- 200 OK (soft-deleted)
- 403 Forbidden (not owner)
- 404 Not Found

---

### **Admin Panel (Protected, require admin JWT)**
> [!NOTE]
> **These endpoints are for admin users only!**

#### `POST /api/admin/hotels` — Create hotel

```json
{
  "name": "Hotel X",
  "starRate": 4,
  "location": "Beach Road",
  "cityId": 2
}
```

#### `PUT /api/admin/hotels/{id}` — Update hotel

```json
{
  "name": "Updated Hotel X",
  "starRate": 5,
  "location": "Downtown",
  "cityId": 2
}
```

#### `DELETE /api/admin/hotels/{id}` — Delete hotel

- 200 OK (soft-deleted)

#### `POST /api/admin/rooms` — Create room

```json
{
  "roomNumber": "302",
  "adults": 2,
  "children": 1,
  "pricePerNight": 100,
  "hotelId": 2,
  "roomTypeId": 1
}
```

#### `POST /api/admin/roomtypes` — Create room type

```json
{
  "name": "Suite"
}
```

#### `POST /api/admin/discounts` — Create discount

```json
{
  "percentage": 15
}
```

#### `GET /api/admin/hotels` — List all hotels

**Response:**
```json
[
  {
    "id": 1,
    "name": "Grand Nablus",
    "starRate": 5,
    "location": "City Center",
    "cityId": 1
  }
]
```

#### `GET /api/admin/hotels/{id}` — Get a hotel by ID

**Response:**
```json
{
  "id": 1,
  "name": "Grand Nablus",
  "starRate": 5,
  "location": "City Center",
  "cityId": 1
}
```

---

#### `GET /api/admin/rooms` — List all rooms

**Response:**
```json
[
  {
    "id": 5,
    "roomNumber": "105",
    "adults": 2,
    "children": 1,
    "pricePerNight": 100,
    "hotelId": 2,
    "roomTypeId": 1,
    "isAvailable": true,
    "discountId": null
  }
]
```

#### `GET /api/admin/rooms/{id}` — Get a room by ID

**Response:**
```json
{
  "id": 5,
  "roomNumber": "105",
  "adults": 2,
  "children": 1,
  "pricePerNight": 100,
  "hotelId": 2,
  "roomTypeId": 1,
  "isAvailable": true,
  "discountId": null
}
```

#### `PUT /api/admin/rooms/{id}` — Update room

```json
{
  "roomNumber": "201",
  "adults": 3,
  "children": 2,
  "pricePerNight": 120,
  "hotelId": 2,
  "roomTypeId": 2,
  "discountId": 1,
  "isAvailable": false
}
```

#### `DELETE /api/admin/rooms/{id}` — Delete room

- 200 OK (deleted)

---

#### `GET /api/admin/roomtypes` — List all room types

**Response:**
```json
[
  { "id": 1, "name": "Single" },
  { "id": 2, "name": "Suite" }
]
```

#### `DELETE /api/admin/roomtypes/{id}` — Delete room type

- 200 OK (deleted)

---

#### `GET /api/admin/discounts` — List all discounts

**Response:**
```json
[
  { "id": 1, "percentage": 15 }
]
```

#### `DELETE /api/admin/discounts/{id}` — Delete discount

- 200 OK (deleted)

---

#### `GET /api/admin/cities` — List all cities

**Response:**
```json
[
  { "id": 1, "name": "Nablus", "country": "Palestine", "postOffice": "PO Box 1001" }
]
```

#### `POST /api/admin/cities` — Create city

```json
{
  "name": "Ramallah",
  "country": "Palestine",
  "postOffice": "PO Box 2001"
}
```

#### `PUT /api/admin/cities/{id}` — Update city

```json
{
  "name": "Updated City",
  "country": "Jordan",
  "postOffice": "PO Box 7777"
}
```

#### `DELETE /api/admin/cities/{id}` — Delete city

- 200 OK (deleted)

#### `GET /api/admin/cities/{id}` — Get a city by ID

**Response:**
```json
{
  "id": 1,
  "name": "Nablus",
  "country": "Palestine",
  "postOffice": "PO Box 1001"
}
```

---

### **Other Useful Endpoints**

#### `GET /api/reviews/user/{userId}` — Get all reviews by a user

**Response:**
```json
[
  {
    "id": 1,
    "hotelId": 1,
    "rating": 4,
    "comment": "Nice hotel",
    "createdAt": "2025-08-08T12:00:00"
  }
]
```

#### `GET /api/bookings/{id}` — Get booking by ID

**Response:**
```json
{
  "id": 3,
  "hotelName": "Grand Nablus",
  "roomNumber": "101",
  "checkIn": "2025-08-20T00:00:00",
  "checkOut": "2025-08-25T00:00:00",
  "status": "Confirmed"
}
```

---

## 🛑 Error Handling

The API returns clear and consistent error responses for client and server errors.

### Common Error Responses

#### 400 Bad Request (Validation Error)
```json
{
  "status": 400,
  "error": "Bad Request",
  "message": "The email field is required.",
  "details": [
    "Email is required.",
    "Password must be at least 8 characters."
  ]
}
```

#### 401 Unauthorized
```json
{
  "status": 401,
  "error": "Unauthorized",
  "message": "Missing or invalid JWT token."
}
```

#### 403 Forbidden
```json
{
  "status": 403,
  "error": "Forbidden",
  "message": "You do not have permission to access this resource."
}
```

#### 404 Not Found
```json
{
  "status": 404,
  "error": "Not Found",
  "message": "Resource not found."
}
```

#### 500 Internal Server Error
```json
{
  "status": 500,
  "error": "Internal Server Error",
  "message": "An unexpected error occurred. Please try again later."
}
```

> **Tip:** Always check the error `message` and `status` fields to handle errors properly in your client app.

>>>>>>>>>>>>>>>>>

---

## 📝 Testing

Run all unit tests:

```sh
dotnet test
```

- Tests are in `TravelBooking.Tests/`
- Uses **xUnit** and **Moq**

---

## 📄 License

This project is **MIT Licensed** — use, modify, and share freely.

---

## 🤝 Contributing

1. Fork the repo and create your feature branch (`git checkout -b feature/my-feature`)
2. Commit your changes (`git commit -m 'Add feature'`)
3. Push to the branch (`git push origin feature/my-feature`)
4. Open a Pull Request

---

## 📬 Contact

For bugs, suggestions, or feature requests, please open an issue on GitHub.

---
