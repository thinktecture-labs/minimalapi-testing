using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientService;

[Route("[controller]")]
public class MvcPatientController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromServices] PatientDbContext db)
    {
        return Ok(await db.Patients.ToListAsync());
    }
}