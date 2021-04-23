using Nostreets_Services.Classes.Domain.Charts;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using System.Collections.Generic;
using System;
using System.Linq;
using Nostreets.Extensions.Interfaces;
using Nostreets.Extensions.DataControl.Classes;
using Nostreets.Extensions.Extend.Basic;

namespace Nostreets_Services.Services.Database
{
    public class ChartsService : IChartService
    {
        #region Public Constructors

        public ChartsService(
              IDBService<Chart<List<int>>, int, ChartAddRequest, ChartUpdateRequest> chartDBSrv
            , IDBService<Chart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>> pieChartDBSrv)
        {
            _chartSrv = chartDBSrv;
            _pieChartSrv = pieChartDBSrv;

        }

        #endregion Public Constructors

        #region Private Fields

        IDBService<Chart<List<int>>, int, ChartAddRequest, ChartUpdateRequest> _chartSrv = null;
        IDBService<Chart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>> _pieChartSrv = null;

        #endregion Private Fields

        #region Public Methods

        public void Delete(int id)
        {
            _chartSrv.Delete(id);
        }

        public Chart<object> Get(int id)
        {
            return _chartSrv.Get(id).Cast<Chart<object>>();
        }

        public List<Chart<object>> GetAll()
        {
            List<Chart<object>> result = null;
            var charts = _chartSrv.GetAll().Where(a => a.Series != null);
            var pieCharts = _pieChartSrv.GetAll().Where(a => a.Series != null);

            if (pieCharts == null && charts != null)
                foreach (Chart<List<int>> c in charts)
                {
                    Chart<object> newChart = new Chart<object>
                    {
                        ChartId = c.ChartId,
                        DateCreated = c.DateCreated,
                        DateModified = c.DateModified,
                        Labels = c.Labels,
                        Legend = c.Legend,
                        Name = c.Name,
                        TypeId = c.TypeId,
                        UserId = c.UserId
                    };
                    newChart.SetPropertyValue("Series", c.Series.Cast(typeof(object)));

                    if (result == null)
                        result = new List<Chart<object>>();
                    result.Add(newChart);
                }
            else if (charts == null && pieCharts != null)
                foreach (Chart<int> p in pieCharts)
                {
                    Chart<object> newChart = new Chart<object>
                    {
                        ChartId = p.ChartId,
                        DateCreated = p.DateCreated,
                        DateModified = p.DateModified,
                        Labels = p.Labels,
                        Legend = p.Legend,
                        Name = p.Name,
                        TypeId = p.TypeId,
                        UserId = p.UserId
                    };
                    newChart.SetPropertyValue("Series", p.Series.Cast(typeof(object)));

                    if (result == null)
                        result = new List<Chart<object>>();
                    result.Add(newChart);
                }
            else if (charts != null && pieCharts != null)
            {
                foreach (Chart<int> p in pieCharts)
                {
                    Chart<object> newChart = new Chart<object>
                    {
                        ChartId = p.ChartId,
                        DateCreated = p.DateCreated,
                        DateModified = p.DateModified,
                        Labels = p.Labels,
                        Legend = p.Legend,
                        Name = p.Name,
                        TypeId = p.TypeId,
                        UserId = p.UserId
                    };
                    newChart.SetPropertyValue("Series", p.Series.Cast(typeof(object)));

                    if (result == null)
                        result = new List<Chart<object>>();
                    result.Add(newChart);
                }
                foreach (Chart<List<int>> c in charts)
                {
                    Chart<object> newChart = new Chart<object>
                    {
                        ChartId = c.ChartId,
                        DateCreated = c.DateCreated,
                        DateModified = c.DateModified,
                        Labels = c.Labels,
                        Legend = c.Legend,
                        Name = c.Name,
                        TypeId = c.TypeId,
                        UserId = c.UserId
                    };
                    newChart.SetPropertyValue("Series", c.Series.Cast(typeof(object)));

                    if (result == null)
                        result = new List<Chart<object>>();
                    result.Add(newChart);
                }
            }



            return result;
        }

        public int Insert(Chart<List<int>> model)
        {
            return _chartSrv.Insert(model);
        }

        public int Insert(ChartAddRequest model, Converter<ChartAddRequest, Chart<List<int>>> converter)
        {
            return _chartSrv.Insert(converter(model));
        }

        public int Insert(Chart<int> model)
        {
            return _pieChartSrv.Insert(model);
        }

        public int Insert(ChartAddRequest<int> model, Converter<ChartAddRequest<int>, Chart<int>> converter)
        {
            return _pieChartSrv.Insert(converter(model));
        }

        public void Update(ChartUpdateRequest model, Converter<ChartUpdateRequest, Chart<List<int>>> converter)
        {
            _chartSrv.Update(converter(model));
        }

        public void Update(Chart<List<int>> model)
        {
            _chartSrv.Update(model);
        }

        public void Update(ChartUpdateRequest<int> model, Converter<ChartUpdateRequest<int>, Chart<int>> converter)
        {
            _pieChartSrv.Update(converter(model));
        }

        public void Update(Chart<int> model)
        {
            _pieChartSrv.Update(model);
        }

        public IEnumerable<Chart<List<int>>> Where(Func<Chart<List<int>>, bool> predicate)
        {
            return _chartSrv.Where(predicate);
        }

        public IEnumerable<Chart<int>> Where(Func<Chart<int>, bool> predicate)
        {
            return _pieChartSrv.Where(predicate);
        }

        #endregion Public Methods

    }
}