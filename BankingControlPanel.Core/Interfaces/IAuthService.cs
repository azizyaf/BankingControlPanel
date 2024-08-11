using BankingControlPanel.Core.Models.DTOs.Auth;
using BankingControlPanel.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingControlPanel.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<ApplicationUser> GetCurrentUserAsync();
        Task<IEnumerable<ApplicationUserDto>> GetUsersAsync();
        Task<ApplicationUserDto> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(DeleteUserDto deleteUserDto);
        Task<IEnumerable<IdentityRoleDto>> GetRolesAsync();
        Task<IdentityRoleDto> GetRoleByIdAsync(string roleId);
        Task<IdentityRoleDto> AddRoleAsync(AddRoleDto addRoleDto);
        Task<IdentityRoleDto> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<bool> DeleteRoleAsync(DeleteRoleDto deleteRoleDto);
    }
}
