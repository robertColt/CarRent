using CarRent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRent.ViewModel
{
    public class UserManager
    {
        public static User CurrentUser
        {
            get; set;
        }
    }
}
