using WrocRide.Entities;
using WrocRide.Helpers;
using WrocRide.Models;

namespace WrocRide.Services
{
    public interface IAdminService
    {
        PagedList<DocumentDto> GetDocuments(DocumentQuery query);

    }
    public class AdminService : IAdminService
    {
        private readonly WrocRideDbContext _dbContext;
        public AdminService(WrocRideDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PagedList<DocumentDto> GetDocuments(DocumentQuery query)
        {
            var documents = _dbContext.Documents
                .Where(d => d.DocumentStatus == Models.Enums.DocumentStatus.UnderVerification )
                .Select(d => new DocumentDto()
                {
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

    }
}
