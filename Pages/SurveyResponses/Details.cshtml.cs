using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TransitSurveyAzure.Data;
using TransitSurveyAzure.Models;

namespace TransitSurveyAzure.Pages.SurveyResponses
{
    public class DetailsModel : PageModel
    {
        private readonly SurveyDbContext _context;

        public DetailsModel(SurveyDbContext context)
        {
            _context = context;
        }

        public MetroResidentSurveyResponses MetroResidentSurveyResponses { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metroresidentsurveyresponses = await _context.SurveyResults.FirstOrDefaultAsync(m => m.zipcode == id);
            if (metroresidentsurveyresponses == null)
            {
                return NotFound();
            }
            else
            {
                MetroResidentSurveyResponses = metroresidentsurveyresponses;
            }
            return Page();
        }
    }
}
