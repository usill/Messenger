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

            UserViewData? user = await _userService.GetViewDataAsync(userId);

            if(user == null)
            {
                return Redirect("/login");
            }

            ViewData["username"] = user.Username;
            ViewData["avatar"] = avatarDir + user.Avatar;
            ViewData["contacts"] = user.PreparedContacts;

            return View();
        }
    }
}
