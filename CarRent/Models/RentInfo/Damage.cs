using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models.RentInfo
{
    public class Damage : Model
    {
        public int Id { get; set; }
        public double FineAmount { get; set; }
        public bool Paid { get; set; }
        public DateTime Date { get; set; }
        public int VehicleId { get; set; }
        public int UserId { get; set; }
    }
}