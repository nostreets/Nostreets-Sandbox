using System.Collections.Generic;

namespace NostreetsORM.Interfaces
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
}
