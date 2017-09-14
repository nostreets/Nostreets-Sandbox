using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Utilities;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using Nostreets_Services.Enums;

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
            return _expenseSrv.Where((a) => a.UserId == userId).ToList();
        }

        public List<Income> GetAllIncome(string userId)
        {
            return _incomeSrv.Where((a) => a.UserId == userId).ToList();
        }

        public Expenses GetExpense(string userId, Expenses expense)
        {
            return _expenseSrv.Where((a) => a.UserId == userId && a.Name == expense.Name).SingleOrDefault();
        }

        public Income GetIncome(string userId, Income income)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.Name == income.Name).SingleOrDefault();
        }

        public Chart<List<decimal>> GetIncomeChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {
            Chart<List<decimal>> result = null;
            List<Income> income = GetAllIncome(userId);
            ScheduleTypes chartSchedule;

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.Add(new TimeSpan(24, 0, 0)) : endDate.Value;

            TimeSpan diff = (end - start);

            result.Labels = CalculateLabelRange(out chartSchedule, startDate, endDate, preferedLabel);
            result.Name = String.Format("Income {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartTypes.Line;
            result.UserId = userId;

            if (result.Series == null) { result.Series = new List<List<decimal>>(); }

            for (int n = 0; n < income.Count; n++)
            {
                result.Series.Add(new List<decimal>());
                result.Legend.Add(income[n].Name);

                switch (chartSchedule)
                {

                    case ScheduleTypes.Hourly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            int day = 1;
                            day += ((i + 1) % 24 == 0) ? 1 : 0;
                            DateTime intervalDate = start.AddDays(day);

                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((!IsPayDay(intervalDate, chartSchedule, income[n])) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, income[n]) ? 0 : income[n].PayRate);
                                    break;
                            }

                        }
                        break;

                    case ScheduleTypes.Daily:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            DateTime intervalDate = start.AddDays(i + 1);
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, income[n]) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, income[n]) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].PayRate);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Weekly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((income[n].TimePaid.Month != start.Month + (i % 4 != 0 ? 0 : i / 4)) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add((!i.IsWeekOfMonth(income[n].TimePaid)) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].PayRate * 7);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(income[n].PayRate * 168);
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
                                    result.Series[n].Add((income[n].TimePaid.Month == start.Month + i) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((income[n].TimePaid.Month != start.Month + i || income[n].TimePaid.AddMonths(6).Month != start.Month + i) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(income[n].PayRate);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(income[n].PayRate * 4);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].PayRate * DateTime.DaysInMonth(start.Year, start.Month + i));
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(income[n].PayRate * 672);
                                    break;

                                default:
                                    result.Series[n].Add(income[n].TimePaid.Month == start.Month + i && i + 1 < 12 ? 0 : income[n].PayRate);
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
                                    result.Series[n].Add(income[n].PayRate);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add(income[n].PayRate * 2);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(income[n].PayRate * 12);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(income[n].PayRate * 52);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(income[n].PayRate * 365);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(income[n].PayRate * 8064);
                                    break;

                                default:
                                    result.Series[n].Add(income[n].TimePaid.Year != start.Year ? 0 : income[n].PayRate);
                                    break;
                            }
                        }
                        break;
                }

            }


            return result;
        }

        public Chart<List<decimal>> GetExpensesChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {
            Chart<List<decimal>> result = null;
            List<Expenses> expenses = GetAllExpenses(userId);
            ScheduleTypes chartSchedule;

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.Add(new TimeSpan(24, 0, 0)) : endDate.Value;

            TimeSpan diff = (end - start);

            result.Labels = CalculateLabelRange(out chartSchedule, startDate, endDate, preferedLabel);
            result.Name = String.Format("Expenses {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartTypes.Line;
            result.UserId = userId;

            if (result.Series == null) { result.Series = new List<List<decimal>>(); }

            for (int n = 0; n < expenses.Count; n++)
            {
                result.Series.Add(new List<decimal>());
                result.Legend.Add(expenses[n].Name);

                switch (chartSchedule)
                {

                    case ScheduleTypes.Hourly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            int day = 1;
                            day += ((i + 1) % 24 == 0) ? 1 : 0;
                            DateTime intervalDate = start.AddDays(day);

                            switch (expenses[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((!IsPayDay(intervalDate, chartSchedule, null, expenses[n])) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, null, expenses[n]) ? 0 : expenses[n].Price);
                                    break;
                            }

                        }
                        break;

                    case ScheduleTypes.Daily:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            DateTime intervalDate = start.AddDays(i + 1);
                            switch (expenses[n].PaySchedule)
                            {
                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, null, expenses[n]) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(!IsPayDay(intervalDate, chartSchedule, null, expenses[n]) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Price);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Weekly:
                        for (int i = 0; i < result.Labels.Count; i++)
                        {
                            switch (expenses[n].PaySchedule)
                            {

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((expenses[n].TimePaid.Month != start.Month + (i % 4 != 0 ? 0 : i / 4)) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add((!i.IsWeekOfMonth(expenses[n].TimePaid)) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(expenses[n].Price);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Price * 7);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(expenses[n].Price * 168);
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
                                    result.Series[n].Add((expenses[n].TimePaid.Month == start.Month + i) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add((expenses[n].TimePaid.Month != start.Month + i || expenses[n].TimePaid.AddMonths(6).Month != start.Month + i) ? 0 : expenses[n].Price);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(expenses[n].Price);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(expenses[n].Price * 4);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Price * DateTime.DaysInMonth(start.Year, start.Month + i));
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(expenses[n].Price * 672);
                                    break;

                                default:
                                    result.Series[n].Add(expenses[n].TimePaid.Month == start.Month + i && i + 1 < 12 ? 0 : expenses[n].Price);
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
                                    result.Series[n].Add(expenses[n].Price);
                                    break;

                                case ScheduleTypes.BiYearly:
                                    result.Series[n].Add(expenses[n].Price * 2);
                                    break;

                                case ScheduleTypes.Monthly:
                                    result.Series[n].Add(expenses[n].Price * 12);
                                    break;

                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add(expenses[n].Price * 52);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add(expenses[n].Price * 365);
                                    break;

                                case ScheduleTypes.Hourly:
                                    result.Series[n].Add(expenses[n].Price * 8064);
                                    break;

                                default:
                                    result.Series[n].Add(expenses[n].TimePaid.Year != start.Year ? 0 : expenses[n].Price);
                                    break;
                            }
                        }
                        break;
                }

            }


            return result;
        }

        public Chart<List<decimal>> GetCombinedChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {
            Chart<List<decimal>> result = null;
            ScheduleTypes chartSchedule;

            Chart<List<decimal>> incomeChart = GetIncomeChart(userId, startDate, endDate, preferedLabel);
            Chart<List<decimal>> expensesChart = GetExpensesChart(userId, startDate, endDate, preferedLabel);

            result.Labels = CalculateLabelRange(out chartSchedule, startDate, endDate, preferedLabel);
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
            _expenseSrv.Update(request);
        }

        public void UpdateIncome(Income request)
        {
            _incomeSrv.Update(request);
        }

        public void DeleteExpense(string userId, int id)
        {
            _expenseSrv.Delete(id);
        }

        public void DeleteIncome(string userId, int id)
        {
            _incomeSrv.Delete(id);
        }

        private List<string> CalculateLabelRange(out ScheduleTypes schedule, DateTime? startDate = null, DateTime? endDate = null, string preferedLabels = null)
        {
            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                    end = (endDate == null) ? DateTime.Now.Add(new TimeSpan(24, 0, 0)) : endDate.Value;

            string[] hourly = new[] { "12AM", "1AM", "2AM", "3AM", "4AM", "5AM", "6AM", "7AM", "8AM", "9AM", "10AM", "11AM", "12PM", "1PM", "2PM", "3PM", "4PM", "5PM", "6PM", "7PM", "8PM", "9PM", "10PM", "11PM" },
                     daily = DateTimeFormatInfo.CurrentInfo.DayNames.Where((a, b) => b >= (int)start.DayOfWeek).Concat(DateTimeFormatInfo.CurrentInfo.DayNames.Where((a, b) => b < (int)start.DayOfWeek)).ToArray(),
                     weekly = new[] { "Week 1" },
                     monthly = DateTimeFormatInfo.CurrentInfo.MonthNames.Where((a, b) => b >= start.Month - 1).Concat(DateTimeFormatInfo.CurrentInfo.MonthNames.Where((a, b) => b < start.Month - 1)).ToArray(),
                     yearly = new[] { "Year 1" };

            List<string> result = null;



            TimeSpan diff = (end - start);

            if (result == null) { result = new List<string>(); }

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
                    for (int i = 24; i < diff.TotalHours; i += 24)
                    {
                        result.AddRange(hourly);
                    }
                    break;

                case ScheduleTypes.Daily:
                    for (int i = 1; i < diff.TotalDays; i++)
                    {
                        result.AddRange(daily);
                    }
                    break;

                case ScheduleTypes.Weekly:
                    int week = 1;
                    for (int i = 7; i < diff.TotalDays; i += 7)
                    {
                        result.Add("Week" + (++week));
                    }
                    break;

                case ScheduleTypes.Monthly:
                    for (int i = 365; i < diff.TotalDays; i += 365)
                    {
                        result.AddRange(monthly);
                    }
                    break;

                case ScheduleTypes.Yearly:
                    int year = 1;
                    for (int i = 365; i < diff.TotalDays; i += 365)
                    {
                        result.Add("Year" + (++year));
                    }
                    break;

            }


            return result;
        }

        private bool IsPayDay(DateTime intervalDay, ScheduleTypes schedule, Income income = null, Expenses expense = null)
        {
            bool result = false;

            switch (schedule)
            {
                case ScheduleTypes.Hourly:

                    switch (income.PaySchedule)
                    {
                        case ScheduleTypes.Weekly:
                            if (income != null)
                            {
                                result = (income.TimePaid.DayOfWeek == intervalDay.DayOfWeek && income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            else if (expense != null)
                            {
                                result = (expense.TimePaid.DayOfWeek == intervalDay.DayOfWeek && income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            break;

                        case ScheduleTypes.Daily:
                            if (income != null)
                            {
                                result = (income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            else if (expense != null)
                            {
                                result = (expense.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            break;
                    }
                    break;

                case ScheduleTypes.Daily:

                    switch (income.PaySchedule)
                    {

                        case ScheduleTypes.Monthly:
                            if (income != null)
                            {
                                result = (income.TimePaid.Month == intervalDay.Month && income.TimePaid.Day == intervalDay.Day && income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            else if (expense != null)
                            {
                                result = (expense.TimePaid.Month == intervalDay.Month && income.TimePaid.Day == intervalDay.Day && income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            break;

                        case ScheduleTypes.Weekly:
                            if (income != null)
                            {
                                result = (income.TimePaid.DayOfWeek == intervalDay.DayOfWeek && income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            else if (expense != null)
                            {
                                result = (expense.TimePaid.DayOfWeek == intervalDay.DayOfWeek && income.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            }
                            break;
                    }
                    break;

            }

            return result;
        }

    }
}
