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
    public class IndexModel : PageModel
    {
        private readonly SurveyDbContext _context;

        public IndexModel(SurveyDbContext context)
        {
            _context = context;
        }

        public IList<MetroResidentSurveyResponses> MetroResidentSurveyResponses { get; set; } = default!;

        public async Task OnGetAsync()
        {
            MetroResidentSurveyResponses = await _context.SurveyResults.ToListAsync();
        }
    }
}
