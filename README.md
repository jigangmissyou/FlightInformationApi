# Flight Information API

A robust RESTful API for managing flight information, built with ASP.NET Core 8.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete flight records.
- **Search & Filtering**: Advanced search by airline, airports, and date ranges.
- **Pagination**: Efficiently handle large datasets with paginated results.
- **Friendly Responses**: Consistent API response format for success and error states.
- **Validation**: Comprehensive input validation with user-friendly error messages.
- **Documentation**: Integrated Swagger/OpenAPI documentation.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/jigangmissyou/FlightInformationApi.git
cd FlightInformationApi
```

### 2. Run the API

Navigate to the project directory and run the application:

```bash
cd FlightInformationApi
dotnet run
```

The API will start and listen on `http://localhost:5247` (or similar, check the console output).

### 3. Explore the API

Once running, you can access the Swagger UI to explore and test the endpoints:

- **Swagger UI**: [http://localhost:5247/swagger](http://localhost:5247/swagger)

## Running Tests

The project includes a comprehensive suite of unit tests covering controllers and services.

To run all tests:

```bash
dotnet test
```

To run tests with detailed output:

```bash
dotnet test --logger "console;verbosity=detailed"
```

## API Endpoints

### Flights

- `GET /api/flights` - Get all flights (paginated).
  - Parameters: `pageNumber`, `pageSize`
- `GET /api/flights/{id}` - Get a specific flight by ID.
- `POST /api/flights` - Create a new flight.
- `PUT /api/flights/{id}` - Update an existing flight.
- `DELETE /api/flights/{id}` - Delete a flight.
- `GET /api/flights/search` - Search for flights.
  - Parameters: `airline`, `departureAirport`, `arrivalAirport`, `date` (single), `startDate`, `endDate` (range).

## Project Structure

- **Controllers**: API endpoints and request handling.
- **Services**: Business logic and data processing.
- **DTOs**: Data Transfer Objects for API requests and responses.
- **Models**: Domain entities.
- **Data**: Database context (In-Memory EF Core).
- **Tests**: Unit tests for the application.
