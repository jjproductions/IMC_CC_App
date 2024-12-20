﻿using IMC_CC_App.Components;
using IMC_CC_App.Interfaces;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;
using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Mvc;
using IMC_CC_App.Security;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Cors;
using System.Security.Claims;
using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Authorization;

namespace IMC_CC_App.Routes
{
    public class StatementsAPI(IRepositoryManager repositoryManager, ILogger logger, IAuthorizationService authService) : RouterBase
    {
        private readonly IRepositoryManager _repositoryManager = repositoryManager;
        private readonly ILogger _logger = logger;
        private readonly IAuthorizationService _authService = authService;

        public override void AddRoutes(WebApplication app)
        {
            ApiVersionSet apiVersionSet = app.NewApiVersionSet()
                .HasApiVersion(new Asp.Versioning.ApiVersion(1))
                .ReportApiVersions()
                .Build();

            RouteGroupBuilder groupBuilder = app.MapGroup("/api/v{apiVersion:apiversion}/statements")
            .WithApiVersionSet(apiVersionSet)
            .RequireCors("AllowedOrigins");

            groupBuilder.MapGet("/", (
                [FromQuery(Name = "id")] int? id,
                [FromQuery(Name = "getall")] bool? getAllStatements, 
                ClaimsPrincipal principal) => 
                Get(principal, id, getAllStatements ))
                .RequireAuthorization(); 
        }



        protected virtual async Task<ExpenseDTO> Get(ClaimsPrincipal principal, int? id, bool? getAllStatements=false)
        {
            var authResult = await _authService.AuthorizeAsync(principal, "User");
            _logger.Warning($"Get Statments - Auth claim: {principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value}...{authResult.Succeeded} :: id={id}");

            StatementRequest request = new();
            CancellationToken cancellationToken = CancellationToken.None;
            request.getAllStatements = getAllStatements;
            if (id == null)
                request.Email = principal.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            else
                request.CardId = id;
            ExpenseDTO? db_result = await _repositoryManager.statementService.GetStatementsAsync(request, cancellationToken);
            return db_result;
        }


        
    }
}
