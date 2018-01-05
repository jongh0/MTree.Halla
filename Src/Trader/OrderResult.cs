using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class OrderResult
    {
        public string OrderNumber { get; set; }
        
        public string Code { get; set; }

        public OrderResultTypes ResultType { get; set; }

        public int ConcludedQuantity { get; set; }

        public int ConcludedPrice { get; set; }

        public int OrderedQuantity { get; set; }

        public int OrderedPrice { get; set; }

        public string AccountNumber { get; set; }

        public override string ToString()
        {
            List<string> strList = new List<string>();

            try
            {
                foreach (var property in typeof(OrderResult).GetProperties())
                    strList.Add($"{property.Name}: {property.GetValue(this)}");
            }
            catch
            {
            }

            return string.Join(", ", strList.ToArray());
        }
    }
}
