using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.KiwoomTrader
{
    public partial class KiwoomTrader
    {
        public List<string> GetAccountList()
        {
            List<string> accounts = new List<string>();
            if (WaitLogin() == true)
            {
                foreach (string acc in kiwoomObj.GetLoginInfo("ACCNO").Split(';'))
                {
                    if (acc != string.Empty)
                    {
                        accounts.Add(acc);
                    }
                }
            }
            return accounts;
        }

        public int GetDeposit(string accountCode)
        {
            throw new NotImplementedException();
        }

        public OrderResult MakeOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public List<HoldingStock> GetHoldingList(string accountCode)
        {
            throw new NotImplementedException();
        }
    }
}
