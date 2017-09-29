using Newtonsoft.Json;
using NostreetsORM;
using NostreetsORM.Utilities;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Nostreets_Services.Services.Database
{
    public class ChartsService : BaseService, IChartsExtended
    {
        public ChartsService() : base() { }

        public ChartsService(string connectionKey) : base(connectionKey) { }

        public List<Chart<object>> GetAll()
        {
            List<Chart<object>> list = null;
            DataProvider.ExecuteCmd(() => Connection, "dbo.Charts_SelectAll",
                null,
                (reader, set) =>
                {
                    Chart<object> chart = DataMapper<Chart<object>>.Instance.MapToObject(reader);
                    if (list == null) { list = new List<Chart<object>>(); }
                    list.Add(chart);
                });
            return list;
        }

        public void Delete(int id)
        {
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Delete",
                param => param.Add(new SqlParameter("ChartId", id)));
        }

        public Chart<object> Get(int id)
        {
            Chart<object> chart = null;
            DataProvider.ExecuteCmd(() => Connection, "dbo.Charts_SelectById",
                param => param.Add(new SqlParameter("ChartId", id)),
                (reader, set) =>
                {
                    chart = DataMapper<Chart<object>>.Instance.MapToObject(reader);
                });
            return chart;
        }

        public int Insert(ChartAddRequest model)
        {
            int id = 0;
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Insert",
                       param => param.AddRange(new[] {
                           new SqlParameter("Name", model.Name),
                           new SqlParameter("UserId", model.UserId),
                           new SqlParameter("TypeId", (int)model.TypeId),
                           new SqlParameter("Legend", JsonConvert.SerializeObject(model.Legend)),
                           new SqlParameter("Series", JsonConvert.SerializeObject(model.Series)),
                           new SqlParameter("Labels", JsonConvert.SerializeObject(model.Labels)),
                           new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output  }
                       }),
                       param => int.TryParse(param["@Id"].Value.ToString(), out id));
            return id;
        }

        public int Insert<T>(ChartAddRequest<T> model)
        {
            int id = 0;
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Insert",
                       param => param.AddRange(new[] {
                           new SqlParameter("Name", model.Name),
                           new SqlParameter("UserId", model.UserId),
                           new SqlParameter("TypeId", (int)model.TypeId),
                           new SqlParameter("Legend", JsonConvert.SerializeObject(model.Legend)),
                           new SqlParameter("Series", JsonConvert.SerializeObject(model.Series)),
                           new SqlParameter("Labels", JsonConvert.SerializeObject(model.Labels)),
                           new SqlParameter { ParameterName = "@Id", DbType = System.Data.DbType.Int32, Direction = System.Data.ParameterDirection.Output  }
                       }),
                       param => int.TryParse(param["@Id"].Value.ToString(), out id));
            return id;
        }

        public void Update(ChartUpdateRequest model)
        {
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Update",
                       param => param.AddRange(new[] {
                           new SqlParameter("ChartId", model.ChartId),
                           model.Name != null ? new SqlParameter("Name", model.Name) : new SqlParameter("Name", null),
                           new SqlParameter("UserId", model.UserId),
                           new SqlParameter("TypeId", (int)model.TypeId),
                           model.Legend != null ? new SqlParameter("Legend", JsonConvert.SerializeObject(model.Legend)) : new SqlParameter("Legend", null),
                           model.Series != null ? new SqlParameter("Series", JsonConvert.SerializeObject(model.Series)) : new SqlParameter("Series", null),
                           model.Labels != null ? new SqlParameter("Labels", JsonConvert.SerializeObject(model.Labels)) : new SqlParameter("Labels", null)
                       }));
        }

        public void Update<T>(ChartUpdateRequest<T> model)
        {
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Update",
                       param => param.AddRange(new[] {
                           new SqlParameter("ChartId", model.ChartId),
                           model.Name != null ? new SqlParameter("Name", model.Name) : new SqlParameter("Name", null),
                           new SqlParameter("UserId", model.UserId),
                           new SqlParameter("TypeId", (int)model.TypeId),
                           model.Legend != null ? new SqlParameter("Legend", JsonConvert.SerializeObject(model.Legend)) : new SqlParameter("Legend", null),
                           model.Series != null ? new SqlParameter("Series", JsonConvert.SerializeObject(model.Series)) : new SqlParameter("Series", null),
                           model.Labels != null ? new SqlParameter("Labels", JsonConvert.SerializeObject(model.Labels)) : new SqlParameter("Labels", null)
                       }));
        }

    }
}