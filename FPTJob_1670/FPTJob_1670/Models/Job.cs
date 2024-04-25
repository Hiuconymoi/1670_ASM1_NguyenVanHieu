    using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace FPTJob_1670.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public string Title { get; set; }

        public DateTime Application_Deadline { get; set; }

        public int Salary { get; set; }

        public int? Employer_id { get; set; }

        [ForeignKey("Employer_id")]
        public virtual Employer? Employer { get; set; }

        public virtual ICollection<Application>? Application { get; set; }
        public string? Image { get; set; }

        [NotMapped]
        public IFormFile? ProfileImage { get; set; }

        public string? statusJobs { get; set; }

    }
}
