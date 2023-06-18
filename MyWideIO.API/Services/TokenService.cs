using Microsoft.IdentityModel.Tokens;
using MyWideIO.API.Models.DB_Models;
using MyWideIO.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MyWideIO.API.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(AppUserModel user, IEnumerable<string> roles)
        {
            List<Claim> claims = new()
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString()),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));



            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["Authentication:JwtKey"]));
            const string algorithm = SecurityAlgorithms.HmacSha256;

            SigningCredentials signingCredentials = new(key, algorithm);

            JwtHeader header = new(signingCredentials);

            JwtPayload payload = new(claims);

            JwtSecurityToken token = new(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
