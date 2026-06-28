namespace FirstFastEndpoints.Features.Auth.Login
{
    public class LoginRequest
    {
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
