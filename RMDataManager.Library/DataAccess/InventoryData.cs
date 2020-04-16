using System.Collections.Generic;

using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class InventoryData : IInventoryData
    {
        private readonly ISqlDataAccess _sql;

        public InventoryData()
        {

        }

        public InventoryData(ISqlDataAccess sql)
        {
            _sql = sql;
        }
        public List<InventoryModel> GetInventory()
        {
            var output = _sql.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "RMData");
            return output;
        }

        public void SaveInventoryRecord(InventoryModel item)
        {
            _sql.SaveData("dbo.sqInventory_Insert", item, "RMData");
        }

    }
}
