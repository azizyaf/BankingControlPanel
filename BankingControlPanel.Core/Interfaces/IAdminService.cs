using BankingControlPanel.Core.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Interfaces
{
    public interface IAdminService
    {
        Task SaveSearchParametersAsync(ClientsQueryParameters queryParameters);
        Task<IEnumerable<ClientsQueryParameters>> GetLastSearchParametersAsync();
    }
}
