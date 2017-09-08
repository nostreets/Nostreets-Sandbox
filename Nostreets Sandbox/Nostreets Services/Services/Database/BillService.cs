using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Requests;
using System;
using System.Collections.Generic;


namespace Nostreets_Services.Services.Database
{
    public class BillService
    {
        public BillService()
        {
            _connectionKey = "DefaultConnection";
        }

        public BillService(string connectionKey)
        {
            _connectionKey = connectionKey;
        }


        private string _connectionKey;
        private EFDBService<Expenses> _expenseSrv = new EFDBService<Expenses>();
        private EFDBService<Income> _incomeSrv = new EFDBService<Income>();

        public List<Expenses> GetAllExpenses(string userId)
        {
            _expenseSrv.Get()
        }

        public List<Income> GetAllIncome(string userId)
        {

        }

        public Expenses GetExpense(string userId)
        {

        }

        public Income GetIncome(string userId)
        {

        }

        public Chart<List<int>> GetIncomeChart(string userId, DateTime startDate, DateTime endDate)
        {

        }

        public Chart<List<int>> GetExpensesChart(string userId, DateTime startDate, DateTime endDate)
        {

        }


        public Chart<List<int>> GetCombinedChart(string userId, DateTime startDate, DateTime endDate)
        {

        }

        public void InsertExpense(string userId, ExpensesAddRequest request)
        {

        }

        public void InsertIncome(string userId, ExpensesAddRequest request)
        {

        }

        public void UpdateExpense(string userId, ExpensesUpdateRequest request)
        {

        }

        public void UpdateIncome(string userId, ExpensesUpdateRequest request)
        {

        }

        public void DeleteExpense(string userId, int id)
        {

        }

        public void DeleteIncome(string userId, int id)
        {

        }

    }
}
