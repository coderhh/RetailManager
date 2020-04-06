using System.Collections.Generic;

namespace RMDataManager.Library.Models
{
    public class SaleModel
    {
        public List<SaleDetailsModel> SaleDetails { get; set; } = new List<SaleDetailsModel>();
    }
}
