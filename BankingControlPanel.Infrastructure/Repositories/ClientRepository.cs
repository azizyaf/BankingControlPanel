using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs;
using BankingControlPanel.Core.Models.Entities;
using BankingControlPanel.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(ApplicationDbContext context, ILogger<ClientRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of clients based on the specified query parameters, including filtering and paging.
        /// </summary>
        /// <param name="parameters">The query parameters for filtering and paging.</param>
        /// <returns>A list of clients.</returns>
        public async Task<IEnumerable<Client>> GetClientsAsync(ClientsQueryParameters parameters)
        {
            try
            {
                IQueryable<Client> query = _context.Clients.AsQueryable();

                // Filter by FirstName if provided
                if (!string.IsNullOrEmpty(parameters.FirstName))
                {
                    query = query.Where(c => c.FirstName.Contains(parameters.FirstName));
                }

                // Filter by LastName if provided
                if (!string.IsNullOrEmpty(parameters.LastName))
                {
                    query = query.Where(c => c.LastName.Contains(parameters.LastName));
                }

                // Filter by Email if provided
                if (!string.IsNullOrEmpty(parameters.Email))
                {
                    query = query.Where(c => c.Email.Contains(parameters.Email));
                }

                // Implement paging
                var skip = (parameters.PageNumber - 1) * parameters.PageSize;
                query = query.Skip(skip).Take(parameters.PageSize);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the list of clients.");
                throw new Exception("An error occurred while retrieving the list of clients.", ex);
            }
        }

        /// <summary>
        /// Retrieves a client by its unique identifier.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client.</param>
        /// <returns>The client if found; otherwise, null.</returns>
        public async Task<Client> GetClientByIdAsync(int clientId)
        {
            try
            {
                return await _context.Clients.FindAsync(clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving the client with ID {clientId}.");
                throw new Exception("An error occurred while retrieving the client by ID.", ex);
            }
        }

        /// <summary>
        /// Adds a new client to the database.
        /// </summary>
        /// <param name="client">The client to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task AddAsync(Client client)
        {
            try
            {
                await _context.Clients.AddAsync(client);
                await _context.SaveChangesAsync();
                _logger.LogInformation("A new client has been added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a new client.");
                throw new Exception("An error occurred while adding a new client.", ex);
            }
        }

        /// <summary>
        /// Updates an existing client in the database.
        /// </summary>
        /// <param name="client">The client to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UpdateAsync(Client client)
        {
            try
            {
                _context.Clients.Update(client);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Client with ID {ClientId} has been updated successfully.", client.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while updating the client with ID {client.Id}.");
                throw new Exception("An error occurred while updating the client.", ex);
            }
        }

        /// <summary>
        /// Deletes a client from the database.
        /// </summary>
        /// <param name="clientId">The unique identifier of the client to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task DeleteAsync(int clientId)
        {
            try
            {
                var client = await _context.Clients.FindAsync(clientId);
                if (client == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found.", clientId);
                    throw new Exception("Client not found.");
                }

                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Client with ID {ClientId} has been deleted successfully.", clientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the client with ID {clientId}.");
                throw new Exception("An error occurred while deleting the client.", ex);
            }
        }
    }
}
