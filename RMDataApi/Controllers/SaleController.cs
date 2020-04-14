using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration config;

        public SaleController(IConfiguration config)
        {
            this.config = config;
        }
        [Authorize(Roles = "Cashier")]
        [HttpPost]
        public void Post(SaleModel sale)
        {
            SaleData data = new SaleData(config);
            var cashierId = User.FindFirstValue(ClaimTypes.NameIdentifier);//RequestContext.Principal.Identity.GetUserId();
            data.SaveSale(sale, cashierId);
        }
        [Authorize(Roles = "Admin, Manager")]
        [Route("GetSalesReport")]
        [HttpGet]
        public List<SaleReportModel> GetSalesReport()
        {
            SaleData data = new SaleData(config);
            var result = data.GetSaleReport();
            return result;
        }
    }
}
