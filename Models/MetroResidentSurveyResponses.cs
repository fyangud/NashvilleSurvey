using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace TransitSurveyAzure.Models
{
    [Table("Metro_Resident_Survey_Responses_result")]
    public class MetroResidentSurveyResponses
    {
        [Key]
        //public int Id { get; set; }
        public int zipcode { get; set; }
        public byte num_response { get; set; }
        public double pub_access { get; set; }
        public double com_not_car { get; set; }
    }
}
