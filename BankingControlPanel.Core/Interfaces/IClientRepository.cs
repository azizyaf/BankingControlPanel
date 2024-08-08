using BankingControlPanel.Core.Models.DTOs;
using BankingControlPanel.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetClientsAsync(ClientsQueryParameters parameters);
        Task<Client> GetClientByIdAsync(int clientId);
        Task AddAsync(Client client);
        Task UpdateAsync(Client client);
        Task DeleteAsync(int clientId);
    }
}
