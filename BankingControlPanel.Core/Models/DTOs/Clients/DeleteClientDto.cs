using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Clients
{
    public class DeleteClientDto
    {
        [Required(ErrorMessage = "Client ID is required.")]
        public int ClientId { get; set; }
    }
}
