using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IBillService
    {
        List<Expense> GetAllExpenses(string userId);

        List<Income> GetAllIncome(string userId);

        Expense GetExpense(string userId, string expenseName);

        Income GetIncome(string userId, string incomeName);

        Chart<List<float>> GetIncomeChart(string userId, out ScheduleTypes chartSchuduleType, DateTime? startDate = null, DateTime? endDate = null, ChartLibraryType library = ChartLibraryType.Google);

        Chart<List<float>> GetExpensesChart(string userId, out ScheduleTypes chartSchuduleType, DateTime? startDate = null, DateTime? endDate = null, ChartLibraryType library = ChartLibraryType.Google);

        Chart<List<float>> GetCombinedChart(string userId, out ScheduleTypes chartSchuduleType, DateTime? startDate = null, DateTime? endDate = null, ChartLibraryType library = ChartLibraryType.Google);

        void InsertExpense(Expense request);

        void InsertIncome(Income request);

        void UpdateExpense(Expense request);

        void UpdateIncome(Income request);

        void DeleteExpense(int id);

        void DeleteIncome(int id);
    }
}
