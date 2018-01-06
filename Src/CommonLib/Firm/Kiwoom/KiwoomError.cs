using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Kiwoom
{
    public class KiwoomError
    {
        enum KOAErrorCode
        {
            OP_ERR_NONE = 0,                // 정상처리
            OP_ERR_LOGIN = -100,            // 사용자정보교환에 실패하였습니다. 잠시후 다시 시작하여 주십시오.
            OP_ERR_CONNECT = -101,          // 서버 접속 실패
            OP_ERR_VERSION = -102,          // 버전처리가 실패하였습니다.
            OP_ERR_SISE_OVERFLOW = -200,    // 시세조회 과부하
            OP_ERR_RQ_STRUCT_FAIL = -201,   // REQUEST_INPUT_st Failed
            OP_ERR_RQ_STRING_FAIL = -202,   // 요청 전문 작성 실패
            OP_ERR_ORD_WRONG_INPUT = -300,  // 주문 입력값 오류
            OP_ERR_ORD_WRONG_ACCNO = -301,  // 계좌비밀번호를 입력하십시오.
            OP_ERR_OTHER_ACC_USE = -302,    // 타인계좌는 사용할 수 없습니다.
            OP_ERR_MIS_2BILL_EXC = -303,    // 주문가격이 20억원을 초과합니다.
            OP_ERR_MIS_5BILL_EXC = -304,    // 주문가격은 50억원을 초과할 수 없습니다.
            OP_ERR_MIS_1PER_EXC = -305,     // 주문수량이 총발행주수의 1%를 초과합니다.
            OP_ERR_MID_3PER_EXC = -306,     // 주문수량은 총발행주수의 3%를 초과할 수 없습니다.
        }

        private static Dictionary<int, string> ErrorMessageList { get; set; } = new Dictionary<int, string>();

        static KiwoomError()
        {
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_NONE, "정상처리");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_LOGIN, "사용자정보교환에 실패하였습니다. 잠시후 다시 시작하여 주십시오.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_CONNECT, "서버 접속 실패");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_VERSION, "버전처리가 실패하였습니다.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_SISE_OVERFLOW, "시세조회 과부하");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_RQ_STRUCT_FAIL, "REQUEST_INPUT_st Failed");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_RQ_STRING_FAIL, "요청 전문 작성 실패");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_ORD_WRONG_INPUT, "주문 입력값 오류");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_ORD_WRONG_ACCNO, "계좌비밀번호를 입력하십시오.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_OTHER_ACC_USE, "타인계좌는 사용할 수 없습니다.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_MIS_2BILL_EXC, "주문가격이 20억원을 초과합니다.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_MIS_5BILL_EXC, "주문가격은 50억원을 초과할 수 없습니다.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_MIS_1PER_EXC, "주문수량이 총발행주수의 1%를 초과합니다.");
            ErrorMessageList.Add((int)KOAErrorCode.OP_ERR_MID_3PER_EXC, "주문수량은 총발행주수의 3%를 초과할 수 없습니다.");
        }

        public static string GetErrorMessage(int errCode)
        {
            var errMsg = "알려지지 않은 오류입니다.";

            if (ErrorMessageList.ContainsKey(errCode) == true)
                errMsg = ErrorMessageList[errCode];

            return $"errCode: {errCode}, errMsg: {errMsg}";
        }
    }
}
