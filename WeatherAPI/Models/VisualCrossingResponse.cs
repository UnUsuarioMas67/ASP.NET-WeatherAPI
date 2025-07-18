using System.Text.Json.Serialization;

namespace WeatherAPI.Models;

public class VisualCrossingResponse
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [JsonPropertyName("resolvedAddress")] public string Address { get; set; }
    public string? Description { get; set; }
    public CurrentConditions CurrentConditions { get; set; }
}

public class CurrentConditions
{
    public double Temp { get; set; }
    [JsonPropertyName("feelslike")] public double FeelsLike { get; set; }
    public double Humidity { get; set; }
    [JsonPropertyName("windspeed")] public double WindSpeed { get; set; }
    [JsonPropertyName("winddir")] public double WindDirection { get; set; }
    public double Pressure { get; set; }
    public string Conditions { get; set; }
}