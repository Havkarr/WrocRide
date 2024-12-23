using Microsoft.AspNetCore.Mvc;
using WrocRide.Services;
using WrocRide.Models;
using Microsoft.AspNetCore.Authorization;

namespace WrocRide.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("documents")]
        public ActionResult<IEnumerable<DocumentDto>> GetDocuments([FromQuery] DocumentQuery query)
        {
            var result = _adminService.GetDocuments(query);

            return Ok(result);
        }

    }
}
