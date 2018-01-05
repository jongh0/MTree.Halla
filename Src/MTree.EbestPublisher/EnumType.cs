using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.EbestPublisher
{
    public enum WarningTypes1
    {
        AdministrativeIssue = 1,    // 관리
        UnfairAnnouncement = 2,     // 불성실공시
        InvestAttention = 3,        // 투자유의
        CallingAttention = 4,       // 투자환기
    }

    public enum WarningTypes2
    {
        InvestWarning = 1,          // 경고
        TradingHalt = 2,            // 매매정지
        CleaningTrade = 3,          // 정리매매
        InvestCaution = 4,          // 주의
        InvestRisk = 5,             // 위험
        InvestRiskNoticed = 6,      // 위험예고
        Overheated = 7,             // 단기과열
        OverheatNoticed = 8,        // 단기과열지정예고
    }
}
