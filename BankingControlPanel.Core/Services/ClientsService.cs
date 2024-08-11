using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs.Clients;
using BankingControlPanel.Core.Models.Entities;
using BankingControlPanel.Core.Models.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Services
{
    public class ClientsService : IClientsService
    {
        private readonly IClientsRepository _clientsRepository;
        private readonly IAdminService _adminService;
        private readonly ILogger<ClientsService> _logger;

        public ClientsService(IClientsRepository clientsRepository, IAdminService adminService, ILogger<ClientsService> logger)
        {
            _clientsRepository = clientsRepository;
            _adminService = adminService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated, sorted, and filtered list of clients.
        /// </summary>
        /// <param name="queryParameters">The query parameters for filtering, sorting, and pagination.</param>
        /// <returns>A paginated list of clients.</returns>
        public async Task<PagedResult<ClientDto>> ListClientsAsync(ClientsQueryParameters queryParameters)
        {
            try
            {
                _logger.LogInformation("Listing clients with given query parameters.");

                // Save search parameters for admin
                await _adminService.SaveSearchParametersAsync(queryParameters);

                // Retrieve total count and clients from repository
                var totalItems = await _clientsRepository.GetTotalClientsAsync(queryParameters);
                var clients = await _clientsRepository.GetClientsAsync(queryParameters);

                // Map the Client entities to ClientDto
                var clientDtos = clients.Select(client => new ClientDto
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    PersonalId = client.PersonalId,
                    ProfilePhoto = client.ProfilePhoto,
                    MobileNumber = client.MobileNumber,
                    Sex = client.Sex,
                    Address = new AddressDto
                    {
                        Country = client.Address.Country,
                        City = client.Address.City,
                        Street = client.Address.Street,
                        ZipCode = client.Address.ZipCode
                    },
                    Accounts = client.Accounts.Select(a => new AccountDto
                    {
                        Id = a.Id,
                        AccountNumber = a.AccountNumber,
                        AccountType = a.AccountType,
                        Balance = a.Balance
                    }).ToList()
                }).ToList();

                // Create PagedResult to return
                return new PagedResult<ClientDto>(clientDtos, totalItems, queryParameters.PageNumber, queryParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while listing clients.");
                throw;
            }
        }


        /// <summary>
        /// Retrieves a specific client by ID.
        /// </summary>
        /// <param name="clientId">The ID of the client to retrieve.</param>
        /// <returns>The client details.</returns>
        public async Task<ClientDto> GetClientByIdAsync(int clientId)
        {
            try
            {
                // Validate the clientId
                if (clientId <= 0)
                {
                    _logger.LogWarning("Invalid client ID {ClientId} provided.", clientId);
                    throw new ValidationException("Client ID must be a positive integer.");
                }

                _logger.LogInformation("Retrieving client with ID {ClientId}.", clientId);

                // Retrieve the client entity from the repository
                var client = await _clientsRepository.GetClientByIdAsync(clientId);
                if (client == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found.", clientId);
                    return null;
                }

                // Map the Client entity to ClientDto
                return new ClientDto
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    PersonalId = client.PersonalId,
                    ProfilePhoto = client.ProfilePhoto,
                    MobileNumber = client.MobileNumber,
                    Sex = client.Sex,
                    Address = new AddressDto
                    {
                        Country = client.Address.Country,
                        City = client.Address.City,
                        Street = client.Address.Street,
                        ZipCode = client.Address.ZipCode
                    },
                    Accounts = client.Accounts.Select(a => new AccountDto
                    {
                        Id = a.Id,
                        AccountNumber = a.AccountNumber,
                        AccountType = a.AccountType,
                        Balance = a.Balance
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving client.");
                throw;
            }
        }


        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="createClientDto">The details of the client to create.</param>
        /// <returns>The created client.</returns>
        public async Task<ClientDto> CreateClientAsync(CreateClientDto createClientDto)
        {
            try
            {
                // Validate the DTO
                ValidateDto(createClientDto);

                _logger.LogInformation("Creating a new client.");

                var client = new Client
                {
                    Email = createClientDto.Email,
                    FirstName = createClientDto.FirstName,
                    LastName = createClientDto.LastName,
                    PersonalId = createClientDto.PersonalId,
                    ProfilePhoto = createClientDto.ProfilePhoto,
                    MobileNumber = createClientDto.MobileNumber,
                    Sex = createClientDto.Sex,
                    Address = new Address
                    {
                        Country = createClientDto.Address.Country,
                        City = createClientDto.Address.City,
                        Street = createClientDto.Address.Street,
                        ZipCode = createClientDto.Address.ZipCode
                    },
                    Accounts = createClientDto.Accounts.Select(a => new Account
                    {
                        AccountNumber = a.AccountNumber,
                        AccountType = a.AccountType,
                        Balance = a.Balance
                    }).ToList()
                };

                await _clientsRepository.AddAsync(client);

                return new ClientDto
                {
                    Id = client.Id,
                    Email = client.Email,
                    FirstName = client.FirstName,
                    LastName = client.LastName,
                    PersonalId = client.PersonalId,
                    ProfilePhoto = client.ProfilePhoto,
                    MobileNumber = client.MobileNumber,
                    Sex = client.Sex,
                    Address = new AddressDto
                    {
                        Country = client.Address.Country,
                        City = client.Address.City,
                        Street = client.Address.Street,
                        ZipCode = client.Address.ZipCode
                    },
                    Accounts = client.Accounts.Select(a => new AccountDto
                    {
                        Id = a.Id,
                        AccountNumber = a.AccountNumber,
                        AccountType = a.AccountType,
                        Balance = a.Balance
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new client.");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing client.
        /// </summary>
        /// <param name="updateClientDto">The updated client details, including the client ID.</param>
        /// <returns>The updated client.</returns>
        public async Task<ClientDto> UpdateClientAsync(UpdateClientDto updateClientDto)
        {
            try
            {
                // Validate the DTO
                ValidateDto(updateClientDto);

                var clientId = updateClientDto.ClientId;

                _logger.LogInformation("Updating client with ID {ClientId}.", clientId);

                // Retrieve the existing client from the repository
                var existingClient = await _clientsRepository.GetClientByIdAsync(clientId);
                if (existingClient == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found.", clientId);
                    throw new Exception("Client not found.");
                }

                // Update the client's properties
                existingClient.Email = updateClientDto.Email;
                existingClient.FirstName = updateClientDto.FirstName;
                existingClient.LastName = updateClientDto.LastName;
                existingClient.PersonalId = updateClientDto.PersonalId;
                existingClient.ProfilePhoto = updateClientDto.ProfilePhoto;
                existingClient.MobileNumber = updateClientDto.MobileNumber;
                existingClient.Sex = updateClientDto.Sex;

                if (updateClientDto.Address != null)
                {
                    existingClient.Address.Country = updateClientDto.Address.Country;
                    existingClient.Address.City = updateClientDto.Address.City;
                    existingClient.Address.Street = updateClientDto.Address.Street;
                    existingClient.Address.ZipCode = updateClientDto.Address.ZipCode;
                }

                // Update the client's accounts
                if (updateClientDto.Accounts != null && updateClientDto.Accounts.Any())
                {
                    foreach (var accountDto in updateClientDto.Accounts)
                    {
                        var account = existingClient.Accounts.FirstOrDefault(a => a.Id == accountDto.Id);
                        if (account != null)
                        {
                            account.AccountNumber = accountDto.AccountNumber;
                            account.AccountType = accountDto.AccountType;
                            account.Balance = accountDto.Balance;
                        }
                    }
                }

                // Save the updated client back to the repository
                await _clientsRepository.UpdateAsync(existingClient);

                // Map the updated client back to a DTO
                return new ClientDto
                {
                    Id = existingClient.Id,
                    Email = existingClient.Email,
                    FirstName = existingClient.FirstName,
                    LastName = existingClient.LastName,
                    PersonalId = existingClient.PersonalId,
                    ProfilePhoto = existingClient.ProfilePhoto,
                    MobileNumber = existingClient.MobileNumber,
                    Sex = existingClient.Sex,
                    Address = new AddressDto
                    {
                        Country = existingClient.Address.Country,
                        City = existingClient.Address.City,
                        Street = existingClient.Address.Street,
                        ZipCode = existingClient.Address.ZipCode
                    },
                    Accounts = existingClient.Accounts.Select(a => new AccountDto
                    {
                        Id = a.Id,
                        AccountNumber = a.AccountNumber,
                        AccountType = a.AccountType,
                        Balance = a.Balance
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating client.");
                throw;
            }
        }


        /// <summary>
        /// Deletes a client by ID.
        /// </summary>
        /// <param name="deleteClientDto">The DTO containing the ID of the client to delete.</param>
        /// <returns>True if the client was deleted, otherwise false.</returns>
        public async Task<bool> DeleteClientAsync(DeleteClientDto deleteClientDto)
        {
            try
            {
                // Validate the DTO
                ValidateDto(deleteClientDto);

                var clientId = deleteClientDto.ClientId;

                _logger.LogInformation("Deleting client with ID {ClientId}.", clientId);

                // Retrieve the existing client from the repository
                var existingClient = await _clientsRepository.GetClientByIdAsync(clientId);
                if (existingClient == null)
                {
                    _logger.LogWarning("Client with ID {ClientId} not found.", clientId);
                    throw new Exception("Client not found.");
                }

                // Delete the client
                await _clientsRepository.DeleteAsync(clientId);

                _logger.LogInformation("Client with ID {ClientId} has been deleted.", clientId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting client.");
                throw;
            }
        }

        /// <summary>
        /// Validates the given DTO using data annotations.
        /// </summary>
        /// <param name="dto">The DTO to validate.</param>
        /// <exception cref="ValidationException">Thrown if validation fails.</exception>
        private void ValidateDto(object dto)
        {
            var validationContext = new ValidationContext(dto, null, null);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                throw new ValidationException($"Validation failed: {errors}");
            }
        }
    }
}
