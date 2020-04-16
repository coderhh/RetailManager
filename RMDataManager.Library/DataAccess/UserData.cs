using System.Collections.Generic;

using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;

namespace RMDataManager.Library.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sql;

        public UserData()
        {
        }

        public UserData(ISqlDataAccess sql)
        {
            _sql = sql;
        }
        public List<UserModel> GetUserById(string Id)
        {
            var output = _sql.LoadData<UserModel, dynamic>("dbo.spUserLookUp", new { Id }, "RMData");

            return output;
        }
    }
}
