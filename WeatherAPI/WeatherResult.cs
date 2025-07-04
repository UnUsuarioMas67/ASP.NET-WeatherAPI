using System.Text.Json.Serialization;

namespace WeatherAPI;

public class WeatherResult
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    [JsonPropertyName("resolvedAddress")] public string Address { get; set; }
    public string Timezone { get; set; }
    public double Tzoffset { get; set; }
    public List<WeatherDay>? Days { get; set; }
}

public class WeatherDay
{
    [JsonPropertyName("datetime")] public DateOnly Date { get; set; }
    
    public double Temp { get; set; }
    [JsonPropertyName("feelslike")] public double FeelsLike { get; set; }
    public double Dew { get; set; }
    
    [JsonPropertyName("precip")] public double Precipitation { get; set; }
    [JsonPropertyName("precipprob")] public double PrecipitationProbability { get; set; }
    [JsonPropertyName("precipcover")] public double PrecipitationCover { get; set; }
    [JsonPropertyName("preciptype")] public string[]? PrecipitationType { get; set; }
    
    public double Snow { get; set; }
    
    [JsonPropertyName("windspeed")] public double WindSpeed { get; set; }
    [JsonPropertyName("winddir")] public double WindDirection { get; set; }

    public double Pressure { get; set; }

    public string Conditions { get; set; }
    public string? Description { get; set; }
    public string Icon { get; set; }

    public List<WeatherHours> Hours { get; set; }
}

public class WeatherHours
{
    [JsonPropertyName("datetime")] public TimeOnly Time { get; set; }
    
    public double Temp { get; set; }
    [JsonPropertyName("feelslike")] public double FeelsLike { get; set; }
    public double Dew { get; set; }
    
    [JsonPropertyName("precip")] public double Precipitation { get; set; }
    [JsonPropertyName("precipprob")] public double PrecipitationProbability { get; set; }
    [JsonPropertyName("preciptype")] public string[]? PrecipitationType { get; set; }
    
    public double Snow { get; set; }
    
    [JsonPropertyName("windspeed")] public double WindSpeed { get; set; }
    [JsonPropertyName("winddir")] public double WindDirection { get; set; }

    public double Pressure { get; set; }

    public string Conditions { get; set; }
    public string Icon { get; set; }
}