using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RMDataManager.Library.DataAccess;
using RMDataManager.Library.Models;
using System.Collections.Generic;

namespace RMDataApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductData productData;

        public ProductController(IProductData productData)
        {
            this.productData = productData;
        }
        [HttpGet]
        public List<ProductModel> Get()
        {
            return productData.GetProducts();
        }
    }
}
