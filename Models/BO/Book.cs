using Microsoft.AspNetCore.Identity;

namespace Web.Models.BO
{
    public class Book
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public BookStatus Status { get; set; }
        //Id de l'utilisateur propriétaire du livre (issu de la Table AspNetUser)
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }

    public enum BookStatus
    {
        Available,
        Borrowed
    }
}
