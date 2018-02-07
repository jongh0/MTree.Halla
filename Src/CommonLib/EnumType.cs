using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public enum ProcessTypes
    {
        Unknown,
        CybosStarter,
        DibServer,
        KiwoomStarter,
        Dashboard,
        HistorySaver,
        StrategyManager,
        TestConsumer,
        RealTimeProvider,
        DaishinPublisher,
        DaishinPublisherMaster,
        EbestPublisher,
        KiwoomPublisher,
        KiwoomSessionManager,
        TestPublisher,
        EbestTrader,
        KiwoomTrader,
        VirtualTrader,
        KillAll,
        AutoLauncher,
        DaishinSessionManager,
        DataValidator,
        DataValidatorRegularCheck,
        ResourceMonitor,
        PopupStopper,
        SendLog,
        TestConsole,
    }
}
