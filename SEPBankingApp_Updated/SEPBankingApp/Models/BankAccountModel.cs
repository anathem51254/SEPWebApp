using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace SEPBankingApp.Models
{
    public class BankAccount
    {

        //[Required]
        public string UserId { get; set; }

        [Key]
        [Required]
        public int AccountNumber { get; set; }

        [Required]
        public string AccountType { get; set; }

        //[Required]
        [Display(Name = "Balance")]
        public double CurrentBalance { get; set; }

        public bool BlockAccess { get; set; }

    }

    public class TransactionHistory
    {
        [Key]
        public int TransactionId { get; set; }

        [Display(Name = "Account Number")]
        public int AccountNumber { get; set; }

        [Display(Name = "Account Number")]
        public int DestinationAccountNumber { get; set; }

        public DateTime TransactionDateTime { get; set; }

        public double TransactionAmount { get; set; }

        public double PreBalance { get; set; }

        public double PostBalance { get; set; }

        public string Description { get; set; }

    }
}