using BankingControlPanel.Core.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.DTOs.Clients
{
    public class ClientsQueryParameters
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PersonalId { get; set; }
        public string MobileNumber { get; set; }
        public ClientGender Sex { get; set; }
        public string SearchTerm { get; set; } // Optional search term for general filtering
        public string SortBy { get; set; } // Sorting field (e.g., "FirstName", "LastName", "Email")
        public bool SortDescending { get; set; } = false; // True for descending order, false for ascending
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
