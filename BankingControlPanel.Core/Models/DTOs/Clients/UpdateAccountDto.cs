using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Clients
{
    public class UpdateAccountDto
    {
        [Required(ErrorMessage = "Account ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Account number is required.")]
        public string AccountNumber { get; set; }

        [Required(ErrorMessage = "Account type is required.")]
        public string AccountType { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Balance must be a non-negative amount.")]
        public decimal Balance { get; set; }
    }
}
