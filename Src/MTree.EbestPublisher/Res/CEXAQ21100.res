BEGIN_FUNCTION_MAP
	.Func,������ �ֹ�ü�᳻����ȸ,CEXAQ21100,SERVICE=CEXAQ21100,headtype=B,CREATOR=�̽���,CREDATE=2012/07/02 21:01:29;
	BEGIN_DATA_MAP
	CEXAQ21100InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����Է±���, ChoicInptTpCode, ChoicInptTpCode, char, 1;
		������ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		ü�ᱸ��, PrdtExecTpCode, PrdtExecTpCode, char, 1;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
	end
	CEXAQ21100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����Է±���, ChoicInptTpCode, ChoicInptTpCode, char, 1;
		������ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		ü�ᱸ��, PrdtExecTpCode, PrdtExecTpCode, char, 1;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
	end
	CEXAQ21100OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�ֹ�����, OrdQty, OrdQty, long, 16;
		ü�����, ExecQty, ExecQty, long, 16;
	end
	CEXAQ21100OutBlock3,Out1(*EMPTY*),output,occurs;
	begin
		���¹�ȣ1, AcntNo1, AcntNo1, char, 20;
		�ֹ���, OrdDt, OrdDt, char, 8;
		�ֹ���ȣ, OrdNo, OrdNo, long, 10;
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		�ֹ��ð�, OrdTime, OrdTime, char, 9;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		������ұ��и�, MrcTpNm, MrcTpNm, char, 10;
		�������������Ǳ����ڵ�, ErxPrcCndiTpCode, ErxPrcCndiTpCode, char, 1;
		�����ɼ�ȣ��������, FnoOrdprcPtnNm, FnoOrdprcPtnNm, char, 40;
		�ֹ����ǰ���, OrdCndiPrc, OrdCndiPrc, double, 25.8;
		�ֹ���, OrdPrc, OrdPrc, double, 13.2;
		�ֹ�����, OrdQty, OrdQty, long, 16;
		�ֹ����и�, OrdTpNm, OrdTpNm, char, 10;
		ü�ᰡ, ExecPrc, ExecPrc, double, 13.2;
		ü�����, ExecQty, ExecQty, long, 16;
		��ü�����, UnercQty, UnercQty, long, 16;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
		��Ÿ�ü��, CommdaNm, CommdaNm, char, 40;
	end
	END_DATA_MAP
END_FUNCTION_MAP
