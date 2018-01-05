using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models.RentInfo
{
    public class InvoiceDetail : Model
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public int RentId { get; set; }
    }
}