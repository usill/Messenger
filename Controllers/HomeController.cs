using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestSignalR.Models.DTO;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IUserService messangerService)
        {
            _logger = logger;
            _userService = messangerService;
            _configuration = configuration;
        }
        
        public async Task<IActionResult> Index()
        {
            int userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            string avatarDir = _configuration.GetSection("Web:Avatar").Get<string>()!;

            ViewData["username"] = User.FindFirstValue(ClaimTypes.Name);
            ViewData["avatar"] = avatarDir + await _userService.GetAvatar(userId);

            List<GetContactsRequest> contacts = await _userService.GetContactsAsync(userId);
            contacts.OrderByDescending(c => c.linkedMessage.SendedAt);
            
            ViewData["contacts"] = contacts;

            return View();
        }
    }
}
