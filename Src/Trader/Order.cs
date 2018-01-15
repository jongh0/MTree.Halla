using CommonLib.Firm.Ebest;
using CommonLib.Firm.Ebest.Block;
using CommonLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class Order
    {
        public string AccountNumber { get; set; }

        public string AccountPassword { get; set; }

        public string Code { get; set; }

        public long Quantity { get; set; }

        public long Price { get; set; }

        public PriceTypes PriceType { get; set; }

        public OrderTypes OrderType { get; set; }

        public string OriginOrderNumber { get; set; }

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this);
        }

        public void CopyTo(CSPAT00600InBlock1 block)
        {
            if (block == null) return;

            block.AcntNo = AccountNumber;
            block.InptPwd = AccountPassword;
            block.IsuNo = Code;
            block.OrdQty = Quantity;
            if (OrderType == OrderTypes.BuyNew)
                block.BnsTpCode = "2";
            else
                block.BnsTpCode = "1";

            if (PriceType == PriceTypes.LimitPrice)
            {
                block.OrdPrc = Price;
                block.OrdprcPtnCode = "00";
            }
            else
            {
                block.OrdPrc = 0;
                block.OrdprcPtnCode = "03";
            }

            block.MgntrnCode = "000";
            block.LoanDt = "";
            block.OrdCndiTpCode = "0";
        }

        public void CopyTo(CSPAT00700InBlock1 block)
        {
            if (block == null) return;

            block.OrgOrdNo = long.TryParse(OriginOrderNumber, out long oon) ? oon : 0;
            block.AcntNo = AccountNumber;
            block.InptPwd = AccountPassword;
            block.IsuNo = Code;
            block.OrdQty = Quantity;

            if (PriceType == PriceTypes.LimitPrice)
            {
                block.OrdPrc = Price;
                block.OrdprcPtnCode = "00";
            }
            else
            {
                block.OrdPrc = 0;
                block.OrdprcPtnCode = "03";
            }

            block.OrdCndiTpCode = "0";
        }

        public void CopyTo(CSPAT00800InBlock1 block)
        {
            if (block == null) return;

            block.OrgOrdNo = long.TryParse(OriginOrderNumber, out long oon) ? oon : 0;
            block.AcntNo = AccountNumber;
            block.InptPwd = AccountPassword;
            block.IsuNo = Code;
            block.OrdQty = Quantity;
        }
    }
}
