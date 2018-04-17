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
using NostreetsExtensions.Utilities;
using Nostreets_Services.Domain;
using NostreetsExtensions.Interfaces;

namespace Nostreets_Services.Services.Database
{
    public class BillService : IBillService
    {
        public BillService(IDBService<Income> incomeSrv, IDBService<Expense> expenseSrv)
        {
            _incomeSrv = incomeSrv;
            _expenseSrv = expenseSrv;
        }

        private IDBService<Expense> _expenseSrv = null;
        private IDBService<Income> _incomeSrv = null;


        private List<string> CalculateLabelRange(out ScheduleTypes schedule, DateTime startDate, DateTime endDate)
        {
            List<string> result = null;
            TimeSpan diff = (endDate - startDate);

            Func<List<string>>
                generateHourly = () =>
                {
                    string[] defaultHourly = new[] { startDate.Month + '/' + startDate.Hour + " 12AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 1AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 2AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 3AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 4AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 5AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 6AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 7AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 8AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 9AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 10AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 11AM"
                                                    , startDate.Month + '/' + startDate.Hour + " 12PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 1PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 2PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 3PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 4PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 5PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 6PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 7PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 8PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 9PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 10PM"
                                                    , startDate.Month + '/' + startDate.Hour + " 11PM" };
                    List<string> hourly = defaultHourly.Where((a, b) => b >= startDate.Hour).Concat(defaultHourly.Where((a, b) => b < startDate.Hour)).ToList();

                    for (int i = 24; i < Math.Round(diff.TotalHours); i += 24)
                    {
                        DateTime newDate = startDate.AddHours(i);
                        defaultHourly = new[] { newDate.Month + '/' + newDate.Hour + " 12AM"
                                                , newDate.Month + '/' + newDate.Hour + " 1AM"
                                                , newDate.Month + '/' + newDate.Hour + " 2AM"
                                                , newDate.Month + '/' + newDate.Hour + " 3AM"
                                                , newDate.Month + '/' + newDate.Hour + " 4AM"
                                                , newDate.Month + '/' + newDate.Hour + " 5AM"
                                                , newDate.Month + '/' + newDate.Hour + " 6AM"
                                                , newDate.Month + '/' + newDate.Hour + " 7AM"
                                                , newDate.Month + '/' + newDate.Hour + " 8AM"
                                                , newDate.Month + '/' + newDate.Hour + " 9AM"
                                                , newDate.Month + '/' + newDate.Hour + " 10AM"
                                                , newDate.Month + '/' + newDate.Hour + " 11AM"
                                                , newDate.Month + '/' + newDate.Hour + " 12PM"
                                                , newDate.Month + '/' + newDate.Hour + " 1PM"
                                                , newDate.Month + '/' + newDate.Hour + " 2PM"
                                                , newDate.Month + '/' + newDate.Hour + " 3PM"
                                                , newDate.Month + '/' + newDate.Hour + " 4PM"
                                                , newDate.Month + '/' + newDate.Hour + " 5PM"
                                                , newDate.Month + '/' + newDate.Hour + " 6PM"
                                                , newDate.Month + '/' + newDate.Hour + " 7PM"
                                                , newDate.Month + '/' + newDate.Hour + " 8PM"
                                                , newDate.Month + '/' + newDate.Hour + " 9PM"
                                                , newDate.Month + '/' + newDate.Hour + " 10PM"
                                                , newDate.Month + '/' + newDate.Hour + " 11PM" };
                        hourly = defaultHourly.Where((a, b) => b >= startDate.Hour).Concat(defaultHourly.Where((a, b) => b < startDate.Hour)).ToList();
                        hourly.AddRange(hourly);
                    }

                    return hourly;
                }
            ,
                generateDaily = () =>
                {

                    int day = 0;
                    DateTime end = default(DateTime);
                    List<string> daily = new List<string>();

                    string formattedDate = endDate.ToString("M/d");
                    daily.Prepend(formattedDate);
                    do
                    {
                        end = endDate.AddDays(--day);
                        formattedDate = end.ToString("M/d");
                        daily.Prepend(formattedDate);
                    }
                    while (end > startDate);

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
                schedule = ScheduleTypes.Hourly;
                result = generateHourly();
            }
            else if (diff.TotalHours < 1920/*2160*/)
            {
                schedule = ScheduleTypes.Daily;
                result = generateDaily();
            }
            else if (diff.TotalHours < 4320)
            {
                schedule = ScheduleTypes.Weekly;
                result = generateWeekly();
            }
            else if (diff.TotalHours < 43829)
            {
                schedule = ScheduleTypes.Monthly;
                result = generateMonthly();
            }
            else
            {
                schedule = ScheduleTypes.Yearly;
                result = generateYearly();
            }

            return result;
        }

