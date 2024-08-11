using Asp.Versioning;
using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingControlPanel.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)] // Internal Server Error
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user with a specified role (Admin only).
        /// </summary>
        /// <param name="registerDto">The registration details.</param>
        /// <returns>The authentication response containing the JWT token.</returns>
        [HttpPost("admin/register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("Admin {Admin} registered a new user {User}.", User.Identity.Name, registerDto.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering a new user.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Register a new user with the default role "User".
        /// </summary>
        /// <param name="registerDto">The registration details.</param>
        /// <returns>The authentication response containing the JWT token.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> RegisterUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            registerDto.Role = "User"; // Force the role to "User"
            try
            {
                var result = await _authService.RegisterAsync(registerDto);
                _logger.LogInformation("User {User} registered successfully.", registerDto.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering a new user.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Log in a user and generate a JWT token.
        /// </summary>
        /// <param name="loginDto">The login details.</param>
        /// <returns>The authentication response containing the JWT token.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Unauthorized
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.LoginAsync(loginDto);
                _logger.LogInformation("User {User} logged in successfully.", loginDto.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging in the user.");
                return Unauthorized(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Get a list of all users (Admin only).
        /// </summary>
        /// <returns>A list of users.</returns>
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var result = await _authService.GetUsersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Update user information (Admin only).
        /// </summary>
        /// <param name="updateUserDto">The user details to update.</param>
        /// <returns>The updated user details.</returns>
        [HttpPut("users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.UpdateUserAsync(updateUserDto);
                _logger.LogInformation("Admin {Admin} updated user {User}.", User.Identity.Name, updateUserDto.UserName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the user.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a user (Admin only).
        /// </summary>
        /// <param name="deleteUserDto">The details of the user to delete.</param>
        /// <returns>True if the user was deleted successfully, otherwise false.</returns>
        [HttpDelete("users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto deleteUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.DeleteUserAsync(deleteUserDto);
                _logger.LogInformation("Admin {Admin} deleted user {UserId}.", User.Identity.Name, deleteUserDto.UserId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the user.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        #region Roles

        /// <summary>
        /// Get a list of all roles (Admin only).
        /// </summary>
        /// <returns>A list of roles.</returns>
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> GetRoles()
        {
            try
            {
                var result = await _authService.GetRolesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roles.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Add a new role (Admin only).
        /// </summary>
        /// <param name="addRoleDto">The role details to add.</param>
        /// <returns>The added role details.</returns>
        [HttpPost("roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> AddRole([FromBody] AddRoleDto addRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.AddRoleAsync(addRoleDto);
                _logger.LogInformation("Admin {Admin} added new role {Role}.", User.Identity.Name, addRoleDto.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding the role.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Update a role (Admin only).
        /// </summary>
        /// <param name="updateRoleDto">The role details to update.</param>
        /// <returns>The updated role details.</returns>
        [HttpPut("roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.UpdateRoleAsync(updateRoleDto);
                _logger.LogInformation("Admin {Admin} updated role {Role}.", User.Identity.Name, updateRoleDto.Name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the role.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a role (Admin only).
        /// </summary>
        /// <param name="deleteRoleDto">The details of the role to delete.</param>
        /// <returns>True if the role was deleted successfully, otherwise false.</returns>
        [HttpDelete("roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Bad Request
        public async Task<IActionResult> DeleteRole([FromBody] DeleteRoleDto deleteRoleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.DeleteRoleAsync(deleteRoleDto);
                if (!result)
                {
                    return NotFound(new { Message = "Role not found" });
                }
                _logger.LogInformation("Admin {Admin} deleted role {RoleId}.", User.Identity.Name, deleteRoleDto.RoleId);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the role.");
                return BadRequest(new { Message = ex.Message });
            }
        }

        #endregion
    }
}
