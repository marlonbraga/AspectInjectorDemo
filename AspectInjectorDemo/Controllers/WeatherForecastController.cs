using Microsoft.AspNetCore.Mvc;

namespace AspectInjectorDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var service = new WeatherForecastService();
            return service.Get();
        }
    }
}
