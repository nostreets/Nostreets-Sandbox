using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using System.Collections.Generic;
using System;
using System.Linq;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions.DataControl.Classes;
using NostreetsExtensions.Extend.Basic;

namespace Nostreets_Services.Services.Database
{
    public class ChartsService : IChartService
    {
        #region Public Constructors

        public ChartsService(
              IDBService<ChartistChart<List<int>>, int, ChartAddRequest, ChartUpdateRequest> chartDBSrv
            , IDBService<ChartistChart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>> pieChartDBSrv)
        {
            _chartSrv = chartDBSrv;
            _pieChartSrv = pieChartDBSrv;

        }

        #endregion Public Constructors

        #region Private Fields

        IDBService<ChartistChart<List<int>>, int, ChartAddRequest, ChartUpdateRequest> _chartSrv = null;
        IDBService<ChartistChart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>> _pieChartSrv = null;

        #endregion Private Fields

        #region Public Methods

        public void Delete(int id)
        {
            _chartSrv.Delete(id);
        }

        public ChartistChart<object> Get(int id)
        {
            return (ChartistChart<object>)typeof(ChartistChart<object>).Cast(_chartSrv.Get(id));
        }

        public List<ChartistChart<object>> GetAll()
        {
            List<ChartistChart<object>> result = null;
            var charts = _chartSrv.GetAll().Where(a => a.Series != null);
            var pieCharts = _pieChartSrv.GetAll().Where(a => a.Series != null);

            if (pieCharts == null && charts != null)
                foreach (ChartistChart<List<int>> c in charts)
                {
                    ChartistChart<object> newChart = new ChartistChart<object>
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
                        result = new List<ChartistChart<object>>();
                    result.Add(newChart);
                }
            else if (charts == null && pieCharts != null)
                foreach (ChartistChart<int> p in pieCharts)
                {
                    ChartistChart<object> newChart = new ChartistChart<object>
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
                        result = new List<ChartistChart<object>>();
                    result.Add(newChart);
                }
            else if (charts != null && pieCharts != null)
            {
                foreach (ChartistChart<int> p in pieCharts)
                {
                    ChartistChart<object> newChart = new ChartistChart<object>
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
                        result = new List<ChartistChart<object>>();
                    result.Add(newChart);
                }
                foreach (ChartistChart<List<int>> c in charts)
                {
                    ChartistChart<object> newChart = new ChartistChart<object>
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
                        result = new List<ChartistChart<object>>();
                    result.Add(newChart);
                }
            }



            return result;
        }

        public int Insert(ChartistChart<List<int>> model)
        {
            return _chartSrv.Insert(model);
        }

        public int Insert(ChartAddRequest model, Converter<ChartAddRequest, ChartistChart<List<int>>> converter)
        {
            return _chartSrv.Insert(converter(model));
        }

        public int Insert(ChartistChart<int> model)
        {
            return _pieChartSrv.Insert(model);
        }

        public int Insert(ChartAddRequest<int> model, Converter<ChartAddRequest<int>, ChartistChart<int>> converter)
        {
            return _pieChartSrv.Insert(converter(model));
        }

        public void Update(ChartUpdateRequest model, Converter<ChartUpdateRequest, ChartistChart<List<int>>> converter)
        {
            _chartSrv.Update(converter(model));
        }

        public void Update(ChartistChart<List<int>> model)
        {
            _chartSrv.Update(model);
        }

        public void Update(ChartUpdateRequest<int> model, Converter<ChartUpdateRequest<int>, ChartistChart<int>> converter)
        {
            _pieChartSrv.Update(converter(model));
        }

        public void Update(ChartistChart<int> model)
        {
            _pieChartSrv.Update(model);
        }

        public IEnumerable<ChartistChart<List<int>>> Where(Func<ChartistChart<List<int>>, bool> predicate)
        {
            return _chartSrv.Where(predicate);
        }

        public IEnumerable<ChartistChart<int>> Where(Func<ChartistChart<int>, bool> predicate)
        {
            return _pieChartSrv.Where(predicate);
        }

        #endregion Public Methods

    }
}