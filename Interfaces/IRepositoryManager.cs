﻿namespace IMC_CC_App.Interfaces
{
    public interface IRepositoryManager
    {
        IStatement statementService { get; }
        IExpense expenseService { get; }
        IUser userService { get; }
        //IPermission permissionService { get; }
        IImage imageService { get; }
        IReport reportService { get; }

        void Save();
    }
}
