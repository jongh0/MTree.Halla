BEGIN_FUNCTION_MAP
	.Func,������ �����ֹ�,CEXAT11200,SERVICE=CEXAT11200,headtype=B,CREATOR=�̽���,CREDATE=2012/06/27 21:27:10;
	BEGIN_DATA_MAP
	CEXAT11200InBlock1,In(*EMPTY*),input;
	begin
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
	end
	CEXAT11200OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�Ÿű����ڵ�, BnsTpCode, BnsTpCode, char, 1;
		�������������Ǳ����ڵ�, ErxPrcCndiTpCode, ErxPrcCndiTpCode, char, 1;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		��������, MdfyQty, MdfyQty, long, 16;
		�ֹ����ǰ���, OrdCndiPrc, OrdCndiPrc, double, 25.8;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
	end
	CEXAT11200OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�ֹ���ȣ, OrdNo, OrdNo, long, 10;
		������, BrnNm, BrnNm, char, 40;
		���¸�, AcntNm, AcntNm, char, 40;
		�����, IsuNm, IsuNm, char, 50;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		�ֹ����űݾ�, OrdMgn, OrdMgn, long, 16;
		�����ֹ����űݾ�, MnyOrdMgn, MnyOrdMgn, long, 16;
		�ֹ����ɼ���, OrdAbleQty, OrdAbleQty, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
