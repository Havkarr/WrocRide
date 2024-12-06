﻿namespace WrocRide.Models
{
    public class RegisterDriverDto : RegisterUserDto
    {
        public float Pricing { get; set; }
        public string FileLocation { get; set; }
        public string LicensePlate { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string BodyColor { get; set; }
        public int YearProduced { get; set; }

    }
}