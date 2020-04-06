using System.Configuration;

namespace RMDataManager.Library
{
    public class ConfigHelper
    {
        public decimal GetTaxTate()
        {
            string rateTax = ConfigurationManager.AppSettings["taxRate"];

            if (!decimal.TryParse(rateTax, out decimal output))
            {
                throw new ConfigurationErrorsException("The tax rate is not set up properly");
            }

            return output;
        }
    }
}
