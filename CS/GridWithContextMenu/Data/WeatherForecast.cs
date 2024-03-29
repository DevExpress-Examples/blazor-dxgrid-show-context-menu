using static GridWithContextMenu.Data.WeatherForecastService;

namespace GridWithContextMenu.Data
{
    public class WeatherForecast {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public Summaries Summary { get; set; }
    }
}