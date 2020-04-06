using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;
using System.Linq;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData
    {   
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
                ProductData productData = new ProductData();
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

            SqlDataAccess sql = new SqlDataAccess();
            sql.SaveData("dbo.spSale_Insert", saleToDb, "RMData");

            saleToDb.Id = sql.LoadData<int,dynamic>("dbo.spSale_Lookup", new { saleToDb.CashierId, saleToDb.SaleDate}, "RMData").FirstOrDefault();

            foreach (var item in details)
            {
                item.SaleId = saleToDb.Id;
                sql.SaveData("dbo.spSaleDetail_Insert", item, "RMData");
            }

        }
    }
}
