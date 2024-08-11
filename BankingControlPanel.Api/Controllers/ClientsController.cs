using Asp.Versioning;
using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs.Clients;
using BankingControlPanel.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _clientsService;
        private readonly ILogger<ClientsController> _logger;
        private readonly IAdminService _adminService;

        public ClientsController(IClientsService clientsService, ILogger<ClientsController> logger, IAdminService adminService)
        {
            _clientsService = clientsService;
            _logger = logger;
            _adminService = adminService;
        }

        /// <summary>
        /// Lists clients with filtering, sorting, and paging.
        /// </summary>
        /// <param name="queryParameters">The parameters for filtering, sorting, and paging.</param>
        /// <returns>Returns a paginated list of clients.</returns>
        [HttpGet]
        public async Task<IActionResult> ListClients([FromQuery] ClientsQueryParameters queryParameters)
        {
            try
            {
                var clients = await _clientsService.ListClientsAsync(queryParameters);
                return Ok(clients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while listing clients.");
                return StatusCode(503, "Service unavailable. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves a specific client by ID.
        /// </summary>
        /// <param name="clientId">The ID of the client to retrieve.</param>
        /// <returns>Returns the client details if found.</returns>
        [HttpGet("{clientId}")]
        public async Task<IActionResult> GetClientById(int clientId)
        {
            if (clientId <= 0)
            {
                return BadRequest("Client ID must be a positive integer.");
            }

            try
            {
                var client = await _clientsService.GetClientByIdAsync(clientId);
                if (client == null)
                {
                    return NotFound($"Client with ID {clientId} not found.");
                }

                return Ok(client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the client.");
                return StatusCode(503, "Service unavailable. Please try again later.");
            }
        }

        /// <summary>
        /// Creates a new client.
        /// </summary>
        /// <param name="createClientDto">The details of the client to create.</param>
        /// <returns>Returns the created client with its ID.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientDto createClientDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateClientDto.");
                return BadRequest(ModelState);
            }

            try
            {
                var client = await _clientsService.CreateClientAsync(createClientDto);
                return CreatedAtAction(nameof(GetClientById), new { clientId = client.Id }, client);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating client.");
                return StatusCode(503, "Service unavailable. Please try again later.");
            }
        }

        /// <summary>
        /// Updates an existing client.
        /// </summary>
        /// <param name="updateClientDto">The updated client details.</param>
        /// <returns>Returns the updated client details.</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateClient([FromBody] UpdateClientDto updateClientDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateClientDto.");
                return BadRequest(ModelState);
            }

            try
            {
                var updatedClient = await _clientsService.UpdateClientAsync(updateClientDto);

                return Ok(updatedClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the client.");
                return StatusCode(503, "Service unavailable. Please try again later.");
            }
        }

        /// <summary>
        /// Deletes a client by ID.
        /// </summary>
        /// <param name="clientId">The ID of the client to delete.</param>
        /// <returns>Returns Ok with a confirmation message if the deletion was successful, or NotFound if the client was not found.</returns>
        [HttpDelete("{clientId}")]
        public async Task<IActionResult> DeleteClient(int clientId)
        {
            if (clientId <= 0)
            {
                return BadRequest("Client ID must be a positive integer.");
            }

            try
            {
                var result = await _clientsService.DeleteClientAsync(new DeleteClientDto { ClientId = clientId });
                if (!result)
                {
                    return NotFound($"Client with ID {clientId} not found.");
                }

                return Ok($"Client with ID {clientId} has been successfully deleted.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the client.");
                return StatusCode(503, "Service unavailable. Please try again later.");
            }
        }

        /// <summary>
        /// Retrieves the last 3 search parameters used by the current admin user.
        /// </summary>
        /// <returns>An IActionResult containing the last 3 ClientsQueryParameters.</returns>
        [HttpGet("last-searches")]
        public async Task<IActionResult> GetLastSearchParameters()
        {
            try
            {
                var lastSearches = await _adminService.GetLastSearchParametersAsync();

                if (lastSearches == null || !lastSearches.Any())
                {
                    _logger.LogWarning("No search parameters found for the current user.");
                    return NotFound("No search parameters found.");
                }

                return Ok(lastSearches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the last 3 search parameters.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
    }
}
