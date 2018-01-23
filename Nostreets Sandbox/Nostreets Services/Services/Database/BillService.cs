using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Enums;
using Nostreets_Services.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NostreetsExtensions;
using NostreetsORM;

namespace Nostreets_Services.Services.Database
{
    public class BillService : IBillService
    {
        public BillService()
        {
            _connectionKey = "DefaultConnection";
            _incomeSrv = new DBService<Income>(_connectionKey);
            _expenseSrv = new DBService<Expenses>(_connectionKey);
        }

        public BillService(string connectionKey)
        {
            _connectionKey = connectionKey;
            _incomeSrv = new DBService<Income>(_connectionKey);
            _expenseSrv = new DBService<Expenses>(_connectionKey);
        }


        private string _connectionKey = null;
        private DBService<Expenses> _expenseSrv = null;
        private DBService<Income> _incomeSrv = null;

        public List<Expenses> GetAllExpenses(string userId)
        {
            List<Expenses> expenses = _expenseSrv.Where((a) => a.UserId == userId)?.ToList();
            expenses?.Sort((a, b) => DateTime.Compare(a.DateModified.Value, b.DateModified.Value));
            return expenses;
        }

        public List<Income> GetAllIncome(string userId)
        {
            List<Income> incomes = _incomeSrv.Where((a) => a.UserId == userId)?.ToList();
            incomes?.Sort((a, b) => DateTime.Compare(a.DateModified.Value, b.DateModified.Value));
            return incomes;
        }

        public Expenses GetExpense(string userId, int id)
        {
            return _expenseSrv.Get(id);
        }

        public Expenses GetExpense(string userId, string expenseName)
        {
            return _expenseSrv.Where(a => a.UserId == userId && a.Name == expenseName).FirstOrDefault();
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
            return _incomeSrv.Get(id);
        }

