using _8Boys.DTOs;
using _8Boys.Models;
using Microsoft.AspNetCore.Identity;

namespace _8Boys.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO request);
        Task<AuthResponseDTO?> LoginAsync(LoginDTO request);
        Task LogoutAsync();
    }
}