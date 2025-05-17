using Microsoft.AspNetCore.Identity;

namespace Dawam_backend.DTOs
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public IEnumerable<IdentityError> Errors { get; set; }
    }
}
