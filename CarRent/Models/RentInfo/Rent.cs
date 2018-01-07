using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models.RentInfo
{
    public class Rent : Model
    {
        private int vehicleId;

        public int Id { get; set; }
        public DateTime BeginTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool Returned { get; set; }
        public double Discount { get; set; }
        public double TotalAmount { get; set; }
        public bool Paid { get; set; }
        public int VehicleId
        {
            get { return vehicleId; }
            set
            {
                vehicleId = value;
                VehicleName = new VehicleDAO()
            }
        }
        public int UserId { get; set; }

        public string VehicleName { get; set; }

    }
}