using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Request;
using NostreetsExtensions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IChartService 
    {
        List<ChartistChart<object>> GetAll();
        ChartistChart<object> Get(int id);
        int Insert(ChartistChart<List<int>> model);
        int Insert(ChartAddRequest model, Converter<ChartAddRequest, ChartistChart<List<int>>> converter);
        int Insert(ChartistChart<int> model);
        int Insert(ChartAddRequest<int> model, Converter<ChartAddRequest<int>, ChartistChart<int>> converter);
        void Delete(int id);
        void Update(ChartUpdateRequest model, Converter<ChartUpdateRequest, ChartistChart<List<int>>> converter);
        void Update(ChartistChart<List<int>> model);
        void Update(ChartistChart<int> model);
        void Update(ChartUpdateRequest<int> model, Converter<ChartUpdateRequest<int>, ChartistChart<int>> converter);
        IEnumerable<ChartistChart<List<int>>> Where(Func<ChartistChart<List<int>>, bool> predicate);
        IEnumerable<ChartistChart<int>> Where(Func<ChartistChart<int>, bool> predicate);
    }


}
