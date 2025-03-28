using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using TestSignalR.Models;
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
            ViewData["contacts"] = await _userService.GetContactsAsync(userId);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
