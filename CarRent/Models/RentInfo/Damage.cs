using CarRent.Database;
using CarRent.Helper;
using System;
using System.Linq;

namespace CarRent.Models.RentInfo
{
    public class Damage : Model
    {
        private int vehicleId;

        public int Id { get; set; }
        public double FineAmount { get; set; }
        public DateTime Date { get; set; }
        public bool Paid { get; set; }
        public int VehicleId
        {
            get { return vehicleId; }
            set
            {
                vehicleId = value;
                try
                {
                    VehicleName = new VehicleDAO().GetVehicles(id: vehicleId).First().Name;
                }
                catch (Exception ex)
                {
                    DebugLog.WriteLine(ex);
                }
            }
        }
        public string VehicleName { get; set; }
        public int UserId { get; set; }
    }
}