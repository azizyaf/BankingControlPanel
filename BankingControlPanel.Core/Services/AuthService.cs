using BankingControlPanel.Core.Interfaces;
using BankingControlPanel.Core.Models.DTOs.Auth;
using BankingControlPanel.Core.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Registers a new user with the specified role.
        /// </summary>
        /// <param name="registerDto">The registration details.</param>
        /// <returns>The authentication response containing the JWT token.</returns>
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            ValidateDto(registerDto);

            try
            {
                var user = new ApplicationUser { UserName = registerDto.UserName, Email = registerDto.Email };
                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, registerDto.Role);
                    var token = GenerateJwtToken(user, registerDto.Role);
                    _logger.LogInformation("User {UserName} registered successfully.", registerDto.UserName);
                    return new AuthResponseDto
                    {
                        Token = token,
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = registerDto.Role
                    };
                }

                throw new Exception($"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering the user.");
                throw;
            }
        }

        /// <summary>
        /// Logs in a user and generates a JWT token.
        /// </summary>
        /// <param name="loginDto">The login details.</param>
        /// <returns>The authentication response containing the JWT token.</returns>
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            ValidateDto(loginDto);

            try
            {
                var result = await _signInManager.PasswordSignInAsync(loginDto.UserName, loginDto.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByNameAsync(loginDto.UserName);
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = GenerateJwtToken(user, roles.FirstOrDefault());
                    _logger.LogInformation("User {UserName} logged in successfully.", loginDto.UserName);
                    return new AuthResponseDto
                    {
                        Token = token,
                        UserName = user.UserName,
                        Email = user.Email,
                        Role = roles.FirstOrDefault()
                    };
                }

                throw new Exception("Login failed: Invalid username or password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while logging in the user.");
                throw;
            }
        }

        /// <summary>
        /// Gets the current user based on the current HTTP context.
        /// </summary>
        /// <returns>The current ApplicationUser if found; otherwise, null.</returns>
        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            try
            {
                // Retrieve the username from the current HTTP context
                var userName = _userManager.GetUserId(_httpContextAccessor.HttpContext.User);

                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("Username not found in the current context.");
                    return null;
                }

                // Retrieve the user from the database using the username
                var user = await _userManager.FindByNameAsync(userName);

                if (user == null)
                {
                    _logger.LogWarning($"User with username {userName} not found in the database.");
                    return null;
                }

                _logger.LogInformation($"User with username {userName} retrieved successfully.");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the current user.");
                throw new Exception("An error occurred while retrieving the current user.", ex);
            }
        }


        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of users.</returns>
        public async Task<IEnumerable<ApplicationUserDto>> GetUsersAsync()
        {
            try
            {
                var users = _userManager.Users.ToList();
                var userDtos = new List<ApplicationUserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    userDtos.Add(new ApplicationUserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles
                    });
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving users.");
                throw;
            }
        }

        /// <summary>
        /// Updates user details.
        /// </summary>
        /// <param name="updateUserDto">The user details to update.</param>
        /// <returns>The updated user details.</returns>
        public async Task<ApplicationUserDto> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            ValidateDto(updateUserDto);

            try
            {
                var user = await _userManager.FindByIdAsync(updateUserDto.Id);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                user.UserName = updateUserDto.UserName;
                user.Email = updateUserDto.Email;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    _logger.LogInformation("User {UserName} updated successfully.", updateUserDto.UserName);
                    return new ApplicationUserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Roles = roles
                    };
                }

                throw new Exception($"Update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a user by ID.
        /// </summary>
        /// <param name="deleteUserDto">The ID of the user to delete.</param>
        /// <returns>True if the user was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteUserAsync(DeleteUserDto deleteUserDto)
        {
            ValidateDto(deleteUserDto);

            try
            {
                var user = await _userManager.FindByIdAsync(deleteUserDto.UserId);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User {UserId} deleted successfully.", deleteUserDto.UserId);
                    return true;
                }

                throw new Exception($"Delete failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user.");
                throw;
            }
        }

        #region Roles

        /// <summary>
        /// Retrieves all roles.
        /// </summary>
        /// <returns>A list of roles.</returns>
        public async Task<IEnumerable<IdentityRoleDto>> GetRolesAsync()
        {
            try
            {
                var roles = _roleManager.Roles.ToList();
                var roleDtos = roles.Select(role => new IdentityRoleDto
                {
                    Id = role.Id,
                    Name = role.Name
                });

                return roleDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving roles.");
                throw;
            }
        }

        /// <summary>
        /// Retrieves a role by ID.
        /// </summary>
        /// <param name="roleId">The ID of the role to retrieve.</param>
        /// <returns>The role details.</returns>
        public async Task<IdentityRoleDto> GetRoleByIdAsync(string roleId)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role == null)
                {
                    return null;
                }

                _logger.LogInformation("Role {RoleId} retrieved successfully.", roleId);
                return new IdentityRoleDto
                {
                    Id = role.Id,
                    Name = role.Name
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the role.");
                throw;
            }
        }

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <param name="addRoleDto">The role details to add.</param>
        /// <returns>The added role details.</returns>
        public async Task<IdentityRoleDto> AddRoleAsync(AddRoleDto addRoleDto)
        {
            ValidateDto(addRoleDto);

            try
            {
                var role = new IdentityRole(addRoleDto.Name);
                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} added successfully.", addRoleDto.Name);
                    return new IdentityRoleDto
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                }

                throw new Exception($"Add role failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the role.");
                throw;
            }
        }

        /// <summary>
        /// Updates a role.
        /// </summary>
        /// <param name="updateRoleDto">The role details to update.</param>
        /// <returns>The updated role details.</returns>
        public async Task<IdentityRoleDto> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            ValidateDto(updateRoleDto);

            try
            {
                var role = await _roleManager.FindByIdAsync(updateRoleDto.Id);
                if (role == null)
                {
                    throw new Exception("Role not found.");
                }

                role.Name = updateRoleDto.Name;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {RoleName} updated successfully.", updateRoleDto.Name);
                    return new IdentityRoleDto
                    {
                        Id = role.Id,
                        Name = role.Name
                    };
                }

                throw new Exception($"Update failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the role.");
                throw;
            }
        }

        /// <summary>
        /// Deletes a role by ID.
        /// </summary>
        /// <param name="deleteRoleDto">The ID of the role to delete.</param>
        /// <returns>True if the role was deleted successfully, otherwise false.</returns>
        public async Task<bool> DeleteRoleAsync(DeleteRoleDto deleteRoleDto)
        {
            ValidateDto(deleteRoleDto);

            try
            {
                var role = await _roleManager.FindByIdAsync(deleteRoleDto.RoleId);
                if (role == null)
                {
                    throw new Exception("Role not found.");
                }

                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Role {RoleId} deleted successfully.", deleteRoleDto.RoleId);
                    return true;
                }

                throw new Exception($"Delete role failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the role.");
                throw;
            }
        }

        #endregion

        private string GenerateJwtToken(ApplicationUser user, string role)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


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
