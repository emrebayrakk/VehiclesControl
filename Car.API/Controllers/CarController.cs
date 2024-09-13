using Car.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Car.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarController : ControllerBase
    {

        private readonly ILogger<CarController> _logger;
        private readonly ICarService _carService;

        public CarController(ILogger<CarController> logger, ICarService carService)
        {
            _logger = logger;
            _carService = carService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var cars = await _carService.GetCars();
                if (cars == null || !cars.Any())
                {
                    _logger.LogInformation("No cars found.");
                    return NotFound("No cars found.");
                }

                return Ok(cars);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting cars.");
                return StatusCode(500, "An error occurred while retrieving cars.");
            }
        }
    }
}
