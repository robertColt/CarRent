using System;
using System.Collections.Generic;

namespace CarRent.Models.RentInfo
{
    public class Invoice : Model
    {
        public const double VAT = 0.19;

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double TotalAmount { get; set; }
        public double VatAmount { get; set; }
        public int UserId { get; set; }
        private List<Rent> Rents { get; set; }
        
        public void AddRent(Rent rent)
        {
            if(Rents == null)
            {
                Rents = new List<Rent>();
            }
            Rents.Add(rent);
            this.TotalAmount += rent.TotalAmount - rent.TotalAmount * rent.Discount;
        }
    }
}