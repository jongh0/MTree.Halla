BEGIN_FUNCTION_MAP
	.Func,������ �ż�/�ŵ��ֹ�,CEXAT11100,SERVICE=CEXAT11100,headtype=B,CREATOR=�̽���,CREDATE=2012/06/27 20:48:05;
	BEGIN_DATA_MAP
	CEXAT11100InBlock1,In(*EMPTY*),input;
	begin
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�������������Ǳ����ڵ�, ErxPrcCndiTpCode, ErxPrcCndiTpCode, char, 1;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		�ֹ�����, OrdQty, OrdQty, long, 16;
	end
	CEXAT11100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�������������Ǳ����ڵ�, ErxPrcCndiTpCode, ErxPrcCndiTpCode, char, 1;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		�ֹ�����, OrdQty, OrdQty, long, 16;
		�ֹ����ǰ���, OrdCndiPrc, OrdCndiPrc, double, 25.8;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
	end
	CEXAT11100OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�ֹ���ȣ, OrdNo, OrdNo, long, 10;
		������, BrnNm, BrnNm, char, 40;
		���¸�, AcntNm, AcntNm, char, 40;
		�����, IsuNm, IsuNm, char, 50;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		�ֹ����ű�, OrdMgn, OrdMgn, long, 16;
		�����ֹ����ű�, MnyOrdMgn, MnyOrdMgn, long, 16;
		�ֹ����ɼ���, OrdAbleQty, OrdAbleQty, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
