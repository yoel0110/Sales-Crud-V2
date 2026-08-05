namespace Sales.Application.Dtos
{
    public class LoginResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = null!;
        public string? Username { get; set; }
    }
}
