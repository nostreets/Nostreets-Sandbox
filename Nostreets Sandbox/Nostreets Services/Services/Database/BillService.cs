using Nostreets_Services.Domain.Bills;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Requests;
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

        public Chart<List<int>> GetIncomeChart(string userId, DateTime? startDate = null, DateTime? endDate = null, string preferedLabel = null)
        {


            Chart<List<int>> result = null;
            List<Income> income = GetAllIncome(userId);
            ScheduleTypes schedule;

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.Add(new TimeSpan(24, 0, 0)) : endDate.Value;

            TimeSpan diff = (end - start);

            result.Name = "Income Chart";
            result.Labels = CalculateLabelRange(out schedule, startDate, endDate, preferedLabel);


            if (result.Series == null) { result.Series = new List<List<int>>(); }

            for (int n = 0; n < income.Count; n++)
            {
                result.Series.Add(new List<int>());
                result.Legend.Add(income[n].Name);

                switch (schedule)
                {

                    case ScheduleTypes.Hourly:
                        for (int i = 1; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((i == 168) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add((i % income[n].TimePaid.Hour) != 0 ? 0 : income[n].PayRate);
                                    break;
                            }
                            
                        }
                        break;

                    case ScheduleTypes.Daily:
                        for (int i = 1; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((i == 168) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add((i % income[n].TimePaid.Hour) != 0 ? 0 : income[n].PayRate);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Weekly:
                        for (int i = 1; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((i == 168) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add((i % income[n].TimePaid.Hour) != 0 ? 0 : income[n].PayRate);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Monthly:
                        for (int i = 1; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((i == 168) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add((i % income[n].TimePaid.Hour) != 0 ? 0 : income[n].PayRate);
                                    break;
                            }
                        }
                        break;

                    case ScheduleTypes.Yearly:
                        for (int i = 1; i < result.Labels.Count; i++)
                        {
                            switch (income[n].PaySchedule)
                            {
                                case ScheduleTypes.Weekly:
                                    result.Series[n].Add((i == 168) ? 0 : income[n].PayRate);
                                    break;

                                case ScheduleTypes.Daily:
                                    result.Series[n].Add((i % income[n].TimePaid.Hour) != 0 ? 0 : income[n].PayRate);
                                    break;
                            }
                        }
                        break;

                }
            }


            return result;
        }

        public Chart<List<int>> GetExpensesChart(string userId, DateTime startDate, DateTime endDate)
        {
            Chart<List<int>> result = null;



            return result;
        }

        public Chart<List<int>> GetCombinedChart(string userId, DateTime startDate, DateTime endDate)
        {
            Chart<List<int>> result = null;



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
            string[] hourly = new[] { "12AM", "1AM", "2AM", "3AM", "4AM", "5AM", "6AM", "7AM", "8AM", "9AM", "10AM", "11AM", "12PM", "1PM", "2PM", "3PM", "4PM", "5PM", "6PM", "7PM", "8PM", "9PM", "10PM", "11PM" },
                     daily = DateTimeFormatInfo.CurrentInfo.DayNames,
                     weekly = new[] { "Week 1" },
                     monthly = DateTimeFormatInfo.CurrentInfo.MonthNames,
                     yearly = new[] { "Year 1" };

            List<string> result = null;

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.Add(new TimeSpan(24, 0, 0)) : endDate.Value;

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

    }
}
