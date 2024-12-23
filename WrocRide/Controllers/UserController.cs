using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WrocRide.Entities;
using WrocRide.Models;
using WrocRide.Services;

namespace WrocRide.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/me")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<UserDto> Get()
        {
            var result = _userService.GetUser();

            return Ok(result);
        }


        [HttpPut]
        public ActionResult UpdateProfile([FromBody] UpdateUserDto dto)
        {
            _userService.UpdateUser(dto);

            return NoContent();
        }
    }
}
