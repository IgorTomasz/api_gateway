using System.ComponentModel.DataAnnotations;

namespace api_gateway.models.DTOs
{
    public class ChangeUserPasswordRequest
    {
        [Required]
        public Guid SessionId { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
