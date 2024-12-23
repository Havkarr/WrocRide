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

<<<<<<< HEAD
        [HttpPut]
=======
        [HttpPost]
>>>>>>> a93b2aa8f314e8c61b44c5323103c01c713f105e
        public ActionResult UpdateProfile([FromBody] UpdateUserDto dto)
        {
            _userService.UpdateUser(dto);

<<<<<<< HEAD
            return Ok();
=======
            return NoContent();
>>>>>>> a93b2aa8f314e8c61b44c5323103c01c713f105e
        }
    }
}
