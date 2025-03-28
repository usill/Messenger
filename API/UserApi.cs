using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestSignalR.Services.Interfaces;
using TestSignalR.Models.Helper;
using TestSignalR.Models.DTO;
using TestSignalR.Models;

namespace TestSignalR.API
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserApi : ControllerBase
    {
        private readonly IUserService _userService;
        public UserApi(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("find/{name}")]
        public async Task<ActionResult<UserRequest>> FindByName(string name)
        {
            var user = await _userService.FindByNameAsync(name);

            if(user == null)
            {
                return NotFound();
            }

            var userResult = Mapper.Map<User, UserRequest>(user);
            return Ok(JsonHelper.Serialize(userResult));
        }
    }
}
