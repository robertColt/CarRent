using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRent.Models
{
    public abstract class Model
    {

        public override string ToString()
        {
            return this.GetType().Name;
        }
    }
}