        public Income GetIncome(string userId, string incomeName)
        {
            return _incomeSrv.Where(a => a.UserId == userId && a.Name == incomeName).FirstOrDefault();
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

        public Chart<List<float>> GetIncomeChart(string userId, ref ScheduleTypes chartSchedule, DateTime? startDate = null, DateTime? endDate = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();
            List<Income> incomes = GetAllIncome(userId);

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.AddDays(14) : endDate.Value;

            TimeSpan diff = (end - start);

            int numOfAssetsSkipped = 0;

            result.Labels = CalculateLabelRange(ref chartSchedule, start, end);
            result.Name = String.Format("Income {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartTypes.Line;
            result.UserId = userId;

            if (result.Series == null) { result.Series = new List<List<float>>(); }
            if (result.Legend == null) { result.Legend = new List<string>(); }

            if (incomes != null)
                for (int n = 0; n < incomes.Count; n++)
                {
                    if (incomes[n].IsHiddenOnChart) { numOfAssetsSkipped++; continue; }

                    result.Series.Add(new List<float>());
                    result.Legend.Add(incomes[n].Name);

                    switch (chartSchedule)
                    {

                        case ScheduleTypes.Hourly:
                            for (int i = 0; i < result.Labels.Count; i++)
                            {
                                int day = 0;
                                day += ((i + 1) % 24 == 0) ? 1 : 0;

                                IsPayDay(start, i, chartSchedule, incomes[n], out float pay);
                                result.Series[n - numOfAssetsSkipped].Add(pay);
                            }
                            break;

                        case ScheduleTypes.Daily:
                            for (int i = 0; i < result.Labels.Count; i++)
                            {
                                IsPayDay(start, i, chartSchedule, incomes[n], out float pay);
                                result.Series[n - numOfAssetsSkipped].Add(pay);
                            }
                            break;

                        case ScheduleTypes.Weekly:
                            for (int i = 0; i < result.Labels.Count; i++)
                            {
                                IsPayDay(start, i, chartSchedule, incomes[n], out float pay);
                                result.Series[n - numOfAssetsSkipped].Add(pay);
                            }
                            break;

                        case ScheduleTypes.Monthly:
                            for (int i = 0; i < result.Labels.Count; i++)
                            {
                                switch (incomes[n].PaySchedule)
                                {
                                    case ScheduleTypes.Yearly:
                                        result.Series[n].Add((incomes[n].TimePaid.Value.Month == start.Month + i) ? 0 : incomes[n].Cost);
                                        break;

                                    case ScheduleTypes.BiYearly:
                                        result.Series[n].Add((incomes[n].TimePaid.Value.Month != start.Month + i || incomes[n].TimePaid.Value.AddMonths(6).Month != start.Month + i) ? 0 : incomes[n].Cost);
                                        break;

                                    case ScheduleTypes.Monthly:
                                        result.Series[n].Add(incomes[n].Cost);
                                        break;

                                    case ScheduleTypes.Weekly:
                                        result.Series[n].Add(incomes[n].Cost * 4);
                                        break;

                                    case ScheduleTypes.Daily:
                                        result.Series[n].Add(incomes[n].Cost * DateTime.DaysInMonth(start.Year, start.Month + i));
                                        break;

                                    case ScheduleTypes.Hourly:
                                        result.Series[n].Add(incomes[n].Cost * 672);
                                        break;

                                    default:
                                        result.Series[n].Add(incomes[n].TimePaid.Value.Month == start.Month + i && i + 1 < 12 ? 0 : incomes[n].Cost);
                                        break;
                                }
                            }
                            break;

                        case ScheduleTypes.Yearly:
                            for (int i = 0; i < result.Labels.Count; i++)
                            {
                                switch (incomes[n].PaySchedule)
                                {
                                    case ScheduleTypes.Yearly:
                                        result.Series[n].Add(incomes[n].Cost);
                                        break;

                                    case ScheduleTypes.BiYearly:
                                        result.Series[n].Add(incomes[n].Cost * 2);
                                        break;

                                    case ScheduleTypes.Monthly:
                                        result.Series[n].Add(incomes[n].Cost * 12);
                                        break;

                                    case ScheduleTypes.Weekly:
                                        result.Series[n].Add(incomes[n].Cost * 52);
                                        break;

                                    case ScheduleTypes.Daily:
                                        result.Series[n].Add(incomes[n].Cost * 365);
                                        break;

                                    case ScheduleTypes.Hourly:
                                        result.Series[n].Add(incomes[n].Cost * 8064);
                                        break;

                                    default:
                                        result.Series[n].Add(incomes[n].TimePaid.Value.Year != start.Year ? 0 : incomes[n].Cost);
                                        break;
                                }
                            }
                            break;
                    }

                }
            else
                result = null;


            return result;
        }

        public Chart<List<float>> GetExpensesChart(string userId, ref ScheduleTypes chartSchedule, DateTime? startDate = null, DateTime? endDate = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();
            List<Expenses> expenses = GetAllExpenses(userId);

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.AddDays(14) : endDate.Value;

            TimeSpan diff = (end - start);

            result.Labels = CalculateLabelRange(ref chartSchedule, start, end);
            result.Name = String.Format("Expenses {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartTypes.Line;
            result.UserId = userId;

            if (result.Series == null) { result.Series = new List<List<float>>(); }
            if (result.Legend == null) { result.Legend = new List<string>(); }

            if (expenses != null)
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
                            }
                            break;

                        case ScheduleTypes.Daily:
                            for (int i = 0; i < result.Labels.Count; i++)
                            {

                                IsPayDay(start, i, chartSchedule, expenses[n], out float price);

                                result.Series[n].Add(price);
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
            else
                result = null;


            return result;
        }

        public Chart<List<float>> GetCombinedChart(string userId, ref ScheduleTypes chartSchedule, DateTime? startDate = null, DateTime? endDate = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();

            Chart<List<float>> incomeChart = GetIncomeChart(userId, ref chartSchedule, startDate, endDate);
            Chart<List<float>> expensesChart = GetExpensesChart(userId, ref chartSchedule, startDate, endDate);

            result.Labels = CalculateLabelRange(ref chartSchedule, (startDate != null) ? startDate.Value : DateTime.Now, (endDate != null) ? endDate.Value : DateTime.Now.AddDays(14));
            result.Name = String.Format("Complete {0} Chart", chartSchedule.ToString());
            result.UserId = userId;
            result.TypeId = ChartTypes.Line;
            result.Series = incomeChart?.Series.Concat(expensesChart.Series).ToList();
            result.Legend = incomeChart?.Legend.Concat(expensesChart.Legend).ToList();

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
            _expenseSrv.Update(/*(a) => a.Id == request.Id, */request);
        }

        public void UpdateIncome(Income request)
        {
            _incomeSrv.Update(/*(a) => a.Id == request.Id,*/ request);
        }

        public void DeleteExpense(int id)
        {
            _expenseSrv.Delete(id);
        }

        public void DeleteIncome(int id)
        {
            _incomeSrv.Delete(id);
        }

        private List<string> CalculateLabelRange(ref ScheduleTypes schedule, DateTime startDate, DateTime endDate)
        {
            List<string> result = null;
            TimeSpan diff = (endDate - startDate);

            Func<List<string>>
                generateHourly = () =>
                {
                    string[] defaultHourly = new[] { "12AM", "1AM", "2AM", "3AM", "4AM", "5AM", "6AM", "7AM", "8AM", "9AM", "10AM", "11AM", "12PM", "1PM", "2PM", "3PM", "4PM", "5PM", "6PM", "7PM", "8PM", "9PM", "10PM", "11PM" };
                    List<string> hourly = defaultHourly.Where((a, b) => b >= startDate.Hour).Concat(
                                        defaultHourly.Where((a, b) => b < startDate.Hour)).ToList();

                    for (int i = hourly.Count; i < Math.Round(diff.TotalHours); i += 24)
                    {
                        hourly.AddRange(hourly);
                    }

                    return hourly;
                }
            ,
                generateDaily = () =>
                {
                    List<string> daily = daily = DateTimeFormatInfo.CurrentInfo.DayNames.Where((a, b) => b >= (int)startDate.DayOfWeek).Concat(
                                                 DateTimeFormatInfo.CurrentInfo.DayNames.Where((a, b) => b < (int)startDate.DayOfWeek)).ToList();

                    for (int i = daily.Count, n = 0; i <= Math.Round(diff.TotalDays); i++, n++)
                    {
                        if (n > 6) { n = 0; }
                        daily.Add(daily[n]);
                    }

                    return daily;
                }
            ,
                generateWeekly = () =>
                    {

                        List<string> weekly = new List<string>();

                        for (int i = 0; i < diff.TotalDays; i += 7)
                        {
                            DateTime weekDate = startDate.AddDays(i);
                            weekly.Add(String.Format("{0}/{1} - {2}/{3}", weekDate.StartOfWeek().Month, weekDate.StartOfWeek().Day, weekDate.EndOfWeek().Month, weekDate.EndOfWeek().Day));
                        }

                        return weekly;
                    }
            ,
                generateMonthly = () =>
                {
                    List<string> monthly = DateTimeFormatInfo.CurrentInfo.MonthNames.Where((a, b) => b >= startDate.Month - 1).Concat(
                              DateTimeFormatInfo.CurrentInfo.MonthNames.Where((a, b) => b < startDate.Month - 1)).ToList();

                    for (int i = 365; i < Math.Round(diff.TotalDays); i += 365)
                    {
                        monthly.AddRange(monthly);
                    }

                    return monthly;
                }
            ,
                generateYearly = () =>
                {
                    List<string> yearly = new List<string>();

                    int year = 1;
                    for (int i = 365; i < Math.Round(diff.TotalDays); i += 365)
                    {
                        result.Add("Year " + (++year));
                    }

                    return yearly;
                };



            if (result == null) { result = new List<string>(); }

            if (diff.TotalHours < 168)
            {
                if (schedule == ScheduleTypes.Daily)
                {
                    result = generateDaily();
                }
                else
                {
                    schedule = ScheduleTypes.Hourly;
                    result = generateHourly();
                }
            }
            else if (diff.TotalHours < 672)
            {
                if (schedule == ScheduleTypes.Weekly)
                {
                    result = generateWeekly();
                }
                else
                {
                    schedule = ScheduleTypes.Daily;
                    result = generateDaily();
                }
            }
            else if (diff.TotalHours < 4032)
            {
                if (schedule == ScheduleTypes.Monthly)
                {
                    result = generateMonthly();
                }
                else
                {
                    schedule = ScheduleTypes.Weekly;
                    result = generateWeekly();
                }
            }
            else if (diff.TotalHours < 24192)
            {
                if (schedule == ScheduleTypes.Yearly)
                {
                    result = generateYearly();
                }
                else
                {
                    schedule = ScheduleTypes.Monthly;
                    result = generateMonthly();
                }
            }
            else
            {
                schedule = ScheduleTypes.Yearly;
                result = generateYearly();
            }


            /*
            switch (schedule)
            {

                case ScheduleTypes.Hourly:
                    for (int i = result.Count; i < Math.Round(diff.TotalHours); i += 24)
                    {
                        result.AddRange(hourly);
                    }
                    break;

                case ScheduleTypes.Daily:
                    for (int i = result.Count, n = 0; i <= Math.Round(diff.TotalDays); i++, n++)
                    {
                        if (n > 6) { n = 0; }
                        result.Add(daily[n]);
                    }
                    break;

                case ScheduleTypes.Weekly:
                    //for (int i = 0; (i * 0) < Math.Round(diff.TotalDays); i++)
                    //{
                    //    result.Add(startDate.AddDays((i == 0) ? 0 : i * 7).ToShortDateString().Substring(0, 4) + "-" + startDate.AddDays((i == 0) ? 7 : (i + 1) * 7).ToShortDateString().Substring(0, 4)); //"Week " + (i));
                    //}
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
            */

            return result;
        }

        private bool IsPayDay(DateTime start, int interval, ScheduleTypes schedule, Asset asset, out float pay)
        {
            //int day = 0;
            float lastPayment = 0;
            bool result = false;
            DateTime intervalDay = start;
            pay = 0;

            if (interval != 0) { IsPayDay(start, interval - 1, schedule, asset, out lastPayment); };

            switch (schedule)
            {
                case ScheduleTypes.Hourly:

                    //day += ((interval + 1) % 24 == 0) ? 1 : 0;
                    //intervalDay = start.AddDays(day);
                    intervalDay = start.AddHours(interval);


                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            result = (asset.BeginDate?.Hour < intervalDay.Hour && asset.EndDate?.Hour > intervalDay.Hour) ? true : false;
                            pay = lastPayment + asset.Cost;
                            break;

                        case ScheduleTypes.Daily:
                            result = (asset.TimePaid.Value.Hour == intervalDay.Hour) ? true : false;
                            pay = (result) ? (asset.Cost * 24) + lastPayment : lastPayment;
                            break;

                        case ScheduleTypes.Weekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek && asset.TimePaid.Value.Hour == intervalDay.Hour) ? true : false;
                            pay = (result) ? ((asset.Cost * 24) * 7) + lastPayment : lastPayment;
                            break;

                        case ScheduleTypes.BiWeekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek && asset.TimePaid.Value.Hour == intervalDay.Hour && ((asset.TimePaid.Value.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven()) || (asset.TimePaid.Value.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))) ? true : false;
                            pay = (result) ? ((asset.Cost * 24) * 14) + lastPayment : lastPayment;
                            break;

                        default:
                            result = (asset.TimePaid.Value.Year == intervalDay.Year && asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.Day == intervalDay.Day && asset.TimePaid.Value.Hour == intervalDay.Hour) ? true : false;
                            pay = (result) ? asset.Cost + lastPayment : lastPayment;
                            break;
                    }
                    break;

                case ScheduleTypes.Daily:
                    intervalDay = start.AddDays(interval - 1);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            result = true;
                            pay = lastPayment + (asset.Cost * 24);
                            break;

                        case ScheduleTypes.Daily:
                            result = true;
                            pay = lastPayment + asset.Cost;
                            break;

                        case ScheduleTypes.Weekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek) ? true : false;
                            pay = (result) ? lastPayment + (asset.Cost * 7) : lastPayment;
                            break;

                        case ScheduleTypes.BiWeekly:
                            result = (asset.TimePaid.Value.DayOfWeek == intervalDay.DayOfWeek && ((asset.TimePaid.Value.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven()) || (asset.TimePaid.Value.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))) ? true : false;
                            pay = (result) ? lastPayment + (asset.Cost * 14) : lastPayment;
                            break;

                        case ScheduleTypes.Monthly:
                            result = (asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.Day == intervalDay.Day) ? true : false;
                            pay = (result) ? lastPayment + (intervalDay.Day * asset.Cost) : lastPayment;
                            break;

                        default:
                            result = (asset.TimePaid.Value.Year == intervalDay.Year && asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.Day == intervalDay.Day) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : lastPayment;
                            break;
                    }
                    break;


