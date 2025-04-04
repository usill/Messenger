using System.ComponentModel.DataAnnotations;

namespace TestSignalR.Models.DTO
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [MinLength(4, ErrorMessage = "Логин должен состоять минимум из 4 символов.")]
        [MaxLength(64, ErrorMessage = "Логин слишком длинный.")]
        public string login { get; set; }
        [MinLength(4, ErrorMessage = "Имя пользователя должно состоять минимум из 4 символов.")]
        [MaxLength(48, ErrorMessage = "Имя пользователя слишком длинное.")]
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string username { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        [MinLength(7, ErrorMessage = "Пароль должен быть более 6 символов.")]
        [MaxLength(255, ErrorMessage = "Пароль слишком длинный.")]
        public string password { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения.")]
        public string passwordRepeat { get; set; }
    }
}
