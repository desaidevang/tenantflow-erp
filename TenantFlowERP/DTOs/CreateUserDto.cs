namespace TenantFlowERP.DTOs
{
    public class CreateUserDto
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        // Admin or Staff
        public string Role { get; set; } = "Staff";
    }
}