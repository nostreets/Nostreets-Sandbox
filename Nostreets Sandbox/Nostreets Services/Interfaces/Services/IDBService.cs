using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface IDBService<T, IdType, AddType, UpdateType>
    {

        List<T> GetAll();
        T Get(IdType id);
        int Insert(AddType model);
        void Delete(IdType id);
        void Update(UpdateType model);
    }
}
