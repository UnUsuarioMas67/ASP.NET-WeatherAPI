# WeatherAPI

Weather API that fetches and returns weather data from a 3rd party API. This project makes use of 3rd party APIs,
caching and enviroment variables.

Solution for the roadmap.sh project: [Weather API](https://roadmap.sh/projects/weather-api-wrapper-service)

## Features

- Fetch weather data from an specified city and date range using Visual Crossing API.
- Caching with Redis to reduce API calls.
- Implements fixed window rate limiting using Microsoft.AspNetCore.RateLimiting middleware.

## Requirements

- .NET 9 SDK or later
- Redis
- Visual Crossing account
- Visual Studio 2022 or another IDE of choice

## Installation

### 1. Clone the repository

```bash
git clone https://github.com/UnUsuarioMas67/ASP.NET-WeatherAPI
cd WeatherAPI
```

### 2. Setup Environment Variables

First reate a `.env` file inside the `WeatherAPI` folder. Then add the following variables to the file:

```
# Visual Crossing
VISUAL_CROSSING_API_KEY=YOUR API KEY
# Redis
REDIS_CONNECTION_STRING=YOUR CONNECTION STRING
REDIS_USERNAME=YOUR USER
REDIS_PASSWORD=YOUR PASSWORD
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

## Usage

### Endpoint

**GET** `/api/weather/{location}`

### Example Request

````
GET http://localhost:5043/api/weather/New%20York
````

### Example Response

````json
{
  "latitude": 40.7146,
  "longitude": -74.0071,
  "address": "New York, NY, United States",
  "description": "Similar temperatures continuing with a chance of rain tomorrow.",
  "temp": 28.5,
  "feelsLike": 29.5,
  "humidity": 55,
  "windSpeed": 13.7,
  "windDirection": 303,
  "pressure": 1012,
  "conditions": "Clear"
}
````

