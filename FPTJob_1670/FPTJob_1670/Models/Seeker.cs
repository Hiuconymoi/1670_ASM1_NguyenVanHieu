using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPTJob_1670.Models
{
    public class Seeker
    {

        public int Id { get; set; }
        public string SeekerId { get; set; }

        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }


        [ForeignKey("SeekerId")]
        public virtual IdentityUser? User_ID { get; set; }

        public virtual ICollection<Application>? Application { get; set; }
    }
}
