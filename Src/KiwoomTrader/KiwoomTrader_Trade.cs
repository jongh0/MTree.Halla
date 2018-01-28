using Trader;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLib.Firm.Kiwoom;
using Trader.Account;

namespace KiwoomTrader
{
    public partial class KiwoomTrader_
    {
        private OrderResult CurrOrderResult { get; set; }

        public List<AccountInformation> GetAccountInformations()
        {
            throw new NotImplementedException();
        }

        public List<string> GetAccountList()
        {
            throw new NotImplementedException();
        }

        public long GetDeposit(string accNum, string accPw)
        {
            throw new NotImplementedException();
        }
        
        public bool MakeOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            throw new NotImplementedException();
        }

        private void OrderResultReceived(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            throw new NotImplementedException();
        }

        private void OrderConclusionReceived(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            throw new NotImplementedException();
        }

        private void AccountDepositReceived(AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            throw new NotImplementedException();
        }
    }
}
