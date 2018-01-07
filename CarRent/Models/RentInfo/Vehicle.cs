using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models.RentInfo
{
    public class Vehicle : Model
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Type { get; set; }
        public double PriceDay { get; set; }
        public double PriceHour { get; set; }
        public bool Damaged { get; set; }
        public string Description { get; set; }
        public DateTime NextRevision { get; set; }
        public string LicenseCategory { get; set; }
        public string Name { get; set; }
    }
}