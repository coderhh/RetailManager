using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Library.DataAccess
{
    public class InventoryData
    {
        private readonly IConfiguration config;

        public InventoryData()
        {
           
        }

        public InventoryData(IConfiguration config)
        {
            this.config = config;
        }
        public List<InventoryModel> GetInventory()
        {
            SqlDataAccess sql = GetSqlDataAccessInstance();

            var output = sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMData");
            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            SqlDataAccess sql = GetSqlDataAccessInstance();
            sql.SaveData("dbo.sqInventory_Insert", item, "RMData");
        }

        private SqlDataAccess GetSqlDataAccessInstance()
        {
            SqlDataAccess sql;
            if (config == null)
            {
                sql = new SqlDataAccess();
            }
            else
            {
                sql = new SqlDataAccess(config);
            }

            return sql;
        }
    }
}
