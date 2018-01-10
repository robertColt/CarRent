using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models.RentInfo
{
    public class Vehicle : Model
    {
        public enum LicenseCategories
        {
            A,B,C,D,E,F,G
        }

        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Type { get; set; }
        public double PriceDay { get; set; }
        public double PriceHour { get; set; }
        public bool Damaged { get; set; }
        public string Description { get; set; }
        public DateTime NextRevision { get; set; }
        public LicenseCategories LicenseCategory { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Id + " " + Name;
        }
    }
}