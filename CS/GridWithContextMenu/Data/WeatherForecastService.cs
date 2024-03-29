namespace GridWithContextMenu.Data
{
    public class WeatherForecastService {
        public enum Summaries {
            Freezing, Bracing, Chilly, Cool, Mild, Warm, Balmy, Hot, Sweltering, Scorching
        };
        private List<WeatherForecast>? Forecasts;
        public Task<List<WeatherForecast>> GetForecastAsync() {
            if (Forecasts == null) {
                var rnd = new Random();
                Forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                    ID = index,
                    Date = DateTime.Today.AddDays(index),
                    TemperatureC = rnd.Next(-20, 55),
                    Summary = (Summaries)rnd.Next(Enum.GetNames(typeof(Summaries)).Length)
                }).ToList();
            }
            return Task.FromResult(Forecasts);
        }
        public Task<List<WeatherForecast>> ChangeForecastAsync(WeatherForecast changed) {
            var orginalForecast = Forecasts.FirstOrDefault(i => i.ID == changed.ID);
            if (orginalForecast != null) {
                orginalForecast.TemperatureC = changed.TemperatureC;
                orginalForecast.Summary = changed.Summary;
                orginalForecast.Date = changed.Date;
            }
            else {
                Forecasts.Add(changed);
            }
            return Task.FromResult(Forecasts);
        }
        public Task<List<WeatherForecast>> Remove(WeatherForecast forecast) {
            Forecasts.Remove(forecast);
            return Task.FromResult(Forecasts);
        }
    }
}