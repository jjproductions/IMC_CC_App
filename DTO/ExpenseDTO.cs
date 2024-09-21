using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;
using IMC_CC_App.DTO;

namespace IMC_CC_App.DTO
{
    public class ExpenseDTO
    {
        public ExpenseDTO()
        {
            Status = new();
            Expenses = new();
        }

        [Required]
        public CommonDTO Status { get; set; }
        public List<Expense> Expenses { get; set; }
    }


    public class Expense
    {
        public Expense()
        {
            CardInfo = new();
        }

        public int Id { get; set; }

        [Required]
        public DateTimeOffset TransactionDate { get; set; }

        [Required]
        public DateTimeOffset PostDate { get; set; }

        public required double Amount { get; set; }
        
        public required string Description { get; set; }

        public int? CardNumber { get; set; }

        public required string Category { get; set; }

        public required string Type { get; set; }

        public DateTimeOffset Created { get; set; } = DateTimeOffset.UtcNow;

        public string? Memo { get; set; }

        public int? ReportID { get; set; }

        public CreditCardDTO? CardInfo { get; set; }


    }

    public class ExpenseRequest
    {
        public int CardNumber { get; set; }

        public required DateTimeOffset TransactionDate { get; set; }
        
        public required DateTimeOffset PostDate { get; set; }

        public required double Amount { get; set; }

        public required string Description { get; set; }

        public required string Category { get; set; }

        public required string Type { get; set; }

        public string? Memo { get; set; }
    }


    public class StatementRequest
    {
        public int NumOfStatements { get; set; } = -1;

        public int? Limit { get; set; }

        public int StatementID { get; set; }
    }

}
