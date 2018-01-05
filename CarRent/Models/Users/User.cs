using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models
{
    public class User : Model
    {
        public enum Function
        {
            USER, ADMIN
        }

        private UserDetails userDetails;

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Function UserFunction { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int UserDetailsId { get; set; }
        public UserDetails UserDetails
        {
            get { return this.userDetails; }
            set
            {
                this.userDetails = value;
                this.UserDetailsId = this.userDetails.Id;
            }
        }
    }
}