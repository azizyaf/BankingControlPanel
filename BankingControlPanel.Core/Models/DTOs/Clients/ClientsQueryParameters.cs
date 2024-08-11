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
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PersonalId { get; set; }
        public string? MobileNumber { get; set; }
        public ClientGender? Sex { get; set; }
        public string? SearchTerm { get; set; } // Optional search term for general filtering
        public string? SortBy { get; set; } // Sorting field (e.g., "FirstName", "LastName", "Email")
        public bool SortDescending { get; set; } = false; // True for descending order, false for ascending

        // Fields for filtering by Address
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? ZipCode { get; set; }

        // Fields for filtering by Account
        public string? AccountNumber { get; set; }
        public string? AccountType { get; set; }
        public decimal? MinBalance { get; set; }
        public decimal? MaxBalance { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
