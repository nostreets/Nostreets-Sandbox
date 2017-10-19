using NostreetsEntities;
using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NostreetsExtensions;

namespace Nostreets_Services.Services.Database
{
    public class BillService : IBillService
    {
        public BillService()
        {
            _connectionKey = "DefaultConnection";
            _incomeSrv = new EFDBService<Income>(_connectionKey);
            _expenseSrv = new EFDBService<Expenses>(_connectionKey);
        }

        public BillService(string connectionKey)
        {
            _connectionKey = connectionKey;
            _incomeSrv = new EFDBService<Income>(_connectionKey);
            _expenseSrv = new EFDBService<Expenses>(_connectionKey);
        }


        private string _connectionKey = null;
        private EFDBService<Expenses> _expenseSrv = null;
        private EFDBService<Income> _incomeSrv = null;

        public List<Expenses> GetAllExpenses(string userId)
        {
            List<Expenses> expenses = _expenseSrv.Where((a) => a.UserId == userId).ToList();
            expenses.Sort((a, b) => DateTime.Compare(a.DateModified.Value, b.DateModified.Value));
            return expenses;
        }

        public List<Income> GetAllIncome(string userId)
        {
            List<Income> incomes = _incomeSrv.Where((a) => a.UserId == userId).ToList();
            incomes.Sort((a, b) => DateTime.Compare(a.DateModified.Value, b.DateModified.Value));
            return incomes;
        }

        public Expenses GetExpense(string userId, int id)
        {
            return _expenseSrv.Get(a => a.UserId == userId && a.Id == id);    //.SingleOrDefault();
        }

        public Expenses GetExpense(string userId, string expenseName)
        {
            return _expenseSrv.Get(a => a.UserId == userId && a.Name == expenseName);     //.SingleOrDefault();
        }

        public List<Expenses> GetExpenses(string userId, ExpenseTypes type)
        {
            return _expenseSrv.Where((a) => a.UserId == userId && a.ExpenseType == type).ToList();
        }

        public List<Expenses> GetExpenses(string userId, ScheduleTypes type)
        {
            return _expenseSrv.Where((a) => a.UserId == userId && a.PaySchedule == type).ToList();
        }

        public List<Expenses> GetExpenses(string userId, ExpenseTypes billType, ScheduleTypes scheduleType)
        {
            return _expenseSrv.Where((a) => a.UserId == userId && a.ExpenseType == billType && a.PaySchedule == scheduleType).ToList();
        }

        public Income GetIncome(string userId, int id)
        {
            return _incomeSrv.Get(a => a.Id == id && a.UserId == userId);           //.Where((a) => a.UserId == userId && a.Id == id).SingleOrDefault();
        }

        public Income GetIncome(string userId, string incomeName)
        {
            return _incomeSrv.Get(a => a.UserId == userId && a.Name == incomeName); //.SingleOrDefault();
        }

