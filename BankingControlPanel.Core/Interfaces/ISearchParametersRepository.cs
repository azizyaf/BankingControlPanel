using BankingControlPanel.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Interfaces
{
    public interface ISearchParametersRepository
    {
        Task AddSearchParameterAsync(SearchParameter searchParameter);
        Task<List<SearchParameter>> GetLastSearchParametersAsync(string adminId, int count);
    }
}
