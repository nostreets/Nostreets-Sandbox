using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nostreets_Services.Interfaces.Services
{
    public interface ICacheService
    {
        void Add(string key, object o, DateTimeOffset expiration, string dependsOnKey = null);
        void Add(string key, object o, string dependsOnKey = null);
        global::System.Collections.Generic.IEnumerable<string> AllKeys { get; }
        bool Contains(string key);
        object Get(string key);
        T Get<T>(string key) where T : class;
        void Remove(string key);
        void RemoveStartsWith(string key);
    }
}
