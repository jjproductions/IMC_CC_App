﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using IMC_CC_App.DTO;
using IMC_CC_App.Models;

namespace IMC_CC_App.DTO
{
    public class ExpenseDTO
    {
        public ExpenseDTO()
        {
            Status = new();
            Expenses = [];
        }

        [Required]
        public CommonDTO Status { get; set; }
        public List<Expense> Expenses { get; set; }
    }


    //Used for displaying activity transactions - Staff
    public class Expense
    {
        public Expense()
        {
            CardInfo = new();
        }

        public int Id { get; set; }

        [Required]
        public string TransactionDate { get; set; }

        [Required]
        public string PostDate { get; set; }

        // public DateTimeOffset TransactionDate1 { get; set; }
        public required double Amount { get; set; }

        public required string Description { get; set; }

        public int CardNumber { get; set; }

        public required string Category { get; set; }

        public required string Type { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public string? Memo { get; set; }

        public int? ReportID { get; set; }

        public CreditCardDTO? CardInfo { get; set; }

        public string? ReceiptUrl { get; set; }


    }

    //Used for uploading bank statements - Finance
    public class ExpenseRequest
    {
        public required double Amount { get; set; }
        public int CardNumber { get; set; }

        public required DateTimeOffset TransactionDate { get; set; }

        public required DateTimeOffset PostDate { get; set; }

        public required string Category { get; set; }

        public required string Description { get; set; }

        public required string Type { get; set; }

        public string? Memo { get; set; }
    }

    //Used for requesting transaction activity - Staff
    public class StatementRequest
    {
        public int NumOfStatements { get; set; } = -1;

        public int? Limit { get; set; }

        public int StatementID { get; set; }

        public string? Email { get; set; }

        public int? CardId { get; set; }

        public bool? getAllStatements { get; set; }
    }

    public class StatementUpdateRequestDTO
    {
        public required int CardNumber { get; set; }
        public string? ReportMemo { get; set; }
        public required string ReportName { get; set; }
        public int? ReportId { get; set; }
        public int[]? ItemsToDelete { get; set; }
        public StatusCategory? Status { get; set; } = StatusCategory.PENDING;
        public required List<StatementUpdateRequest> Statements { get; set; }
        public required bool SendNotification { get; set; }
    }

    // Used for updating statements within a report
    public class StatementUpdateRequest
    {
        public required int Id { get; set; }

        public int ReportId { get; set; }

        public required string Category { get; set; }

        public required string Description { get; set; }

        public required string Type { get; set; }

        public string? Memo { get; set; }

        public string? ReceiptUrl { get; set; }
    }

}
