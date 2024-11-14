using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using IMC_CC_App.DTO;
using Microsoft.IdentityModel.Tokens;

namespace IMC_CC_App.Security
{
    public static class TokenGenerator
    {
        //private readonly IConfiguration _config;
        public static string GenerateToken(User userInfo, string? secretKey)
        {
            if (secretKey == null) throw new UnauthorizedAccessException("site is down: 101"); 
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)); //"5014c26ef532i39e8b648fbf8555f0e7c93e1a7cde9e12192543aa1720947331"));

           
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, userInfo?.Email),
                new Claim(ClaimTypes.Role, userInfo.RoleName)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            
            return jwtToken;
        }
    }



}