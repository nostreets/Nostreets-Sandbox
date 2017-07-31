using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IChartsExtended : IDBContract<Chart<object>, int, ChartAddRequest, ChartUpdateRequest>
    {
        int Insert<T>(ChartAddRequest<T> model);
        void Update<T>(ChartUpdateRequest<T> model);
    }

    public interface IDBContract<Type, IdType, AddType, UpdateType>
    {

        List<Type> GetAll();
        Type Get(IdType id);
        IdType Insert(AddType model);
        void Delete(IdType id);
        void Update(UpdateType model);


    }
}
