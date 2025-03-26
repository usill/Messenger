using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.API
{
    [ApiController]
    [Route("/auth")]
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
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest request)
        {
            if (request.Password != request.PasswordRepeat)
            {
                return BadRequest();
            }

            string passwordHash = _authService.GetPasswordHash(request.Password);

            TimeZoneInfo moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"); // for linux "Europe/Moscow"
            DateTime moscowNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, moscowZone);

            User newUser = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                RegistredAt = moscowNow
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            string token = _authService.GenerateJwtToken(request.Username, newUser.Id.ToString());
            CookieOptions cookieOptions = _authService.GetCookie(token);
            Response.Cookies.Append("authToken", token, cookieOptions);

            return Ok();
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string passwordHash = _authService.GetPasswordHash(request.Password);
            User? user = _context.Users.Where(user => user.Username == request.Username && user.PasswordHash == passwordHash).FirstOrDefault();

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
        public IActionResult Logout()
        {
            Response.Cookies.Delete("authToken");
            return Ok();
        }
        [Authorize]
        [HttpGet("test")]
        public ActionResult<string> TestAuth()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok("Вы авторизованы: " + id);
        }
    }
}
