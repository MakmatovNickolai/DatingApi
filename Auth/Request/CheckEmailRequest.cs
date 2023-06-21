
using System.ComponentModel.DataAnnotations;

namespace Auth.Request;
public class CheckEmailRequest
{
    [EmailAddress]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
}