using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransitSurveyAzure.Data;
using TransitSurveyAzure.Models;

namespace TransitSurveyAzure.Pages.SurveyResponses
{
    public class EditModel : PageModel
    {
        private readonly SurveyDbContext _context;

        public EditModel(SurveyDbContext context)
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
            MetroResidentSurveyResponses = metroresidentsurveyresponses;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(MetroResidentSurveyResponses).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MetroResidentSurveyResponsesExists(MetroResidentSurveyResponses.zipcode))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool MetroResidentSurveyResponsesExists(int id)
        {
            return _context.SurveyResults.Any(e => e.zipcode == id);
        }
    }
}
