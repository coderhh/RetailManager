using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMDataManager.Models
{
    public class SaleModel
    {
        public List<SaleDetailsModel> SaleDetails { get; set; } = new List<SaleDetailsModel>();
    }
}
