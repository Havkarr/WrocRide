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

        [HttpPut("documents/{id}")]
        public ActionResult UpdateDocument([FromRoute] int id, [FromBody] UpdateDocumentDto dto)
        {
            _adminService.UpdateDocument(id, dto);

            return NoContent();
        }

        [HttpGet("documents/{driverId}")]
        public ActionResult<DocumentDto> GetDocumentByDriverId([FromRoute] int driverId)
        {
            var result = _adminService.GetByDriverId(driverId);

            return Ok(result);
        }
    }
}
