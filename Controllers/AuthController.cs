using Microsoft.AspNetCore.Mvc;

namespace TestSignalR.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpGet("/registration")]
        public IActionResult Registration()
        {
            return View();
        }
    }
}
