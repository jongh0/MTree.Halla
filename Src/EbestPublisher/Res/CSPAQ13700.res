BEGIN_FUNCTION_MAP
	.Func,���������ֹ�ü�᳻����ȸ,CSPAQ13700,SERVICE=CSPAQ13700,headtype=B,CREATOR=�̻���,CREDATE=2015/04/13 08:39:53;
	BEGIN_DATA_MAP
	CSPAQ13700InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		�ֹ������ڵ�, OrdMktCode, OrdMktCode, char, 2;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�����ȣ, IsuNo, IsuNo, char, 12;
		ü�Ῡ��, ExecYn, ExecYn, char, 1;
		�ֹ���, OrdDt, OrdDt, char, 8;
		�����ֹ���ȣ2, SrtOrdNo2, SrtOrdNo2, long, 10;
		��������, BkseqTpCode, BkseqTpCode, char, 1;
		�ֹ������ڵ�, OrdPtnCode, OrdPtnCode, char, 2;
	end
	CSPAQ13700OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		�ֹ������ڵ�, OrdMktCode, OrdMktCode, char, 2;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�����ȣ, IsuNo, IsuNo, char, 12;
		ü�Ῡ��, ExecYn, ExecYn, char, 1;
		�ֹ���, OrdDt, OrdDt, char, 8;
		�����ֹ���ȣ2, SrtOrdNo2, SrtOrdNo2, long, 10;
		��������, BkseqTpCode, BkseqTpCode, char, 1;
		�ֹ������ڵ�, OrdPtnCode, OrdPtnCode, char, 2;
	end
	CSPAQ13700OutBlock2,OUT1(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�ŵ�ü��ݾ�, SellExecAmt, SellExecAmt, long, 16;
		�ż�ü��ݾ�, BuyExecAmt, BuyExecAmt, long, 16;
		�ŵ�ü�����, SellExecQty, SellExecQty, long, 16;
		�ż�ü�����, BuyExecQty, BuyExecQty, long, 16;
		�ŵ��ֹ�����, SellOrdQty, SellOrdQty, long, 16;
		�ż��ֹ�����, BuyOrdQty, BuyOrdQty, long, 16;
	end
	CSPAQ13700OutBlock3,OUT(*EMPTY*),output,occurs;
	begin
		�ֹ���, OrdDt, OrdDt, char, 8;
		����������ȣ, MgmtBrnNo, MgmtBrnNo, char, 3;
		�ֹ������ڵ�, OrdMktCode, OrdMktCode, char, 2;
		�ֹ���ȣ, OrdNo, OrdNo, long, 10;
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
		�ֹ������ڵ�, OrdPtnCode, OrdPtnCode, char, 2;
		�ֹ�������, OrdPtnNm, OrdPtnNm, char, 40;
		�ֹ�ó�������ڵ�, OrdTrxPtnCode, OrdTrxPtnCode, long, 9;
		�ֹ�ó��������, OrdTrxPtnNm, OrdTrxPtnNm, char, 50;
		������ұ���, MrcTpCode, MrcTpCode, char, 1;
		������ұ��и�, MrcTpNm, MrcTpNm, char, 10;
		������Ҽ���, MrcQty, MrcQty, long, 16;
		������Ұ��ɼ���, MrcAbleQty, MrcAbleQty, long, 16;
		�ֹ�����, OrdQty, OrdQty, long, 16;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		ü�����, ExecQty, ExecQty, long, 16;
		ü�ᰡ, ExecPrc, ExecPrc, double, 15.2;
		ü��ó���ð�, ExecTrxTime, ExecTrxTime, char, 9;
		����ü��ð�, LastExecTime, LastExecTime, char, 9;
		ȣ�������ڵ�, OrdprcPtnCode, OrdprcPtnCode, char, 2;
		ȣ��������, OrdprcPtnNm, OrdprcPtnNm, char, 40;
		�ֹ����Ǳ���, OrdCndiTpCode, OrdCndiTpCode, char, 1;
		��üü�����, AllExecQty, AllExecQty, long, 16;
		��Ÿ�ü�ڵ�, RegCommdaCode, RegCommdaCode, char, 2;
		��Ÿ�ü��, CommdaNm, CommdaNm, char, 40;
		ȸ����ȣ, MbrNo, MbrNo, char, 3;
		�����ֹ�����, RsvOrdYn, RsvOrdYn, char, 1;
		������, LoanDt, LoanDt, char, 8;
		�ֹ��ð�, OrdTime, OrdTime, char, 9;
		������ù�ȣ, OpDrtnNo, OpDrtnNo, char, 12;
		�ֹ���ID, OdrrId, OdrrId, char, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
