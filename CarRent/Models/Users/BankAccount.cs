using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CarRent.Models
{
    public class BankAccount : Model
    {
        public enum CardTypes
        {
            DEBIT,
            CREDIT
        }

        public int Id { get; set; }
        public string Iban { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int SecurityNumber { get; set; }
        public CardTypes CardType { get; set; }
        public string BankName { get; set; }
        public int UserId { get; set; }
    }
}