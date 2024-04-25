using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;

namespace FPTJob_1670.Models
{
    public class Notification
    {
       public int? Id { get; set; }

        public string? UserId { get; set; }

        public virtual IdentityUser? User_ID { get; set; }

        public string? Message { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
