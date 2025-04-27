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
    public class DeleteModel : PageModel
    {
        private readonly SurveyDbContext _context;

        public DeleteModel(SurveyDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var metroresidentsurveyresponses = await _context.SurveyResults.FindAsync(id);
            if (metroresidentsurveyresponses != null)
            {
                MetroResidentSurveyResponses = metroresidentsurveyresponses;
                _context.SurveyResults.Remove(MetroResidentSurveyResponses);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
