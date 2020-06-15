using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankAccounts.Models
{
    public class WithdrawDeposit
    {
        [Required]
        [Range(0, Int32.MaxValue)]
        public double Amount { get; set; }

        [Required]
        public string WithdrawSelect { get; set; }
    }
}