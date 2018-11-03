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
        List<Chart<object>> GetAll();
        Chart<object> Get(int id);
        int Insert(Chart<List<int>> model);
        int Insert(ChartAddRequest model, Converter<ChartAddRequest, Chart<List<int>>> converter);
        int Insert(Chart<int> model);
        int Insert(ChartAddRequest<int> model, Converter<ChartAddRequest<int>, Chart<int>> converter);
        void Delete(int id);
        void Update(ChartUpdateRequest model, Converter<ChartUpdateRequest, Chart<List<int>>> converter);
        void Update(Chart<List<int>> model);
        void Update(Chart<int> model);
        void Update(ChartUpdateRequest<int> model, Converter<ChartUpdateRequest<int>, Chart<int>> converter);
        IEnumerable<Chart<List<int>>> Where(Func<Chart<List<int>>, bool> predicate);
        IEnumerable<Chart<int>> Where(Func<Chart<int>, bool> predicate);
    }


}
