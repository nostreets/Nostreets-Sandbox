using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Models.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IDBService<Type, IdType, AddType, UpdateType>
    {

        List<Type> GetAll();
        Type Get(IdType id);
        IdType Insert(AddType model);
        void Delete(IdType id);
        void Update(UpdateType model);


    }

    public interface IDBService<T>
    {

        List<T> GetAll();
        T Get(object id);
        object Insert(T model);
        void Delete(object id);
        void Update(object model);


    }

    public interface IChartsExtended : IDBService<Chart<object>, int, ChartAddRequest, ChartUpdateRequest>
    {
        int Insert<T>(ChartAddRequest<T> model);
        void Update<T>(ChartUpdateRequest<T> model);
    }
}
