using Newtonsoft.Json;
using Nostreets_Services.Domain.Charts;
using Nostreets_Services.Interfaces.Services;
using Nostreets_Services.Models.Request;
using Nostreets_Services.Providers;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Nostreets_Services.Services.Database
{
    public class ChartsService : BaseService, IDBService<Chart, int, ChartAddRequest, ChartUpdateRequest>
    {
        public ChartsService() : base() { }

        public ChartsService(string connectionKey) : base(connectionKey) { }

        public List<Chart> GetAll()
        {
            List<Chart> list = null;
            DataProvider.ExecuteCmd(() => Connection, "dbo.Charts_SelectAll",
                null,
                (reader, set) =>
                {
                    Chart chart = DataMapper<Chart>.Instance.MapToObject(reader);
                    if (list == null) { list = new List<Chart>(); }
                    list.Add(chart);
                });
            list.ForEach(a =>
            {
                if (a.TypeId == Enums.ChartTypes.Pie && a.Series == null)
                {
                    a = Get(a.ChartId);
                }
            });
            return list;
        }

        public void Delete(int id)
        {
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Delete",
                param => param.Add(new SqlParameter("ChartId", id)));
        }

        public Chart Get(int id)
        {
            Chart chart = null;
            DataProvider.ExecuteCmd(() => Connection, "dbo.Charts_SelectById",
                param => param.Add(new SqlParameter("ChartId", id)),
                (reader, set) =>
                {
                    chart = DataMapper<Chart>.Instance.MapToObject(reader);
                });
            if (chart.TypeId == Enums.ChartTypes.Pie && chart.Series == null)
            {
                DataProvider.ExecuteCmd(() => Connection, "dbo.Charts_SelectById",
                param => param.Add(new SqlParameter("ChartId", id)),
                (reader, set) =>
                {
                    chart = DataMapper<Chart<int>>.Instance.MapToObject(reader);
                });
            }
            return chart;
        }


        public int Insert(ChartAddRequest model)
        {
            int id = 0;
            DataProvider.ExecuteNonQuery(() => Connection, "dbo.Charts_Insert",
                       param => param.AddRange(new[] {
                           new SqlParameter("Name", model.Name),
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
                           new SqlParameter("TypeId", (int)model.TypeId),
                           model.Legend != null ? new SqlParameter("Legend", JsonConvert.SerializeObject(model.Legend)) : new SqlParameter("Legend", null),
                           model.Series != null ? new SqlParameter("Series", JsonConvert.SerializeObject(model.Series)) : new SqlParameter("Series", null),
                           model.Labels != null ? new SqlParameter("Labels", JsonConvert.SerializeObject(model.Labels)) : new SqlParameter("Labels", null)
                       }));
        }
    }
}