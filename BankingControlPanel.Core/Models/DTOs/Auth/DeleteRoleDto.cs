using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Auth
{
    public class DeleteRoleDto
    {
        [Required(ErrorMessage = "Role ID is required.")]
        public string RoleId { get; set; }
    }
}
