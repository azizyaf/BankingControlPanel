using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Auth
{
    public class DeleteUserDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; }
    }
}
