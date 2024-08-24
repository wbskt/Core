namespace Wbskt.Core.Web.Services
{
    public class User
    {
        public int UserId { get; set; }

        public required string UserName { get; set; }

        public required string EmailId { get; set; }

        public required string PasswordHash { get; set; }

        public required string PasswordSalt { get; set; }
    }
}