        private float PayOfDay(DateTime start, int interval, ScheduleTypes schedule, FinicialAsset asset)
        {
            float lastPayment = 0,
                  pay = 0;
            bool isPayDay = false;
            DateTime intervalDay = default(DateTime);
            Func<int, float> getPay = (modifier) =>
            {
                return (!isPayDay) 
                        ? lastPayment
                        : (modifier == 0)
                        ? lastPayment + (asset.Cost * asset.RateMultilplier)
                        : lastPayment + ((asset.Cost * asset.RateMultilplier) * modifier);
            };


            if (interval != 0) { lastPayment = PayOfDay(start, interval - 1, schedule, asset); };


            switch (schedule)
            {
                case ScheduleTypes.Hourly:
                    intervalDay = start.AddHours(interval);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            isPayDay = (asset.BeginDate == null && asset.EndDate == null)
                                            ? (17 < intervalDay.Hour && intervalDay.Hour > 8)
                                            : (asset.EndDate?.Hour < intervalDay.Hour && intervalDay.Hour > asset.BeginDate?.Hour)
                                            ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Daily:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Weekly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && asset.TimePaid.DayOfWeek == intervalDay.DayOfWeek) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiWeekly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && asset.TimePaid.DayOfWeek == intervalDay.DayOfWeek
                                        && ((asset.TimePaid.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven())
                                            ||
                                            (asset.TimePaid.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd())))
                                        ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.SemiMonthly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                       && intervalDay.SemiMonthDate().Day == intervalDay.Day)
                                       ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Monthly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && asset.TimePaid.Day == intervalDay.Day)
                                        ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Quarterly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && asset.TimePaid.Day == intervalDay.Day
                                        && intervalDay.Month.In(1, 4, 7, 10))
                                        ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiYearly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && (asset.TimePaid.Month == intervalDay.Month || asset.TimePaid.AddMonths(6).Month == intervalDay.Month)
                                        && asset.TimePaid.Day == intervalDay.Day)
                                        ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Yearly:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && asset.TimePaid.Month == intervalDay.Month
                                        && asset.TimePaid.Day == intervalDay.Day)
                                        ? true : false;
                            pay = getPay(0);
                            break;

                        default:
                            isPayDay = (asset.TimePaid.Hour == intervalDay.Hour
                                        && asset.TimePaid.Year == intervalDay.Year
                                        && asset.TimePaid.Month == intervalDay.Month
                                        && asset.TimePaid.Day == intervalDay.Day)
                                        ? true : false;
                            pay = getPay(0);
                            break;
                    }
                    break;

                case ScheduleTypes.Daily:
                    intervalDay = start.AddDays(interval);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            isPayDay = true;
                            int? payedHours = (asset.BeginDate.Value == null && asset.EndDate.Value == null)
                                                ? 8 : asset.EndDate?.Hour - asset.BeginDate?.Hour;
                            pay = getPay(payedHours.Value); //lastPayment + ((asset.Cost * asset.RateMultilplier) * payedHours.Value);
                            break;

                        case ScheduleTypes.Daily:
                            isPayDay = true;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Weekly:
                            isPayDay = (asset.TimePaid.DayOfWeek == intervalDay.DayOfWeek) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiWeekly:
                            isPayDay = (asset.TimePaid.DayOfWeek == intervalDay.DayOfWeek
                                        && ((asset.TimePaid.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven())
                                            ||
                                            (asset.TimePaid.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))
                                        ) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.SemiMonthly:
                            isPayDay = (intervalDay.SemiMonthDate().Day == intervalDay.Day)
                                       ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Monthly:
                            isPayDay = (asset.TimePaid.Day == intervalDay.Day) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Quarterly:
                            isPayDay = (asset.TimePaid.Day == intervalDay.Day && intervalDay.Month.In(1, 4, 7, 10)) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiYearly:
                            isPayDay = ((asset.TimePaid.Month == intervalDay.Month || asset.TimePaid.AddMonths(6).Month == intervalDay.Month)
                                        && asset.TimePaid.Day == intervalDay.Day)
                                        ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Yearly:
                            isPayDay = (asset.TimePaid.Month == intervalDay.Month && asset.TimePaid.Day == intervalDay.Day) ? true : false;
                            pay = getPay(0);
                            break;

                        default:
                            isPayDay = (asset.TimePaid.Year == intervalDay.Year && asset.TimePaid.Month == intervalDay.Month && asset.TimePaid.Day == intervalDay.Day) ? true : false;
                            pay = getPay(0);
                            break;
                    }
                    break;


                case ScheduleTypes.Weekly:
                    intervalDay = start.AddDays((interval + 1) * 7);
                    int intervalWeek = intervalDay.GetWeekOfMonth();

                    switch (asset.PaySchedule)
                    {

                        case ScheduleTypes.Hourly:
                            isPayDay = true;
                            int? payedHours = (asset.BeginDate.Value == null && asset.EndDate.Value == null)
                                                ? 8 : asset.EndDate?.Hour - asset.BeginDate?.Hour;
                            pay = getPay(payedHours.Value * 5); // lastPayment + (((asset.Cost * asset.RateMultilplier) * payedHours.Value) * 5);
                            break;

                        case ScheduleTypes.Daily:
                            isPayDay = true;
                            pay = getPay(5); //  lastPayment + ((asset.Cost * asset.RateMultilplier) * 5);
                            break;

                        case ScheduleTypes.Weekly:
                            isPayDay = true;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiWeekly:
                            isPayDay = (intervalWeek == asset.TimePaid.GetWeekOfMonth()
                                        && ((asset.TimePaid.GetWeekOfYear().IsEven() && intervalDay.GetWeekOfYear().IsEven())
                                            ||
                                           (asset.TimePaid.GetWeekOfYear().IsOdd() && intervalDay.GetWeekOfYear().IsOdd()))
                                        ) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.SemiMonthly:
                            isPayDay = (intervalDay.SemiMonthDate().GetWeekOfMonth() == intervalDay.GetWeekOfMonth())
                                       ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Monthly:
                            isPayDay = (asset.TimePaid.GetWeekOfMonth() == intervalDay.GetWeekOfMonth()) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Quarterly:
                            isPayDay = (asset.TimePaid.GetWeekOfMonth() == intervalDay.GetWeekOfMonth() && intervalDay.Month.In(1, 4, 7, 10)) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiYearly:
                            isPayDay = ((asset.TimePaid.Month == intervalDay.Month || asset.TimePaid.AddMonths(6).Month == intervalDay.Month) && asset.TimePaid.GetWeekOfMonth() == intervalDay.GetWeekOfMonth()) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Yearly:
                            isPayDay = (asset.TimePaid.GetWeekOfYear() == intervalDay.GetWeekOfYear()) ? true : false;
                            pay = getPay(0);
                            break;


                        default:
                            isPayDay = (asset.TimePaid.Year == intervalDay.Year && asset.TimePaid.Month == intervalDay.Month && asset.TimePaid.GetWeekOfMonth() == intervalDay.GetWeekOfMonth())
                                        ? true : false;
                            pay = getPay(0);
                            break;
                    }
                    break;

                case ScheduleTypes.Monthly:
                    intervalDay = start.AddMonths(interval);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            isPayDay = true;
                            int? payedHours = (asset.BeginDate.Value == null && asset.EndDate.Value == null)
                                                ? 8 : asset.EndDate?.Hour - asset.BeginDate?.Hour;
                            pay = getPay(payedHours.Value * 5 * 4); //  lastPayment + ((((asset.Cost * asset.RateMultilplier) * payedHours.Value) * 5) * 4);
                            break;

                        case ScheduleTypes.Daily:
                            isPayDay = true;
                            pay = getPay(5 * 4);    //lastPayment + (((asset.Cost * asset.RateMultilplier) * 5) * 4);
                            break;

                        case ScheduleTypes.Weekly:
                            isPayDay = true;
                            pay = getPay(4);        //lastPayment + ((asset.Cost * asset.RateMultilplier) * 4);
                            break;

                        case ScheduleTypes.BiWeekly:
                            isPayDay = true;
                            pay = getPay(intervalDay.NumberOfDaysInMonth(asset.TimePaid.DayOfWeek) == 4 ? 2 : 3); 
                            break;

                        case ScheduleTypes.SemiMonthly:
                            isPayDay = true;
                            pay = getPay(2);
                            break;

                        case ScheduleTypes.Monthly:
                            isPayDay = true;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Quarterly:
                            isPayDay = intervalDay.Month.In(1, 4, 7, 10) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.BiYearly:
                            isPayDay = (asset.TimePaid.Month == intervalDay.Month || asset.TimePaid.AddMonths(6).Month == intervalDay.Month) ? true : false;
                            pay = getPay(0);
                            break;

                        case ScheduleTypes.Yearly:
                            isPayDay = (asset.TimePaid.Month == intervalDay.Month) ? true : false;
                            pay = getPay(0);
                            break;


                        default:
                            isPayDay = (asset.TimePaid.Year == intervalDay.Year && asset.TimePaid.Month == intervalDay.Month) ? true : false;
                            pay = getPay(0);
                            break;
                    }
                    break;

                case ScheduleTypes.Yearly:
                    intervalDay = start.AddYears(interval);

                    switch (asset.PaySchedule)
                    {
                        case ScheduleTypes.Hourly:
                            isPayDay = true;
                            int? payedHours = (asset.BeginDate.Value == null && asset.EndDate.Value == null)
                                                ? 8 : asset.EndDate?.Hour - asset.BeginDate?.Hour;
                            pay = getPay(payedHours.Value * 5 * 4 * 12);
                            break;

                        case ScheduleTypes.Daily:
                            isPayDay = true;
                            pay = getPay(5 * 4 * 12);
                            break;

                        case ScheduleTypes.Weekly:
                            isPayDay = true;
                            pay = getPay(4 * 12);
                            break;

                        case ScheduleTypes.BiWeekly:
                            isPayDay = true;
                            pay = getPay(intervalDay.NumberOfDaysInMonth(asset.TimePaid.DayOfWeek) * 12);
                            break;

                        case ScheduleTypes.SemiMonthly:
                            isPayDay = true;
                            pay = getPay(2 * 12);
                            break;

                        case ScheduleTypes.Monthly:
                            isPayDay = true;
                            pay = getPay(12);
                            break;

                        case ScheduleTypes.Quarterly:
                            isPayDay = true;
                            pay = getPay(4);
                            break;

                        case ScheduleTypes.BiYearly:
                            isPayDay = true;
                            pay = getPay(2);
                            break;

                        case ScheduleTypes.Yearly:
                            isPayDay = true;
                            pay = pay = getPay(0);
                            break;


                        default:
                            isPayDay = (asset.TimePaid.Year == intervalDay.Year && asset.TimePaid.Month == intervalDay.Month && asset.TimePaid.GetWeekOfMonth() == intervalDay.GetWeekOfMonth())
                                     ? true : false;
                            pay = getPay(0);
                            break;
                    }
                    break;

            }

            return pay;
        }


        public List<Expense> GetAllExpenses(string userId)
        {
            List<Expense> expenses = _expenseSrv.Where((a) => a.UserId == userId)?.ToList();
            expenses?.Sort((a, b) => DateTime.Compare(a.DateModified.Value, b.DateModified.Value));
            return expenses;
        }

        public List<Income> GetAllIncome(string userId)
        {
            List<Income> incomes = _incomeSrv.Where((a) => a.UserId == userId)?.ToList();
            incomes?.Sort((a, b) => DateTime.Compare(a.DateModified.Value, b.DateModified.Value));
            return incomes;
        }

        public Expense GetExpense(string userId, int id)
        {
            return _expenseSrv.Get(id);
        }

        public Expense GetExpense(string userId, string expenseName)
        {
            return _expenseSrv.Where(a => a.UserId == userId && a.Name == expenseName).FirstOrDefault();
        }

        public List<Expense> GetExpenses(string userId, ExpenseType type)
        {
            return _expenseSrv.Where((a) => a.UserId == userId && a.ExpenseType == type).ToList();
        }

        public List<Expense> GetExpenses(string userId, ScheduleTypes type)
        {
            return _expenseSrv.Where((a) => a.UserId == userId && a.PaySchedule == type).ToList();
        }

        public List<Expense> GetExpenses(string userId, ExpenseType billType, ScheduleTypes scheduleType)
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

        public List<Income> GetIncomes(string userId, IncomeType type)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.IncomeType == type).ToList();
        }

        public List<Income> GetIncomes(string userId, ScheduleTypes type)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.PaySchedule == type).ToList();
        }

        public List<Income> GetIncomes(string userId, IncomeType incomeType, ScheduleTypes scheduleType)
        {
            return _incomeSrv.Where((a) => a.UserId == userId && a.IncomeType == incomeType && a.PaySchedule == scheduleType).ToList();
        }

        public Chart<List<float>> GetIncomeChart(string userId, out ScheduleTypes chartSchedule, DateTime? startDate = null, DateTime? endDate = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();
            List<Income> incomes = GetAllIncome(userId);

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.AddDays(14) : endDate.Value;

            TimeSpan diff = (end - start);
            int numOfAssetsSkipped = 0;

            result.Labels = CalculateLabelRange(out chartSchedule, start, end);
            result.Name = String.Format("Income {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartType.Line;
            result.UserId = userId;

            if (result.Series == null)
                result.Series = new List<List<float>>();
            if (result.Legend == null)
                result.Legend = new List<string>();

            if (incomes != null && incomes.Count > 0)
                for (int n = 0; n < incomes.Count; n++)
                {
                    if (incomes[n].IsHiddenOnChart) { numOfAssetsSkipped++; continue; }

                    result.Series.Add(new List<float>());
                    result.Legend.Add(incomes[n].Name);

                    for (int i = 0; i < result.Labels.Count; i++)
                    {
                        float pay = PayOfDay(start, i, chartSchedule, incomes[n]);
                        result.Series[n - numOfAssetsSkipped].Add(pay);
                    }
                }


            return result;
        }

        public Chart<List<float>> GetExpensesChart(string userId, out ScheduleTypes chartSchedule, DateTime? startDate = null, DateTime? endDate = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();
            List<Expense> expenses = GetAllExpenses(userId);

            DateTime start = (startDate == null) ? DateTime.Now : startDate.Value,
                     end = (endDate == null) ? DateTime.Now.AddDays(14) : endDate.Value;

            TimeSpan diff = (end - start);
            int numOfAssetsSkipped = 0;

            result.Labels = CalculateLabelRange(out chartSchedule, start, end);
            result.Name = String.Format("Expenses {0} Chart", chartSchedule.ToString());
            result.TypeId = ChartType.Line;
            result.UserId = userId;

            if (result.Series == null)
                result.Series = new List<List<float>>();
            if (result.Legend == null)
                result.Legend = new List<string>();

            if (expenses != null && expenses.Count > 0)
                for (int n = 0; n < expenses.Count; n++)
                {
                    if (expenses[n].IsHiddenOnChart) { numOfAssetsSkipped++; continue; }

                    result.Series.Add(new List<float>());
                    result.Legend.Add(expenses[n].Name);

                    for (int i = 0; i < result.Labels.Count; i++)
                    {
                        float pay = PayOfDay(start, i, chartSchedule, expenses[n]) * -1;
                        result.Series[n - numOfAssetsSkipped].Add(pay);
                    }
                }


            return result;
        }

        public Chart<List<float>> GetCombinedChart(string userId, out ScheduleTypes chartSchedule, DateTime? startDate = null, DateTime? endDate = null)
        {
            Chart<List<float>> result = new Chart<List<float>>();

            Chart<List<float>> incomeChart = GetIncomeChart(userId, out chartSchedule, startDate, endDate);
            Chart<List<float>> expensesChart = GetExpensesChart(userId, out chartSchedule, startDate, endDate);

            result.Labels = CalculateLabelRange(out chartSchedule, (startDate != null) ? startDate.Value : DateTime.Now, (endDate != null) ? endDate.Value : DateTime.Now.AddDays(14));
            result.Name = String.Format("Complete {0} Chart", chartSchedule.ToString());
            result.UserId = userId;
            result.TypeId = ChartType.Line;
            result.Series = (incomeChart.Series.Count > 0 && expensesChart.Series.Count > 0)
                                ? incomeChart.Series.Concat(expensesChart.Series).ToList()
                                : (incomeChart.Series.Count > 0)
                                ? incomeChart.Series
                                : (expensesChart.Series.Count > 0)
                                ? expensesChart.Series
                                : new List<List<float>>();
            result.Legend = (incomeChart.Legend.Count > 0 && expensesChart.Legend.Count > 0)
                                ? incomeChart.Legend.Concat(expensesChart.Legend).ToList()
                                : (incomeChart.Legend.Count > 0)
                                ? incomeChart.Legend
                                : (expensesChart.Legend.Count > 0)
                                ? expensesChart.Legend
                                : new List<string>();

            return result;
        }

        public void InsertExpense(Expense request)
        {
            request.UserId = request.UserId ?? throw new Exception("request.UserId cannot be null to be able to InsertExpense");
            request.ModifiedUserId = request.ModifiedUserId ?? request.UserId;
            request.DateCreated = (request.DateCreated == null) ? DateTime.Now : request.DateCreated;
            request.DateModified = (request.DateModified == null) ? DateTime.Now : request.DateModified;
            _expenseSrv.Insert(request);
        }

        public void InsertIncome(Income request)
        {
            request.UserId = request.UserId ?? throw new Exception("request.UserId cannot be null to be able to InsertIncome");
            request.ModifiedUserId = request.ModifiedUserId ?? request.UserId;
            request.DateCreated = (request.DateCreated == null) ? DateTime.Now : request.DateCreated;
            request.DateModified = (request.DateModified == null) ? DateTime.Now : request.DateModified;
            _incomeSrv.Insert(request);
        }

        public void UpdateExpense(Expense request)
        {
            request.ModifiedUserId = request.ModifiedUserId ?? throw new Exception("request.ModifiedUserId cannot be null to be able to UpdateExpense");
            request.DateModified = (request.DateModified == null) ? DateTime.Now : request.DateModified;
            _expenseSrv.Update(request);
        }

        public void UpdateIncome(Income request)
        {
            request.ModifiedUserId = request.ModifiedUserId ?? throw new Exception("request.ModifiedUserId cannot be null to be able to UpdateIncome");
            request.DateModified = (request.DateModified == null) ? DateTime.Now : request.DateModified;
            _incomeSrv.Update(request);
        }

        public void DeleteExpense(int id)
        {
            _expenseSrv.Delete(id);
        }

        public void DeleteIncome(int id)
        {
            _incomeSrv.Delete(id);
        }


    }
}
