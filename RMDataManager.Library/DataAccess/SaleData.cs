using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {
        private readonly IConfiguration config;

        public SaleData(IConfiguration config)
        {
            this.config = config;
        }
        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailsDBModel> details = new List<SaleDetailsDBModel>();

            decimal taxRate = ConfigHelper.GetTaxTate() / 100;

            foreach (var item in saleInfo.SaleDetails)
            {
                var saleDetails = new SaleDetailsDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };
                ProductData productData = new ProductData(config);
                var productInfo = productData.GetProductById(item.ProductId);
                saleDetails.PurchasePrice = productInfo.RetailPrice * saleDetails.Quantity;
                if (productInfo.IsTaxable)
                {
                    saleDetails.Tax = saleDetails.PurchasePrice * taxRate;
                }

                details.Add(saleDetails);
            }

            var saleToDb = new SaleDBModel()
            {
                CashierId = cashierId,
                SubTotal = details.Sum(x => x.PurchasePrice),
                Tax = details.Sum(x => x.Tax)
            };

            saleToDb.Total = saleToDb.SubTotal + saleToDb.Tax;

            using (SqlDataAccess sql = new SqlDataAccess(config))
            {
                try
                {
                    sql.StartTransaction("RMData");
                    sql.SaveDataInTransaction("dbo.spSale_Insert", saleToDb);

                    saleToDb.Id = sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { saleToDb.CashierId, saleToDb.SaleDate }).FirstOrDefault();

                    foreach (var item in details)
                    {
                        item.SaleId = saleToDb.Id;
                        sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                    }

                    sql.CommitTransaction();
                }
                catch
                {
                    sql.RollbackTransaction();
                    throw;
                }
            }
        }
        public List<SaleReportModel> GetSaleReport()
        {
            SqlDataAccess sql = new SqlDataAccess(config);
            var output = sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { },"RMData");
            return output;
        }
    }
}
