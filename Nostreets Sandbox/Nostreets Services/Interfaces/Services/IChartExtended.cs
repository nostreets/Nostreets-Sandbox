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
    public interface IChartsExtended : IDBService<Chart<object>, int, ChartAddRequest, ChartUpdateRequest>
    {
        int Insert<T>(ChartAddRequest<T> model);
        void Update<T>(ChartUpdateRequest<T> model);
    }

   
}
