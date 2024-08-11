using BankingControlPanel.Core.Models.DTOs.Clients;
using BankingControlPanel.Core.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Interfaces
{
    public interface IClientsService
    {
        Task<PagedResult<ClientDto>> ListClientsAsync(ClientsQueryParameters queryParameters);
        Task<ClientDto> GetClientByIdAsync(int clientId);
        Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto);
        Task<ClientDto> UpdateClientAsync(UpdateClientDto updateClientDto);
        Task<bool> DeleteClientAsync(DeleteClientDto deleteClientDto);

    }
}
