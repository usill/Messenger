using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TestSignalR.API
{
    [ApiController]
    [Route("/user")]
    [Authorize]
    public class UserApi : ControllerBase
    {
    }
}
