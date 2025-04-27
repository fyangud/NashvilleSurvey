namespace TransitSurveyAzure.Data
{
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations.Schema;
    using TransitSurveyAzure.Models;

    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options) { }

        public DbSet<MetroResidentSurveyResponses> SurveyResults { get; set; }

    }
}
