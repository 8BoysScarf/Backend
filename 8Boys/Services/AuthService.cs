using _8Boys.DTOs;
using _8Boys.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _8Boys.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponseDTO> RegisterAsync(RegisterDTO request)
        {
            // check if email exists
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                var existingByEmail = await _userManager.FindByEmailAsync(request.Email);
                if (existingByEmail != null)
                {
                    return new AuthResponseDTO { Token = string.Empty, ExpiresAt = DateTime.MinValue, UserId = string.Empty };
                }
            }

            // check if username exists (only if provided)
            var usernameToCheck = request.Username ?? request.Email;
            if (!string.IsNullOrWhiteSpace(usernameToCheck))
            {
                var existingByName = await _userManager.FindByNameAsync(usernameToCheck);
                if (existingByName != null)
                {
                    return new AuthResponseDTO { Token = string.Empty, ExpiresAt = DateTime.MinValue, UserId = string.Empty };
                }
            }

            var user = new ApplicationUser { UserName = usernameToCheck, Email = request.Email, Name = request.Name };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new AuthResponseDTO { Token = string.Empty, ExpiresAt = DateTime.MinValue, UserId = string.Empty };
            }

            // ensure the Customer role exists (best-effort)
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            // assign default role
            await _userManager.AddToRoleAsync(user, "Admin");

            // generate token
            var token = await GenerateTokenAsync(user);
            return token;
        }

        public async Task<AuthResponseDTO?> LoginAsync(LoginDTO request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded) return null;

            return await GenerateTokenAsync(user);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        private async Task<AuthResponseDTO> GenerateTokenAsync(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = jwtSettings.GetValue<string>("Key");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");
            var expiresMinutes = jwtSettings.GetValue<int>("ExpiresMinutes");

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResponseDTO
            {
                Token = tokenStr,
                ExpiresAt = expires,
                UserId = user.Id,
                Roles = roles
            };
        }
    }
}