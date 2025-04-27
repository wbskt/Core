using System.ComponentModel.DataAnnotations;

namespace Wbskt.Core.Service.Services;

public class UserRegistrationRequest
{
    public required string UserName { get; set; }

    [Required]
    public required string EmailId { get; set; }

    public required string Password { get; set; }
}