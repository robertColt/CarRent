using CarRent.Database;
using CarRent.Models;
using CarRent.Models.RentInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRent.Helper
{
    public class DAOManager
    {
        public static T GetDAO<T>()
        {

            return (T)Activator.CreateInstance(typeof(T));
        }

    }
}
