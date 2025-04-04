using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Models.Helper;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.API
{
    [ApiController]
    [Route("api/auth")]
    public class AuthApi : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        public AuthApi(IAuthService service, AppDbContext dbContext)
        {
            _authService = service;
            _context = dbContext;
        }
        [HttpPost("registration")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration([FromForm] RegistrationRequest request)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            if (request.password != request.passwordRepeat)
            {
                ModelState.AddModelError("password", " ");
                ModelState.AddModelError("passwordRepeat", "Пароли не совпадают.");
                return ValidationProblem();
            }

            var existUser = await _context.Users.Select(u => new { u.Login }).Where(u => u.Login == request.login).FirstOrDefaultAsync();

            if(existUser != null)
            {
                ModelState.AddModelError("login", "Логин занят.");
                return ValidationProblem();
            }

            string passwordHash = _authService.GetPasswordHash(request.password);

            string avatar = _authService.GetRandomAvatar();

            User newUser = new User
            {
                Login = request.login,
                Username = request.username,
                PasswordHash = passwordHash,
                RegistredAt = DateTimeHelper.GetMoscowTimestampNow(),
                Avatar = avatar
            };

            await _context.Users.AddAsync(newUser);

            User? systemUser = await _context.Users.Where((u) => u.Login == "system").FirstOrDefaultAsync();
            newUser.Contacts.Add(systemUser);

            await _context.SaveChangesAsync();

            await _context.Messages.AddAsync(new Message
            {
                RecipientId = newUser.Id,
                SenderId = systemUser.Id,
                Text = "Поздравляем с регистрацией!",
                SendedAt = DateTimeHelper.GetMoscowTimestampNow()
            });

            await _context.SaveChangesAsync();

            string token = _authService.GenerateJwtToken(request.login, newUser.Id.ToString());
            CookieOptions cookieOptions = _authService.GetCookie(token);
            Response.Cookies.Append("authToken", token, cookieOptions);

            return Ok();
        }
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login([FromForm] LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            string passwordHash = _authService.GetPasswordHash(request.password);
            User? user = _context.Users.Where(u => u.Login == request.login && u.PasswordHash == passwordHash).FirstOrDefault();

            if(user == null)
            {
                ModelState.AddModelError("login", " ");
                ModelState.AddModelError("password", "Некорректные данные");
                return ValidationProblem();
            }

            string token = _authService.GenerateJwtToken(user.Login, user.Id.ToString());
            CookieOptions cookieOptions = _authService.GetCookie(token);
            Response.Cookies.Append("authToken", token, cookieOptions);

            return Ok();
        }
        [HttpGet("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken");
            return Ok();
        }
    }
}
