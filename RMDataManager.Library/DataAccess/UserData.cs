using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataManager.Library.DataAccess
{
    public class UserData
    {
        private readonly IConfiguration config;

        public UserData()
        {
        }

        public UserData(IConfiguration config)
        {
            this.config = config;
        }
        public List<UserModel> GetUserById(string Id)
        {
            SqlDataAccess sql = GetSqlDataAccessInstance();

            var p = new { Id };

            var output = sql.LoadData<UserModel, dynamic>("dbo.spUserLookUp", p, "RMData");

            return output;
        }

        private SqlDataAccess GetSqlDataAccessInstance()
        {
            SqlDataAccess sql;
            if (config == null)
            {
                sql = new SqlDataAccess(config);
            }
            else
            {
                sql = new SqlDataAccess();
            }

            return sql;
        }
    }
}
