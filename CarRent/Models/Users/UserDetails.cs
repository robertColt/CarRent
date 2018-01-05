using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models
{
    public class UserDetails : Model
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
        public bool Premium { get; set; }
        public DateTime BirthDate { get; set; }
        public int UserId { get; set; }
    }
}