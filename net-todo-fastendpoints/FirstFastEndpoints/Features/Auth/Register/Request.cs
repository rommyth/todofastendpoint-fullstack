namespace FirstFastEndpoints.Features.Auth.Register
{
    public class RegisterRequest
    {
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}
