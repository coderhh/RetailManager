﻿using Microsoft.Extensions.Configuration;
using RMDataManager.Library.Internal.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace RMDataManager.Library.DataAccess
{
    public class SaleData : ISaleData
    {
        private readonly IProductData _prodcutData;
        private readonly ISqlDataAccess _sql;
        private readonly IConfiguration _config;

        public SaleData(IProductData prodcutData, ISqlDataAccess sql, IConfiguration config)
        {
            _prodcutData = prodcutData;
            _sql = sql;
            _config = config;
        }

        public SaleData()
        {
        }

        public void SaveSale(SaleModel saleInfo, string cashierId)
        {
            List<SaleDetailsDBModel> details = new List<SaleDetailsDBModel>();

            decimal taxRate = GetTaxRate();

            foreach (var item in saleInfo.SaleDetails)
            {
                var saleDetails = new SaleDetailsDBModel
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };

                var productInfo = _prodcutData.GetProductById(item.ProductId);
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

            try
            {
                _sql.StartTransaction("RMData");
                _sql.SaveDataInTransaction("dbo.spSale_Insert", saleToDb);

                saleToDb.Id = _sql.LoadDataInTransaction<int, dynamic>("dbo.spSale_Lookup", new { saleToDb.CashierId, saleToDb.SaleDate }).FirstOrDefault();

                foreach (var item in details)
                {
                    item.SaleId = saleToDb.Id;
                    _sql.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
                }

                _sql.CommitTransaction();
            }
            catch
            {
                _sql.RollbackTransaction();
                throw;
            }

        }

        public List<SaleReportModel> GetSaleReport()
        {
            var output = _sql.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "RMData");
            return output;
        }

        private decimal GetTaxRate()
        {
            var taxRateStr = _config.GetSection("AppSettings").GetSection("taxRate").Value;
            if (!decimal.TryParse(taxRateStr, out decimal taxRate))
            {
                throw new ConfigurationErrorsException("The tax rate is not set up properly");
            }

            return taxRate / 100;
        }

    }
    
}
