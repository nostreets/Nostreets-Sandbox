using Newtonsoft.Json;
using NostreetsORM;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using System.Collections.Generic;
using System.Data.SqlClient;
using NostreetsExtensions.Helpers;
using System;
using System.Linq;
using NostreetsExtensions.Interfaces;
using NostreetsExtensions;

namespace Nostreets_Services.Services.Database
{
    public class ChartsService : IChartSrv
    {
        public ChartsService()
        {
            _chartSrv = new DBService<Chart<List<int>>, int, ChartAddRequest, ChartUpdateRequest>();
            _pieChartSrv = new DBService<Chart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>>();
        }

        public ChartsService(string connectionKey)
        {
            _chartSrv = new DBService<Chart<List<int>>, int, ChartAddRequest, ChartUpdateRequest>(connectionKey);
            _pieChartSrv = new DBService<Chart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>>(connectionKey);
        }

        DBService<Chart<List<int>>, int, ChartAddRequest, ChartUpdateRequest> _chartSrv = null;
        DBService<Chart<int>, int, ChartAddRequest<int>, ChartUpdateRequest<int>> _pieChartSrv = null;

        public void Delete(int id)
        {
            _chartSrv.Delete(id);
        }

        public Chart<object> Get(int id)
        {
            return (Chart<object>)typeof(Chart<object>).Cast(_chartSrv.Get(id));
        }

        public List<Chart<object>> GetAll()
        {
            List<Chart<object>> result = null;
            var charts =_chartSrv.GetAll();
            var pieCharts = _pieChartSrv.GetAll();
            result = charts.Cast<Chart<object>>()
                           .Concat(pieCharts.Cast<Chart<object>>())
                           .ToList();
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
            return _chartSrv.GetAll().Where(predicate);
        }

        public IEnumerable<Chart<int>> Where(Func<Chart<int>, bool> predicate)
        {
            return _pieChartSrv.GetAll().Where(predicate);
        }

    }
}