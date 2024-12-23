using WrocRide.Models.Enums;

namespace WrocRide.Models
{
    public class DocumentDto
    {
        public DocumentStatus DocumentStatus { get; set; }
        public string FileLocation { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
