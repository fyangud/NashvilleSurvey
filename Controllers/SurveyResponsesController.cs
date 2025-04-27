using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TransitSurveyAzure.Data;
using TransitSurveyAzure.Models;

namespace TransitSurveyAzure.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly SurveyDbContext _context;

        public SurveyResponsesController(SurveyDbContext context)
        {
            _context = context;
        }

        // GET: api/surveyresponses/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetroResidentSurveyResponses>>> GetResponses()
        {
            //return await _context.SurveyResults.ToListAsync();
            var surveyresults = await _context.SurveyResults.ToListAsync();
            return Ok(surveyresults);
        }
    }
}