        public List<Income> GetIncomes(string userId, IncomeTypes type)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.IncomeType == type).ToList();
        }

        public List<Income> GetIncomes(string userId, ScheduleTypes type)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.PaySchedule == type).ToList();
        }

        public List<Income> GetIncomes(string userId, IncomeTypes incomeType, ScheduleTypes scheduleType)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.IncomeType == incomeType && a.PaySchedule == scheduleType).ToList();
        }

        public Chart<List<float>> GetIncomeChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {
            Chart<List<float>> result = new Chart<List<float>>(); ;
            List<Income> income = GetAllIncome(userId);
            ScheduleTypes chartSchedule;

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.AddDays(14) : endDate.Value;

            TimeSpan diff = (end - start);

            int numOfAssetsSkipped = 0;

            result.Labels = CalculateLabelRange(out chartSchedule, start, end, preferedLabel);
            result.Name = String.Format("Income {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartTypes.Line;
            result.UserId = userId;

            if (result.Series == null) { result.Series = new List<List<float>>(); }
            if (result.Legend == null) { result.Legend = new List<string>(); }


            for (int n = 0; n < income.Count; n++)
            {
                if (income[n].IsHiddenOnChart) { numOfAssetsSkipped++; continue; }

                result.Series.Add(new List<float>());
                result.Legend.Add(income[n].Name);

                switch (chartSchedule)
                {

                    case ScheduleTypes.Hourly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            int day = 0;
                            day += ((i + 1) % 24 == 0) ? 1 : 0;

                            IsPayDay(start, i, chartSchedule, income[n], out float pay);
                            result.Series[n - numOfAssetsSkipped].Add(pay);

                            //result.Series[n].Add(income[n].Cost + (!IsPayDay(start, day, chartSchedule, income[n], out pay) ? pay : income[n].Cost));

                        }
                        break;

                    case ScheduleTypes.Daily:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {

                            IsPayDay(start, i, chartSchedule, income[n], out float pay);
                            result.Series[n - numOfAssetsSkipped].Add(pay);

                            //result.Series[n].Add(!IsPayDay(start, i, chartSchedule, income[n], out pay) ? pay : (income[n].PaySchedule == ScheduleTypes.Hourly) ? income[n].Cost * 24 : income[n].Cost);
                        }
                        break;

                    case ScheduleTypes.Weekly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add(income[n].Cost + (((income[n].TimePaid.Value.Month != start.Month + (i % 4 != 0 ? 0 : i / 4)) ? 0 : income[n].Cost)));
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(income[n].Cost + ((!i.IsWeekOfMonth(income[n].TimePaid.Value)) ? 0 : income[n].Cost));
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(income[n].Cost + (income[n].Cost));
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].Cost + (income[n].Cost * 7));
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(income[n].Cost + (income[n].Cost * 168));
                                    break;

                                default:
                                    DateTime beginningOfWeek = start.AddDays((i) * 7);
                                    DateTime endingOfWeek = start.AddDays((i + 1) * 7);

                                    result.Series[n].Add(
                                        (income[n].TimePaid.Value > beginningOfWeek && income[n].TimePaid.Value < endingOfWeek)
                                        ? income[n].Cost : 0);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Monthly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Yearly:
                                    result.Series[n].Add((income[n].TimePaid.Value.Month == start.Month + i) ? 0 : income[n].Cost);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((income[n].TimePaid.Value.Month != start.Month + i || income[n].TimePaid.Value.AddMonths(6).Month != start.Month + i) ? 0 : income[n].Cost);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(income[n].Cost);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(income[n].Cost * 4);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].Cost * DateTime.DaysInMonth(start.Year, start.Month + i));
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(income[n].Cost * 672);
                                    break;

                                default:
                                    result.Series[n].Add(income[n].TimePaid.Value.Month == start.Month + i && i + 1 < 12 ? 0 : income[n].Cost);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Yearly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Yearly:
                                    result.Series[n].Add(income[n].Cost);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add(income[n].Cost * 2);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(income[n].Cost * 12);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(income[n].Cost * 52);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].Cost * 365);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(income[n].Cost * 8064);
                                    break;

                                default:
                                    result.Series[n].Add(income[n].TimePaid.Value.Year != start.Year ? 0 : income[n].Cost);
                                    break;
                            }
                        }
                        break;
                }

            }


            return result;
        }

        public Chart<List<float>> GetExpensesChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();
            List<Expenses> expenses = GetAllExpenses(userId);
            ScheduleTypes chartSchedule;

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.AddDays(14) : endDate.Value;

            TimeSpan diff = (end - start);

            result.Labels = CalculateLabelRange(out chartSchedule, start, end, preferedLabel);
            result.Name = String.Format("Expenses {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartTypes.Line;
            result.UserId = userId;

            if (result.Series == null) { result.Series = new List<List<float>>(); }
            if (result.Legend == null) { result.Legend = new List<string>(); }

            for (int n = 0; n < expenses.Count; n++)
            {
                result.Series.Add(new List<float>());
                result.Legend.Add(expenses[n].Name);

                switch (chartSchedule)
                {

                    case ScheduleTypes.Hourly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {

                            IsPayDay(start, i, chartSchedule, expenses[n], out float price);

                            result.Series[n].Add(price);

                            //switch (expenses[n].PaySchedule)
                            //{
                            //    case ScheduleTypes.Weekly:
                            //        result.Series[n].Add((!IsPayDay(start, i, chartSchedule, expenses[n])) ? 0 : expenses[n].Cost);
                            //        break;

                            //    case ScheduleTypes.Daily:
                            //        result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, expenses[n]) ? 0 : expenses[n].Cost);
                            //        break;
                            //}

                        }
                        break;

                    case ScheduleTypes.Daily:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {

                            IsPayDay(start, i, chartSchedule, expenses[n], out float price);

                            result.Series[n].Add(price);

                            //DateTime intervalDate = start.AddDays(i);
                            //if (expenses[n].PaySchedule == ScheduleTypes.Hourly)
                            //{
                            //    result.Series[n].Add(expenses[n].Cost);
                            //}
                            //else
                            //{
                            //    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, expenses[n]) ? 0 : expenses[n].Cost);
                            //}
                        }
                        break;

                    case ScheduleTypes.Weekly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (expenses[n].PaySchedule)
                            {

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((expenses[n].TimePaid.Value.Month != start.Month + (i % 4 != 0 ? 0 : i / 4)) ? 0 : expenses[n].Cost);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add((!i.IsWeekOfMonth(expenses[n].TimePaid.Value)) ? 0 : expenses[n].Cost);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(expenses[n].Cost);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Cost * 7);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(expenses[n].Cost * 168);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Monthly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (expenses[n].PaySchedule)
                            {
                                case ScheduleTypes.Yearly:
                                    result.Series[n].Add((expenses[n].TimePaid.Value.Month == start.Month + i) ? 0 : expenses[n].Cost);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((expenses[n].TimePaid.Value.Month != start.Month + i || expenses[n].TimePaid.Value.AddMonths(6).Month != start.Month + i) ? 0 : expenses[n].Cost);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(expenses[n].Cost);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(expenses[n].Cost * 4);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Cost * DateTime.DaysInMonth(start.Year, start.Month + i));
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(expenses[n].Cost * 672);
                                    break;

                                default:
                                    result.Series[n].Add(expenses[n].TimePaid.Value.Month == start.Month + i && i + 1 < 12 ? 0 : expenses[n].Cost);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Yearly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (expenses[n].PaySchedule)
                            {
                                case ScheduleTypes.Yearly:
                                    result.Series[n].Add(expenses[n].Cost);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add(expenses[n].Cost * 2);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(expenses[n].Cost * 12);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(expenses[n].Cost * 52);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Cost * 365);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(expenses[n].Cost * 8064);
                                    break;

                                default:
                                    result.Series[n].Add(expenses[n].TimePaid.Value.Year != start.Year ? 0 : expenses[n].Cost);
                                    break;
                            }
                        }
                        break;
                }

            }


            return result;
        }

        public Chart<List<float>> GetCombinedChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();
            ScheduleTypes chartSchedule;

            Chart<List<float>> incomeChart = GetIncomeChart(userId, startDate, endDate, preferedLabel);
            Chart<List<float>> expensesChart = GetExpensesChart(userId, startDate, endDate, preferedLabel);

            result.Labels = CalculateLabelRange(out chartSchedule, (startDate != null) ? startDate.Value : DateTime.Now, (endDate != null) ? endDate.Value : DateTime.Now.AddDays(14), preferedLabel);
            result.Name = String.Format("Complete {0} Chart", chartSchedule.ToString());
            result.UserId = userId;
            result.TypeId = ChartTypes.Line;
            result.Series = incomeChart.Series.Concat(expensesChart.Series).ToList();
            result.Legend = incomeChart.Legend.Concat(expensesChart.Legend).ToList();

            return result;
        }

        public void InsertExpense(Expenses request)
        {
            _expenseSrv.Insert(request);
        }

        public void InsertIncome(Income request)
        {
            _incomeSrv.Insert(request);
        }

        public void UpdateExpense(Expenses request)
        {
            _expenseSrv.Update((a) => a.Id == request.Id, request);
        }

        public void UpdateIncome(Income request)
        {
            _incomeSrv.Update((a) => a.Id == request.Id, request);
        }

        public void DeleteExpense(int id)
        {
            _expenseSrv.Delete((a) => a.Id == id);
        }

        public void DeleteIncome(int id)
        {
            _incomeSrv.Delete((a) => a.Id == id);
        }

        private List<string> CalculateLabelRange(out ScheduleTypes schedule, DateTime startDate, DateTime endDate, string preferedLabels = null)
        {
            string[] defaultHourly = new[] { "12AM", "1AM", "2AM", "3AM", "4AM", "5AM", "6AM", "7AM", "8AM", "9AM", "10AM", "11AM", "12PM", "1PM", "2PM", "3PM", "4PM", "5PM", "6PM", "7PM", "8PM", "9PM", "10PM", "11PM" };

            List<string> hourly = defaultHourly.Where((a, b) => b >= startDate.Hour).Concat(defaultHourly.Where((a, b) => b < startDate.Hour)).ToList(),
                     daily = DateTimeFormatInfo.CurrentInfo.DayNames.Where((a, b) => b >= (int)startDate.DayOfWeek).Concat(DateTimeFormatInfo.CurrentInfo.DayNames.Where((a, b) => b < (int)startDate.DayOfWeek)).ToList(),
                     weekly = new List<string>(),
                     monthly = DateTimeFormatInfo.CurrentInfo.MonthNames.Where((a, b) => b >= startDate.Month - 1).Concat(DateTimeFormatInfo.CurrentInfo.MonthNames.Where((a, b) => b < startDate.Month - 1)).ToList(),
                     yearly = new List<string>();

            List<string> result = null;

            TimeSpan diff = (endDate - startDate);

            if (result == null) { result = new List<string>(); }
            if (preferedLabels == null) { preferedLabels = string.Empty; }

            if (diff.TotalHours < 168)
            {
                if (preferedLabels.ToLower() == "daily")
                {
                    schedule = ScheduleTypes.Daily;
                    result = daily.ToList();
                }
                else
                {
                    schedule = ScheduleTypes.Hourly;
                    result = hourly.ToList();
                }
            }
            else if (diff.TotalHours < 672)
            {
                if (preferedLabels.ToLower() == "weekly")
                {
                    schedule = ScheduleTypes.Weekly;
                    result = weekly.ToList();
                }
                else
                {
                    schedule = ScheduleTypes.Daily;
                    result = daily.ToList();
                }
            }
            else if (diff.TotalHours < 4032)
            {
                if (preferedLabels.ToLower() == "monthly")
                {
                    schedule = ScheduleTypes.Monthly;
                    result = monthly.ToList();
                }
                else
                {
                    schedule = ScheduleTypes.Weekly;
                    result = weekly.ToList();
                }
            }
            else if (diff.TotalHours < 24192)
            {
                if (preferedLabels.ToLower() == "yearly")
                {
                    schedule = ScheduleTypes.Yearly;
                    result = yearly.ToList();
                }
                else
                {
                    schedule = ScheduleTypes.Monthly;
                    result = monthly.ToList();
                }
            }
            else
            {
                schedule = ScheduleTypes.Yearly;
                result = yearly.ToList();
            }

            switch (schedule)
            {

                case ScheduleTypes.Hourly:
                    for (int i = 24; i < Math.Round(diff.TotalHours); i += 24)
                    {
                        result.AddRange(hourly);
                    }
                    break;

                case ScheduleTypes.Daily:
                    for (int i = 0, n = 0; i <= Math.Round(diff.TotalDays); i++, n++)
                    {
                        if (n > 6) { n = 0; }
                        result.Add(daily[n]);
                    }
                    break;

                case ScheduleTypes.Weekly:
                    for (int i = 0; (i * 0) < Math.Round(diff.TotalDays); i++)
                    {
                        result.Add(startDate.AddDays((i == 0) ? 0 : i * 7).ToShortDateString().Substring(0, 4) + "-" + startDate.AddDays((i == 0) ? 7 : (i + 1) * 7).ToShortDateString().Substring(0, 4)); //"Week " + (i));
                    }
                    break;

                case ScheduleTypes.Monthly:
                    for (int i = 365; i < Math.Round(diff.TotalDays); i += 365)
                    {
                        result.AddRange(monthly);
                    }
                    break;

                case ScheduleTypes.Yearly:
                    int year = 1;
                    for (int i = 365; i < Math.Round(diff.TotalDays); i += 365)
                    {
                        result.Add("Year " + (++year));
                    }
                    break;

            }


            return result;
        }

        private bool IsPayDay(DateTime start, int interval, ScheduleTypes schedule, Asset asset, out float pay)
        {
            bool result = false;
            DateTime intervalDay;
            pay = 0;

            switch (schedule)
            {
                case ScheduleTypes.Hourly:
                    int day = 0;
                    day += ((interval + 1) % 24 == 0) ? 1 : 0;
                    intervalDay = start.AddDays(day);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            result = true;
                            pay = (interval + 1) * asset.Cost;
                            break;

                        case ScheduleTypes.Daily:
                            result = (asset.TimePaid.Value.Hour == intervalDay.Hour) ? true : false;
                            pay = (day + 1) * asset.Cost;
                            break;

                        case ScheduleTypes.Weekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek && asset.TimePaid.Value.Hour == intervalDay.Hour) ? true : false;
                            pay = (result) ? (day + 1) * asset.Cost : asset.Cost * day;
                            break;

                        case ScheduleTypes.BiWeekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek && asset.TimePaid.Value.Hour == intervalDay.Hour && ((asset.TimePaid.Value.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven()) || (asset.TimePaid.Value.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))) ? true : false;
                            pay = (result) ? (day + 1) * asset.Cost : asset.Cost * day;
                            break;

                        default:
                            result = (asset.TimePaid.Value.Year == intervalDay.Year && asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.Day == intervalDay.Day && asset.TimePaid.Value.Hour == intervalDay.Hour) ? true : false;
                            pay = (result) ? asset.Cost : 0;
                            break;
                    }
                    break;

                case ScheduleTypes.Daily:
                    intervalDay = start.AddDays(interval);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            result = true;
                            pay = ((interval + 1) * 24) * asset.Cost;
                            break;

                        case ScheduleTypes.Daily:
                            result = true;
                            pay = (interval + 1) * asset.Cost;
                            break;
                        
                        case ScheduleTypes.Weekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;

                        case ScheduleTypes.BiWeekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek && ((asset.TimePaid.Value.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven()) || (asset.TimePaid.Value.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;

                        case ScheduleTypes.Monthly:
                            result = (asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.Day == intervalDay.Day) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;

                        default:
                            result = (asset.TimePaid.Value.Year == intervalDay.Year && asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.Day == intervalDay.Day) ? true : false;
                            pay = (result) ? asset.Cost : 0;
                            break;
                    }
                    break;

            }

            return result;
        }

    }
}
