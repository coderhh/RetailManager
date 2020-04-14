using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace RMDataManager.Library.DataAccess
{
    public class ProductData
    {
        private readonly IConfiguration config;

        public ProductData(IConfiguration config)
        {
            this.config = config;
        }

        public ProductData()
        {
        }

        public List<ProductModel> GetProducts()
        {
            SqlDataAccess sql = GetSqlDataAccessInstance();

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "RMData");

            return output;
        }

        public ProductModel GetProductById(int productId)
        {
            SqlDataAccess sql = GetSqlDataAccessInstance();

            var output = sql.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "RMData").FirstOrDefault() ;

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
