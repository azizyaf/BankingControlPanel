using BankingControlPanel.Core.Models.DTOs.Auth;
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
        Task<IEnumerable<ApplicationUserDto>> GetUsersAsync();
        Task<ApplicationUserDto> UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<IdentityRoleDto>> GetRolesAsync();
        Task<IdentityRoleDto> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<IdentityRoleDto> AddRoleAsync(AddRoleDto addRoleDto);
    }
}
