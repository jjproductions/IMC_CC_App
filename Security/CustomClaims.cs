
using Microsoft.AspNetCore.Authentication;
using IMC_CC_App.DTO;
using System.Security.Claims;
using IMC_CC_App.Repositories;
using IMC_CC_App.Interfaces;
using ILogger = Serilog.ILogger;
using System.Xml.Serialization;

namespace IMC_CC_App.Security
{
    public class CustomClaimsTransformation : IClaimsTransformation
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _logger;

        public CustomClaimsTransformation(IRepositoryManager repositoryManager, ILogger logger)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
        }
        
        public async Task<ClaimsPrincipal?> TransformAsync(ClaimsPrincipal principal)
        {
            ClaimsIdentity newClaimsID = new();
            string? permission = null;
            if (principal.HasClaim(c => c.Type == CustomClaims.Role))
            {
                _logger.Warning("found claim: " + principal.Claims?.SingleOrDefault(x => x.Type == CustomClaims.Role)?.Value);
                return principal;
            }
            
            string? email = principal.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            _logger.Warning("email: " + email);

            if (!string.IsNullOrEmpty(email))
                return principal;
            else
            {
                permission = await _repositoryManager.permissionService.GetPermissions(email);
                Claim claim = new(CustomClaims.Role, permission);
                newClaimsID.AddClaim(claim);
                principal.AddIdentity(newClaimsID);
                _logger.Warning("new claim added: " + claim.Value);
            }

            return principal;
        }
    }
}