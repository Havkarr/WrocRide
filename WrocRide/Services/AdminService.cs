using Microsoft.AspNetCore.Mvc;
using WrocRide.Entities;
using WrocRide.Helpers;
using WrocRide.Models;
using WrocRide.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace WrocRide.Services
{
    public interface IAdminService
    {
        PagedList<DocumentDto> GetDocuments(DocumentQuery query);
        void UpdateDocument([FromRoute] int id, [FromBody] UpdateDocumentDto dto);
        DocumentDto GetByDriverId(int id);

    }
    public class AdminService : IAdminService
    {
        private readonly WrocRideDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IDriverService _driverService;
        public AdminService(WrocRideDbContext dbContext, IUserContextService userContextService, IDriverService driverService)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _driverService = driverService;
        }

        public PagedList<DocumentDto> GetDocuments(DocumentQuery query)
        {
            var documents = _dbContext.Documents
                .Where(d => d.DocumentStatus == Models.Enums.DocumentStatus.UnderVerification )
                .Select(d => new DocumentDto()
                {
                    Id = d.Id,
                    DocumentStatus = d.DocumentStatus,
                    FileLocation = d.FileLocation,
                    RequestDate = d.RequestDate
                })
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var result = new PagedList<DocumentDto>(documents, query.PageSize, query.PageNumber, documents.Count);

            return result;
        }

        public void UpdateDocument([FromRoute] int id, [FromBody] UpdateDocumentDto dto)
        {
            var document = _dbContext.Documents.FirstOrDefault(d => d.Id == id);

            if (document == null)
            {
                throw new NotFoundException("Document not found");
            }

            int? userId = _userContextService.GetUserId;
            var adminId = _dbContext.Admins.FirstOrDefault(u => u.UserId == userId).Id;

            document.DocumentStatus = dto.DocumentStatus;
            document.ExaminationDate = DateTime.UtcNow;
            document.AdminId = adminId;

            if (dto.DocumentStatus == Models.Enums.DocumentStatus.Accepted)
            {
                var status = new UpdateDriverStatusDto
                {
                    DriverStatus = Models.Enums.DriverStatus.Offline
                };
                var driver = _dbContext.Drivers.FirstOrDefault(d => d.DocumentId == id);

                if (driver == null)
                {
                    throw new NotFoundException("Driver not found");
                }

                var driverId = driver.Id;
                _driverService.UpdateStatus(driverId, status);
            }

            _dbContext.SaveChanges();
        }

        public DocumentDto GetByDriverId(int id)
        {
            var driver = _dbContext.Drivers.FirstOrDefault(d => d.Id == id);

            if (driver == null)
            {
                throw new NotFoundException("Driver not found");
            }

            var document = _dbContext.Documents.FirstOrDefault(doc => doc.Id == driver.DocumentId);

            if (document == null)
            {
                throw new NotFoundException("Document not found");
            }

            var result = new DocumentDto()
            {
                Id = document.Id,
                DocumentStatus = document.DocumentStatus,
                FileLocation = document.FileLocation,
                RequestDate = document.RequestDate
            };

            return result;
        }
    }
}
