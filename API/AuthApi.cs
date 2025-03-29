using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
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
        public async Task<IActionResult> Registration([FromForm] RegistrationRequest request)
        {
            if (request.password != request.passwordRepeat)
            {
                return BadRequest();
            }

            string passwordHash = _authService.GetPasswordHash(request.password);

            TimeZoneInfo moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"); // for linux "Europe/Moscow"
            DateTime moscowNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowZone);

            string avatar = _authService.GetRandomAvatar();

            User newUser = new User
            {
                Username = request.username,
                PasswordHash = passwordHash,
                RegistredAt = moscowNow,
                Avatar = avatar
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            string token = _authService.GenerateJwtToken(request.username, newUser.Id.ToString());
            CookieOptions cookieOptions = _authService.GetCookie(token);
            Response.Cookies.Append("authToken", token, cookieOptions);

            return Ok();
        }
        [HttpPost("login")]
        public IActionResult Login([FromForm] LoginRequest request)
        {
            string passwordHash = _authService.GetPasswordHash(request.password);
            User? user = _context.Users.Where(u => u.Username == request.username && u.PasswordHash == passwordHash).FirstOrDefault();

            if(user == null)
            {
                return Unauthorized();
            }

            string token = _authService.GenerateJwtToken(user.Username, user.Id.ToString());
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
