using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.Account;

namespace Trader
{
    public static class TraderMappingBootstrapper
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Initialize()
        {
            try
            {
                AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.AddProfile<AccountInfoMappingProfile>();
                    cfg.AddProfile<HoldingStockMappingProfile>();
                    cfg.AddProfile<OrderMappingProfile>();
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
