namespace WeatherAPI.Models;

public class WeatherData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Address { get; set; }
    public string? Description { get; set; }
    public double Temp { get; set; }
    public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    public double WindSpeed { get; set; }
    public double WindDirection { get; set; }
    public double Pressure { get; set; }
    public string Conditions { get; set; }

    public static WeatherData FromVisualCrossingResponse(VisualCrossingResponse response)
    {
        var data = new WeatherData
        {
            Latitude = response.Latitude,
            Longitude = response.Longitude,
            Address = response.Address,
            Description = response.Description,
            Temp = response.CurrentConditions.Temp,
            FeelsLike = response.CurrentConditions.FeelsLike,
            Humidity = response.CurrentConditions.Humidity,
            WindSpeed = response.CurrentConditions.WindSpeed,
            WindDirection = response.CurrentConditions.WindDirection,
            Pressure = response.CurrentConditions.Pressure,
            Conditions = response.CurrentConditions.Conditions,
        };
        
        return data;
    }
}