                case ScheduleTypes.Weekly:
                    intervalDay = start.AddDays(interval - 1);
                    int intervalWeek = intervalDay.GetWeekOfMonth();

                    switch (asset.PaySchedule)
                    {

                        case ScheduleTypes.Hourly:
                            result = true;
                            pay = (((interval + 1) * 7) * 24) * asset.Cost;
                            break;

                        case ScheduleTypes.Daily:
                            result = true;
                            pay = ((interval + 1) * 7) * asset.Cost;
                            break;

                        case ScheduleTypes.Weekly:
                            result = true;
                            pay = (interval + 1) * asset.Cost;
                            break;

                        case ScheduleTypes.BiWeekly:
                            result = (intervalWeek == asset.TimePaid.Value.GetWeekOfMonth() && ((asset.TimePaid.Value.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven()) || (asset.TimePaid.Value.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;

                        case ScheduleTypes.Monthly:
                            result = (asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.GetWeekOfMonth() == intervalDay.GetWeekOfMonth()) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;

                        case ScheduleTypes.BiYearly:
                            result = ((asset.TimePaid.Value.Month == intervalDay.Month || asset.TimePaid.Value.AddMonths(6).Month == intervalDay.Month) && asset.TimePaid.Value.GetWeekOfMonth() == intervalDay.GetWeekOfMonth()) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;

                        case ScheduleTypes.Yearly:
                            result = (asset.TimePaid.Value.GetWeekOfYear() == intervalDay.GetWeekOfYear()) ? true : false;
                            pay = (result) ? (interval + 1) * asset.Cost : interval * asset.Cost;
                            break;


                        default:
                            result = (asset.TimePaid.Value.Year == intervalDay.Year && asset.TimePaid.Value.Month == intervalDay.Month && asset.TimePaid.Value.GetWeekOfMonth() == intervalDay.GetWeekOfMonth()) ? true : false;
                            pay = (result) ? asset.Cost : 0;
                            break;
                    }
                    break;

            }

            return result;
        }

    }
}
