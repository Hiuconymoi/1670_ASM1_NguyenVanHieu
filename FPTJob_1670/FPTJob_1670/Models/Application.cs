using System.ComponentModel.DataAnnotations.Schema;

namespace FPTJob_1670.Models
{
    public class Application
    {
        public int Id { get; set; }
        public string CoverLetter { get; set; }
        public DateTime ApplicationDate { get; set; }

        public int Seeker_Id { get; set; }
  
        [ForeignKey("Seeker_Id")]
        public virtual Seeker? Seeker { get; set; }

        public int Job_Id { get; set; }
        [ForeignKey("Job_Id")]
        public virtual Job? Job { get; set; }

        public string? statusApplication { get; set; }
    }
}
