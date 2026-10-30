using Microsoft.AspNetCore.Mvc;

namespace DrinksApps.Controllers.ControllerApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class TesteController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { mensagem = "API do iDrink funcionando!" });
        }
    }
}