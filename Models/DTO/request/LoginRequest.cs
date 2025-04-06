using System.ComponentModel.DataAnnotations;

namespace TestSignalR.Models.DTO.request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string login { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string password { get; set; }
    }
}
