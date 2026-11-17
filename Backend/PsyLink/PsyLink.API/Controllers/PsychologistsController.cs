using Microsoft.AspNetCore.Mvc;

namespace PsyLink.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PsychologistsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPsychologists()
        {
            var psychologists = new[]
            {
                new { Id = 1, Name = "Dra. Ana Gómez", Specialty = "Psicología Clínica" },
                new { Id = 2, Name = "Lic. Carlos Ruiz", Specialty = "Terapia Cognitivo-Conductual" }
            };

            return Ok(psychologists);
        }
    }
}