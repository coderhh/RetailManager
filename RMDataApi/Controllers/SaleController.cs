using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace RMDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaleController : ControllerBase
    {
        private readonly ISaleData _saleData;
        private readonly ILogger<SaleController> _logger;

        public SaleController(ISaleData saleData, ILogger<SaleController> logger)
        {
            _saleData = saleData;
            _logger = logger;
        }
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _logger.LogInformation("Cashier {Cashier} checked out", cashierId);
            _saleData.SaveSale(sale, cashierId);
        }
        [Authorize(Roles = "Admin, Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            var result = _saleData.GetSaleReport();
            return result;
        }
    }
}
