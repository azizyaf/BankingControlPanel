using BankingControlPanel.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Clients
{
    public class CreateClientDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(60, ErrorMessage = "First name cannot exceed 60 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(60, ErrorMessage = "Last name cannot exceed 60 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Personal ID is required.")]
        [StringLength(11, ErrorMessage = "Personal ID must be exactly 11 characters.")]
        public string PersonalId { get; set; }

        public string ProfilePhoto { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+[1-9]\d{1,14}$", ErrorMessage = "Mobile number must be in international format.")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        public ClientGender Sex { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public AddressDto Address { get; set; }

        [Required(ErrorMessage = "At least one account is required.")]
        [MinLength(1, ErrorMessage = "At least one account is required.")]
        public List<CreateAccountDto> Accounts { get; set; }
    }
}
