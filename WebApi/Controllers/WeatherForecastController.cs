using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("Test")]
    public class WeatherForecastController : BaseController
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
        
        public WeatherForecastController()
        {
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetWeatherForecast")]
        public IActionResult Get()
        {
            return Ok("Test worked");
        }
    }
}