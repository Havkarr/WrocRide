﻿using WrocRide.Entities;
using WrocRide.Models.Enums;

namespace WrocRide.Models
{
    public record RideDto
    {
        public string ClientName { get; set; }
        public string ClientSurename { get; set; }
        public string DriverName { get; set; }
        public string DriverSurename { get; set; }
        public string PickUpLocation { get; set; }
        public decimal Distance { get; set; }
        public string Destination { get; set; }
        public RideStatus RideStatus { get; set; }
    }

    public record RideDeatailsDto : RideDto
    {
        public decimal? Coast { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int DriverId { get; set; }
        public int ClientId { get; set; }
        public string CarBrand { get; set; }
        public string CarModel { get; set; }
        public int? Grade { get; set; }
    }
}
