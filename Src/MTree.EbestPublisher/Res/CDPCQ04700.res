BEGIN_FUNCTION_MAP
	.Func,���� �ŷ�����,CDPCQ04700,SERVICE=CDPCQ04700,headtype=B,CREATOR=��ȭ��,CREDATE=2012/06/14 10:25:09;
	BEGIN_DATA_MAP
	CDPCQ04700InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ȸ����, QryTp, QryTp, char, 1;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		���۹�ȣ, SrtNo, SrtNo, long, 10;
		��ǰ�����ڵ�, PdptnCode, PdptnCode, char, 2;
		�����з��ڵ�, IsuLgclssCode, IsuLgclssCode, char, 2;
		�����ȣ, IsuNo, IsuNo, char, 12;
	end
	CDPCQ04700OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ȸ����, QryTp, QryTp, char, 1;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		���۹�ȣ, SrtNo, SrtNo, long, 10;
		��ǰ�����ڵ�, PdptnCode, PdptnCode, char, 2;
		�����з��ڵ�, IsuLgclssCode, IsuLgclssCode, char, 2;
		�����ȣ, IsuNo, IsuNo, char, 12;
	end
	CDPCQ04700OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
	end
	CDPCQ04700OutBlock3,Out2(*EMPTY*),output,occurs;
	begin
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�ŷ�����, TrdDt, TrdDt, char, 8;
		�ŷ���ȣ, TrdNo, TrdNo, long, 10;
		�����ڵ��, TpCodeNm, TpCodeNm, char, 50;
		�����ȣ, SmryNo, SmryNo, char, 4;
		�����, SmryNm, SmryNm, char, 40;
		��ұ���, CancTpNm, CancTpNm, char, 20;
		�ŷ�����, TrdQty, TrdQty, long, 16;
		�ŷ���, Trtax, Trtax, long, 16;
		��ȭ����ݾ�, FcurrAdjstAmt, FcurrAdjstAmt, double, 25.4;
		����ݾ�, AdjstAmt, AdjstAmt, long, 16;
		��ü��, OvdSum, OvdSum, long, 16;
		���������ܱݾ�, DpsBfbalAmt, DpsBfbalAmt, long, 16;
		�ŵ��㺸��ȯ��, SellPldgRfundAmt, SellPldgRfundAmt, long, 16;
		��Ź�㺸�������ܱݾ�, DpspdgLoanBfbalAmt, DpspdgLoanBfbalAmt, long, 16;
		�ŷ���ü��, TrdmdaNm, TrdmdaNm, char, 40;
		���ŷ���ȣ, OrgTrdNo, OrgTrdNo, long, 10;
		�����, IsuNm, IsuNm, char, 40;
		�ŷ��ܰ�, TrdUprc, TrdUprc, double, 13.2;
		������, CmsnAmt, CmsnAmt, long, 16;
		��ȭ������ݾ�, FcurrCmsnAmt, FcurrCmsnAmt, double, 15.2;
		��ȯ���̱ݾ�, RfundDiffAmt, RfundDiffAmt, long, 16;
		�������հ�, RepayAmtSum, RepayAmtSum, long, 16;
		�������Ǳ��ܼ���, SecCrbalQty, SecCrbalQty, long, 16;
		�ŵ���ݴ㺸�����ȯ���ڱݾ�, CslLoanRfundIntrstAmt, CslLoanRfundIntrstAmt, long, 16;
		��Ź�㺸������ܱݾ�, DpspdgLoanCrbalAmt, DpspdgLoanCrbalAmt, long, 16;
		ó���ð�, TrxTime, TrxTime, char, 9;
		�ⳳ��ȣ, Inouno, Inouno, long, 10;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�ŷ��ݾ�, TrdAmt, TrdAmt, long, 16;
		��ǥ�ݾ�, ChckAmt, ChckAmt, long, 16;
		�����հ�ݾ�, TaxSumAmt, TaxSumAmt, long, 16;
		��ȭ�����հ�ݾ�, FcurrTaxSumAmt, FcurrTaxSumAmt, double, 26.6;
		�����̿��, IntrstUtlfee, IntrstUtlfee, long, 16;
		���ݾ�, MnyDvdAmt, MnyDvdAmt, long, 16;
		�̼��߻��ݾ�, RcvblOcrAmt, RcvblOcrAmt, long, 16;
		ó��������ȣ, TrxBrnNo, TrxBrnNo, char, 3;
		ó��������, TrxBrnNm, TrxBrnNm, char, 40;
		��Ź�㺸����ݾ�, DpspdgLoanAmt, DpspdgLoanAmt, long, 16;
		��Ź�㺸�����ȯ�ݾ�, DpspdgLoanRfundAmt, DpspdgLoanRfundAmt, long, 16;
		���ذ�, BasePrc, BasePrc, double, 13.2;
		�����ݱ��ܱݾ�, DpsCrbalAmt, DpsCrbalAmt, long, 16;
		��ǥ, BoaAmt, BoaAmt, long, 16;
		��ݰ��ɱݾ�, MnyoutAbleAmt, MnyoutAbleAmt, long, 16;
		�������Ǵ㺸����߻���, BcrLoanOcrAmt, BcrLoanOcrAmt, long, 16;
		�������Ǵ㺸�������ܱ�, BcrLoanBfbalAmt, BcrLoanBfbalAmt, long, 16;
		�Ÿű��ذ�, BnsBasePrc, BnsBasePrc, double, 20.10;
		�������ذ�, TaxchrBasePrc, TaxchrBasePrc, double, 20.10;
		�ŷ��¼�, TrdUnit, TrdUnit, long, 16;
		�ܰ��¼�, BalUnit, BalUnit, long, 16;
		������, EvrTax, EvrTax, long, 16;
		�򰡱ݾ�, EvalAmt, EvalAmt, long, 16;
		�������Ǵ㺸�����ȯ��, BcrLoanRfundAmt, BcrLoanRfundAmt, long, 16;
		�������Ǵ㺸������ܱ�, BcrLoanCrbalAmt, BcrLoanCrbalAmt, long, 16;
		�߰����űݹ߻��Ѿ�, AddMgnOcrTotamt, AddMgnOcrTotamt, long, 16;
		�߰��������űݹ߻��ݾ�, AddMnyMgnOcrAmt, AddMnyMgnOcrAmt, long, 16;
		�߰����űݳ����Ѿ�, AddMgnDfryTotamt, AddMgnDfryTotamt, long, 16;
		�߰��������űݳ��αݾ�, AddMnyMgnDfryAmt, AddMnyMgnDfryAmt, long, 16;
		�Ÿż��ͱݾ�, BnsplAmt, BnsplAmt, long, 16;
		�ҵ漼, Ictax, Ictax, long, 16;
		�ֹμ�, Ihtax, Ihtax, long, 16;
		������, LoanDt, LoanDt, char, 8;
		��ȭ�ڵ�, CrcyCode, CrcyCode, char, 3;
		��ȭ�ݾ�, FcurrAmt, FcurrAmt, double, 24.4;
		��ȭ�ŷ��ݾ�, FcurrTrdAmt, FcurrTrdAmt, double, 24.4;
		��ȭ������, FcurrDps, FcurrDps, double, 21.4;
		��ȭ���������ܱݾ�, FcurrDpsBfbalAmt, FcurrDpsBfbalAmt, double, 21.4;
		�����¸�, OppAcntNm, OppAcntNm, char, 40;
		�����¹�ȣ, OppAcntNo, OppAcntNo, char, 20;
		�����ȯ�ݾ�, LoanRfundAmt, LoanRfundAmt, long, 16;
		�������ڱݾ�, LoanIntrstAmt, LoanIntrstAmt, long, 16;
		�Ƿ��θ�, AskpsnNm, AskpsnNm, char, 40;
		�ֹ�����, OrdDt, OrdDt, char, 8;
		�ŷ�ȯ��, TrdXchrat, TrdXchrat, double, 15.4;
		���������, RdctCmsn, RdctCmsn, double, 21.4;
		��ȭ������, FcurrStmpTx, FcurrStmpTx, double, 21.4;
		��ȭ���ڱ����ŷ���, FcurrElecfnTrtax, FcurrElecfnTrtax, double, 21.4;
		��ȭ���ǰŷ���, FcstckTrtax, FcstckTrtax, double, 21.4;
	end
	CDPCQ04700OutBlock4,Out3(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����հ�ݾ�, PnlSumAmt, PnlSumAmt, long, 16;
		��������, CtrctAsm, CtrctAsm, long, 16;
		�������հ�ݾ�, CmsnAmtSumAmt, CmsnAmtSumAmt, long, 16;
	end
	CDPCQ04700OutBlock5,Out4(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�Աݱݾ�, MnyinAmt, MnyinAmt, long, 16;
		�԰�ݾ�, SecinAmt, SecinAmt, long, 16;
		��ݱݾ�, MnyoutAmt, MnyoutAmt, long, 16;
		���ݾ�, SecoutAmt, SecoutAmt, long, 16;
		���̱ݾ�, DiffAmt, DiffAmt, long, 16;
		���̱ݾ�0, DiffAmt0, DiffAmt0, long, 16;
		�ŵ�����, SellQty, SellQty, long, 16;
		�ŵ��ݾ�, SellAmt, SellAmt, long, 16;
		�ŵ�������, SellCmsn, SellCmsn, long, 16;
		������, EvrTax, EvrTax, long, 19;
		��ȭ�ŵ�����ݾ�, FcurrSellAdjstAmt, FcurrSellAdjstAmt, double, 25.4;
		�ż�����, BuyQty, BuyQty, long, 16;
		�ż��ݾ�, BuyAmt, BuyAmt, long, 16;
		�ż�������, BuyCmsn, BuyCmsn, long, 16;
		ü�Ἴ��, ExecTax, ExecTax, long, 16;
		��ȭ�ż�����ݾ�, FcurrBuyAdjstAmt, FcurrBuyAdjstAmt, double, 25.4;
	end
	END_DATA_MAP
END_FUNCTION_MAP
