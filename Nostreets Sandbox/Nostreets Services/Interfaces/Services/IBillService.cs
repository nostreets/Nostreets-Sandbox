using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IBillService
    {
        List<Expenses> GetAllExpenses(string userId);

        List<Income> GetAllIncome(string userId);

        Expenses GetExpense(string userId, Expenses expense);

        Income GetIncome(string userId, Income income);

        Chart<List<decimal>> GetIncomeChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null);

        Chart<List<decimal>> GetExpensesChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null);

        Chart<List<decimal>> GetCombinedChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null);

        void InsertExpense(Expenses request);

        void InsertIncome(Income request);

        void UpdateExpense(Expenses request);

        void UpdateIncome(Income request);

        void DeleteExpense(string userId, int id);

        void DeleteIncome(string userId, int id);
    }
}
