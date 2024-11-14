//using Azure.Core;
//using IMC_CC_App.DTO;
//using IMC_CC_App.Services;
//using Microsoft.AspNetCore.Authentication;
//using System.Net.Http.Headers;
//using System.Security.Claims;
//using System.Text;

//namespace IMC_CC_App.Security
//{
//    public class AuthenticationHandler
//    {
//        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
//        {
//            if (!Request.Headers.ContainsKey(CustomAuthenticationOptions.AuthorizationHeaderName))
//            {
//                return AuthenticateResult.Fail("Unauthorized");
//            }
//            var authenticationHeaderValue = Request.Headers[CustomAuthenticationOptions.AuthorizationHeaderName];
//            if (string.IsNullOrEmpty(authenticationHeaderValue))
//            {
//                return AuthenticateResult.NoResult();
//            }
//            User user;
//            try
//            {
//                var authenticationHeader = AuthenticationHeaderValue.Parse(authenticationHeaderValue);
//                var credentialBytes = Convert.FromBase64String(authenticationHeader.Parameter);
//                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
//                var username = credentials[0];
//                var password = credentials[1];
//                user = new User()
//                {
//                    Username = username,
//                    Password = password
//                };

//                user = await _userService.Authenticate(username, password);

//                if (user == null)
//                    return AuthenticateResult.Fail("Invalid Username or Password");
//            }
//            catch
//            {
//                return AuthenticateResult.Fail("Invalid Authorization Header");
//            }
//            var claims = new List<Claim>()
//    {
//        new Claim("Username", user.Username)
//    };
//            var claimsIdentity = new ClaimsIdentity(claims, Scheme.Name);
//            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
//            return AuthenticateResult.Success
//                (new AuthenticationTicket(claimsPrincipal,
//                this.Scheme.Name));
//        }
//    }
//}
