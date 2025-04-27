using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TransitSurveyAzure.Data;
using TransitSurveyAzure.Models;

namespace TransitSurveyAzure.Pages.SurveyResponses
{
    public class CreateModel : PageModel
    {
        private readonly SurveyDbContext _context;

        public CreateModel(SurveyDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public MetroResidentSurveyResponses MetroResidentSurveyResponses { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.SurveyResults.Add(MetroResidentSurveyResponses);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
