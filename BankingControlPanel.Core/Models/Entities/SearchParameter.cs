using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Models.Entities
{
    // Represents the search parameters used by an admin.
    public class SearchParameter
    {
        public int Id { get; set; }
        public string SearchCriteria { get; set; } // Serialized ClientsQueryParameters
        public DateTime SearchDate { get; set; }
        public string AdminId { get; set; }
        public ApplicationUser Admin { get; set; }
    }
}
