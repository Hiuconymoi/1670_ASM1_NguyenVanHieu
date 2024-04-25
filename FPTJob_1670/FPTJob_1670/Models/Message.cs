using Microsoft.AspNetCore.Identity;

namespace FPTJob_1670.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string? SenderId { get; set; }
        public string? ReceiverId { get; set; }
        public DateTime SentAt { get; set; }

        // Tham chiếu đến User thông qua IdentityUser
        public virtual IdentityUser? Sender { get; set; }
        public virtual IdentityUser? Receiver { get; set; }

    }
}
