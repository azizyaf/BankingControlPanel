using BankingControlPanel.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Clients
{
    public class UpdateClientDto
    {
        [Required(ErrorMessage = "Client ID is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Personal ID is required.")]
        [StringLength(11, ErrorMessage = "Personal ID must be exactly 11 characters.")]
        public string PersonalId { get; set; }

        public string ProfilePhoto { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        public ClientGender Sex { get; set; }

        public UpdateAddressDto Address { get; set; }

        public List<UpdateAccountDto> Accounts { get; set; }
    }
}
