using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models
{
    public class BlackList : Model
    {
        private const int MAX_WARNINGS = 3;

        private int warnings;

        public int Id { get; set; }
        public int Warnings
        {
            get
            {
                return warnings;
            }
            set
            {
                if (value >= MAX_WARNINGS)
                {
                    Banned = true;
                }
                warnings = value;
            }
        }
        public bool Banned { get; set; }
        public int UserId { get; set; }
    }
}