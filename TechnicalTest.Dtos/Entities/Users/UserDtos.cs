namespace TechnicalTest.Dtos.Entities.Users
{
    public class CreateUserRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Mail { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
    }
}
