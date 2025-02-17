using System.Net;
using Asp.Versioning.Builder;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using IMC_CC_App.Components;
using IMC_CC_App.DTO;
using IMC_CC_App.Interfaces;
using IMC_CC_App.Security;
using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;
using IMC_CC_App.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace IMC_CC_App.Routes
{

    public class SigninAPI : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly IAuthorizationService _authService;

        public SigninAPI(IRepositoryManager repositoryManager, ILogger logger, IConfiguration config, IAuthorizationService authService)
        {
            _repositoryManager = repositoryManager;
            _logger = logger;
            _config = config;
            _authService = authService;
        }

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
               .HasApiVersion(new Asp.Versioning.ApiVersion(1))
               .ReportApiVersions()
               .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/signin").WithApiVersionSet(apiVersionSet);

            groupBuilder.MapPost("/", ([FromBody] Login request) => Post(request))
                .RequireCors("AllowedOrigins")
                .AllowAnonymous();

            groupBuilder.MapPost("/sastoken", ([FromBody] SASTokenRefreshRequest tokenRequest, ClaimsPrincipal principal) => GenerateSASToken(tokenRequest, principal))
                .RequireCors("AllowedOrigins")
                .RequireAuthorization();

        }

        protected virtual async Task<rType> Post(Login request)
        {
            rType response = new();
            //TODO: Use Redis
            var userInfo = await _repositoryManager.userService.GetAuthUserAsync(request.email);
            int expiration = 30;
            if (_config.GetSection("TokenExpiration")?.Value?.ToString() != null)
                expiration = int.Parse(_config.GetSection("TokenExpiration")?.Value?.ToString());

            _logger.Warning($"Login: token expires in {expiration} minutes");

            //HttpResponseMessage msg;
            if (userInfo?.Users?.Count > 0)
            {
                response.access_token = TokenGenerator.GenerateToken(userInfo?.Users?[0], _config.GetSection("AuthKey")?.Value?.ToString(), expiration);
                response.role = userInfo?.Users?[0].RoleName;
                response.card_number = userInfo?.Users?[0].Card;
            }
            return response;

        }

        protected virtual async Task<string> GenerateSASToken(SASTokenRefreshRequest tokenRequest, ClaimsPrincipal principal)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            string sasToken = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(tokenRequest.BlobName))
                {
                    _logger.Error("GenerateSASToken: blobName is null");
                    return sasToken;
                }

                tokenRequest.ContainerName = string.IsNullOrEmpty(tokenRequest.ContainerName) ? _config["ImageContainerName"] : tokenRequest.ContainerName;
                // string? tokenExpiration = blobName.Split("?").Length > 1 ?
                //     SAS.CheckSASExpiration(blobName.Split("?")[1]) :
                //     null;

                if (true)
                {
                    var blobServiceClient = new BlobServiceClient(_config["ImageContainerConnectionString"]);
                    var blobContainerClient = blobServiceClient.GetBlobContainerClient(tokenRequest.ContainerName);
                    var blobClient = blobContainerClient.GetBlobClient(tokenRequest.BlobName);

                    if (!await blobClient.ExistsAsync())
                    {
                        throw new InvalidOperationException("Blob does not exist.");
                    }

                    BlobSasBuilder sasBuilder = new BlobSasBuilder()
                    {
                        BlobName = tokenRequest.BlobName,
                        Resource = "b", // 'b' for blob
                        ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(15)
                    };
                    sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    sasToken = blobClient.GenerateSasUri(sasBuilder).ToString();
                    Console.WriteLine($"Blob URL with SAS: {sasToken}");
                }
            }
            catch (RequestFailedException ex)
            {
                _logger.Error($"GenerateSASToken: {ex.Message}");
            }
            return sasToken;
        }
        public class rType
        {
            public string? access_token { get; set; }
            public string? role { get; set; }
            public int? card_number { get; set; }
        }
    }
}