using Microsoft.AspNetCore.Identity;

namespace Web.Models.BO
{
    public class Livre
    {
        public int LivreId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public LivreStatus Status { get; set; }
        //Id de l'utilisateur propriétaire du livre (issu de la Table AspNetUser)
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }

    public enum LivreStatus
    {
        Submitted,
        Approved,
        Rejected
    }
}
