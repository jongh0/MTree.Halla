BEGIN_FUNCTION_MAP
	.Func,BEP�ܰ���ȸ,CSPAQ12300,SERVICE=CSPAQ12300,headtype=B,CREATOR=�̻���,CREDATE=2015/04/13 08:43:20;
	BEGIN_DATA_MAP
	CSPAQ12300InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�ܰ��������, BalCreTp, BalCreTp, char, 1;
		���������뱸��, CmsnAppTpCode, CmsnAppTpCode, char, 1;
		D2�ܰ������ȸ����, D2balBaseQryTp, D2balBaseQryTp, char, 1;
		�ܰ�����, UprcTpCode, UprcTpCode, char, 1;
	end
	CSPAQ12300OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�ܰ��������, BalCreTp, BalCreTp, char, 1;
		���������뱸��, CmsnAppTpCode, CmsnAppTpCode, char, 1;
		D2�ܰ������ȸ����, D2balBaseQryTp, D2balBaseQryTp, char, 1;
		�ܰ�����, UprcTpCode, UprcTpCode, char, 1;
	end
	CSPAQ12300OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		������, BrnNm, BrnNm, char, 40;
		���¸�, AcntNm, AcntNm, char, 40;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		��ݰ��ɱݾ�, MnyoutAbleAmt, MnyoutAbleAmt, long, 16;
		�ŷ��ұݾ�, SeOrdAbleAmt, SeOrdAbleAmt, long, 16;
		�ڽ��ڱݾ�, KdqOrdAbleAmt, KdqOrdAbleAmt, long, 16;
		HTS�ֹ����ɱݾ�, HtsOrdAbleAmt, HtsOrdAbleAmt, long, 16;
		���űݷ�100�ۼ�Ʈ�ֹ����ɱݾ�, MgnRat100pctOrdAbleAmt, MgnRat100pctOrdAbleAmt, long, 16;
		�ܰ��򰡱ݾ�, BalEvalAmt, BalEvalAmt, long, 16;
		���Աݾ�, PchsAmt, PchsAmt, long, 16;
		�̼��ݾ�, RcvblAmt, RcvblAmt, long, 16;
		������, PnlRat, PnlRat, double, 18.6;
		���ڿ���, InvstOrgAmt, InvstOrgAmt, long, 20;
		���ڼ��ͱݾ�, InvstPlAmt, InvstPlAmt, long, 16;
		�ſ�㺸�ֹ��ݾ�, CrdtPldgOrdAmt, CrdtPldgOrdAmt, long, 16;
		������, Dps, Dps, long, 16;
		D1������, D1Dps, D1Dps, long, 16;
		D2������, D2Dps, D2Dps, long, 16;
		�ֹ���, OrdDt, OrdDt, char, 8;
		�������űݾ�, MnyMgn, MnyMgn, long, 16;
		������űݾ�, SubstMgn, SubstMgn, long, 16;
		���ݾ�, SubstAmt, SubstAmt, long, 16;
		���ϸż�ü��ݾ�, PrdayBuyExecAmt, PrdayBuyExecAmt, long, 16;
		���ϸŵ�ü��ݾ�, PrdaySellExecAmt, PrdaySellExecAmt, long, 16;
		���ϸż�ü��ݾ�, CrdayBuyExecAmt, CrdayBuyExecAmt, long, 16;
		���ϸŵ�ü��ݾ�, CrdaySellExecAmt, CrdaySellExecAmt, long, 16;
		�򰡼����հ�, EvalPnlSum, EvalPnlSum, long, 15;
		��Ź�ڻ��Ѿ�, DpsastTotamt, DpsastTotamt, long, 16;
		�����, Evrprc, Evrprc, long, 19;
		����ݾ�, RuseAmt, RuseAmt, long, 16;
		��Ÿ�뿩�ݾ�, EtclndAmt, EtclndAmt, long, 16;
		������ݾ�, PrcAdjstAmt, PrcAdjstAmt, long, 16;
		D1������, D1CmsnAmt, D1CmsnAmt, long, 16;
		D2������, D2CmsnAmt, D2CmsnAmt, long, 16;
		D1������, D1EvrTax, D1EvrTax, long, 16;
		D2������, D2EvrTax, D2EvrTax, long, 16;
		D1���������ݾ�, D1SettPrergAmt, D1SettPrergAmt, long, 16;
		D2���������ݾ�, D2SettPrergAmt, D2SettPrergAmt, long, 16;
		����KSE�������ű�, PrdayKseMnyMgn, PrdayKseMnyMgn, long, 16;
		����KSE������ű�, PrdayKseSubstMgn, PrdayKseSubstMgn, long, 16;
		����KSE�ſ��������ű�, PrdayKseCrdtMnyMgn, PrdayKseCrdtMnyMgn, long, 16;
		����KSE�ſ������ű�, PrdayKseCrdtSubstMgn, PrdayKseCrdtSubstMgn, long, 16;
		����KSE�������ű�, CrdayKseMnyMgn, CrdayKseMnyMgn, long, 16;
		����KSE������ű�, CrdayKseSubstMgn, CrdayKseSubstMgn, long, 16;
		����KSE�ſ��������ű�, CrdayKseCrdtMnyMgn, CrdayKseCrdtMnyMgn, long, 16;
		����KSE�ſ������ű�, CrdayKseCrdtSubstMgn, CrdayKseCrdtSubstMgn, long, 16;
		�����ڽ����������ű�, PrdayKdqMnyMgn, PrdayKdqMnyMgn, long, 16;
		�����ڽ��ڴ�����ű�, PrdayKdqSubstMgn, PrdayKdqSubstMgn, long, 16;
		�����ڽ��ڽſ��������ű�, PrdayKdqCrdtMnyMgn, PrdayKdqCrdtMnyMgn, long, 16;
		�����ڽ��ڽſ������ű�, PrdayKdqCrdtSubstMgn, PrdayKdqCrdtSubstMgn, long, 16;
		�����ڽ����������ű�, CrdayKdqMnyMgn, CrdayKdqMnyMgn, long, 16;
		�����ڽ��ڴ�����ű�, CrdayKdqSubstMgn, CrdayKdqSubstMgn, long, 16;
		�����ڽ��ڽſ��������ű�, CrdayKdqCrdtMnyMgn, CrdayKdqCrdtMnyMgn, long, 16;
		�����ڽ��ڽſ������ű�, CrdayKdqCrdtSubstMgn, CrdayKdqCrdtSubstMgn, long, 16;
		�������������������ű�, PrdayFrbrdMnyMgn, PrdayFrbrdMnyMgn, long, 16;
		�����������������ű�, PrdayFrbrdSubstMgn, PrdayFrbrdSubstMgn, long, 16;
		�������������������ű�, CrdayFrbrdMnyMgn, CrdayFrbrdMnyMgn, long, 16;
		�����������������ű�, CrdayFrbrdSubstMgn, CrdayFrbrdSubstMgn, long, 16;
		��������������ű�, PrdayCrbmkMnyMgn, PrdayCrbmkMnyMgn, long, 16;
		������ܴ�����ű�, PrdayCrbmkSubstMgn, PrdayCrbmkSubstMgn, long, 16;
		��������������ű�, CrdayCrbmkMnyMgn, CrdayCrbmkMnyMgn, long, 16;
		������ܴ�����ű�, CrdayCrbmkSubstMgn, CrdayCrbmkSubstMgn, long, 16;
		��Ź�㺸����, DpspdgQty, DpspdgQty, long, 16;
		�ż������(D+2), BuyAdjstAmtD2, BuyAdjstAmtD2, long, 16;
		�ŵ������(D+2), SellAdjstAmtD2, SellAdjstAmtD2, long, 16;
		�����ҿ��(D+1), RepayRqrdAmtD1, RepayRqrdAmtD1, long, 16;
		�����ҿ��(D+2), RepayRqrdAmtD2, RepayRqrdAmtD2, long, 16;
		����ݾ�, LoanAmt, LoanAmt, long, 16;
	end
	CSPAQ12300OutBlock3,ST_OUT(*EMPTY*),output,occurs;
	begin
		�����ȣ, IsuNo, IsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		���������ܰ������ڵ�, SecBalPtnCode, SecBalPtnCode, char, 2;
		���������ܰ�������, SecBalPtnNm, SecBalPtnNm, char, 40;
		�ܰ����, BalQty, BalQty, long, 16;
		�Ÿű����ܰ����, BnsBaseBalQty, BnsBaseBalQty, long, 16;
		���ϸż�ü�����, CrdayBuyExecQty, CrdayBuyExecQty, long, 16;
		���ϸŵ�ü�����, CrdaySellExecQty, CrdaySellExecQty, long, 16;
		�ŵ���, SellPrc, SellPrc, double, 21.4;
		�ż���, BuyPrc, BuyPrc, double, 21.4;
		�ŵ����ͱݾ�, SellPnlAmt, SellPnlAmt, long, 16;
		������, PnlRat, PnlRat, double, 18.6;
		���簡, NowPrc, NowPrc, double, 15.2;
		�ſ�ݾ�, CrdtAmt, CrdtAmt, long, 16;
		������, DueDt, DueDt, char, 8;
		���ϸŵ�ü�ᰡ, PrdaySellExecPrc, PrdaySellExecPrc, double, 13.2;
		���ϸŵ�����, PrdaySellQty, PrdaySellQty, long, 16;
		���ϸż�ü�ᰡ, PrdayBuyExecPrc, PrdayBuyExecPrc, double, 13.2;
		���ϸż�����, PrdayBuyQty, PrdayBuyQty, long, 16;
		������, LoanDt, LoanDt, char, 8;
		��մܰ�, AvrUprc, AvrUprc, double, 13.2;
		�ŵ����ɼ���, SellAbleQty, SellAbleQty, long, 16;
		�ŵ��ֹ�����, SellOrdQty, SellOrdQty, long, 16;
		���ϸż�ü��ݾ�, CrdayBuyExecAmt, CrdayBuyExecAmt, long, 16;
		���ϸŵ�ü��ݾ�, CrdaySellExecAmt, CrdaySellExecAmt, long, 16;
		���ϸż�ü��ݾ�, PrdayBuyExecAmt, PrdayBuyExecAmt, long, 16;
		���ϸŵ�ü��ݾ�, PrdaySellExecAmt, PrdaySellExecAmt, long, 16;
		�ܰ��򰡱ݾ�, BalEvalAmt, BalEvalAmt, long, 16;
		�򰡼���, EvalPnl, EvalPnl, long, 16;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
		�ŵ���ü�����, SellUnercQty, SellUnercQty, long, 16;
		�ŵ��̰�������, SellUnsttQty, SellUnsttQty, long, 16;
		�ż���ü�����, BuyUnercQty, BuyUnercQty, long, 16;
		�ż��̰�������, BuyUnsttQty, BuyUnsttQty, long, 16;
		�̰�������, UnsttQty, UnsttQty, long, 16;
		��ü�����, UnercQty, UnercQty, long, 16;
		��������, PrdayCprc, PrdayCprc, double, 15.2;
		���Աݾ�, PchsAmt, PchsAmt, long, 16;
		��Ͻ����ڵ�, RegMktCode, RegMktCode, char, 2;
		����󼼺з��ڵ�, LoanDtlClssCode, LoanDtlClssCode, char, 2;
		��Ź�㺸�������, DpspdgLoanQty, DpspdgLoanQty, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
