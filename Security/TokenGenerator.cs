using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IMC_CC_App.Security
{
    public static class TokenGenerator
    {
        //private readonly IConfiguration _config;
        public static string GenerateToken(string email, IConfiguration _config)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AuthKey").ToString())); //"5014c26ef532i39e8b648fbf8555f0e7c93e1a7cde9e12192543aa1720947331"));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, email)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = tokenHandler.WriteToken(token);
            //var response = "{'token':'" + jwtToken + "'}";

            return jwtToken;
            // return Results.Ok(new { Token = jwtToken });
            // return (Results.Ok()"{'Token': '" + jwtToken + "'}" ));
        }
    }



}