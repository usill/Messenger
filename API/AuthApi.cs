using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Actors;
using TestSignalR.Models;
using TestSignalR.Models.DTO.request;
using TestSignalR.Models.Enums;
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
        private readonly IAvatar _avatar;
        private readonly IHasher _hasher;
        private readonly ICookie _cookie;
        public AuthApi(IAuthService service, AppDbContext dbContext, IAvatar avatar, IHasher hasher, ICookie cookie)
        {
            _authService = service;
            _context = dbContext;
            _avatar = avatar;
            _hasher = hasher;
            _cookie = cookie;
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

            string passwordHash = _hasher.GetHash(request.password);
            string avatar = _avatar.GetRandom();

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
            newUser.Contacts.Add(new Contact
            {
                User = systemUser,
                HasNewMessage = true
            });

            await _context.SaveChangesAsync();

            await _context.Messages.AddAsync(new Message
            {
                RecipientId = newUser.Id,
                SenderId = systemUser.Id,
                Text = "Поздравляем с регистрацией!",
                SendedAt = DateTimeHelper.GetMoscowTimestampNow()
            });

            await _context.SaveChangesAsync();

            string refreshToken = _authService.GenerateRefreshToken();
            setCookies(request.login, newUser.Id.ToString(), refreshToken);
            await _authService.SetRefreshTokenAsync(refreshToken, newUser.Id);

            return Ok();
        }
        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginRequest request)
        {
            if(!ModelState.IsValid)
            {
                return ValidationProblem();
            }

            string passwordHash = _hasher.GetHash(request.password);
            User? user = _context.Users.Where(u => u.Login == request.login && u.PasswordHash == passwordHash).FirstOrDefault();

            if(user == null)
            {
                ModelState.AddModelError("login", " ");
                ModelState.AddModelError("password", "Некорректные данные");
                return ValidationProblem();
            }

            string refreshToken = _authService.GenerateRefreshToken();
            setCookies(user.Login, user.Id.ToString(), refreshToken);
            await _authService.SetRefreshTokenAsync(refreshToken, user.Id);

            return Ok();
        }
        public void setCookies(string login, string id, string refreshToken)
        {
            CookieOptions accessCookieOptions = _cookie.GetOptions(TokenType.Access);
            CookieOptions refreshCookieOptions = _cookie.GetOptions(TokenType.Refresh);

            string accessToken = _authService.GenerateAccessToken(login, id);

            Response.Cookies.Append("authToken", accessToken, accessCookieOptions);
            Response.Cookies.Append("refreshToken", refreshToken, refreshCookieOptions);
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
