BEGIN_FUNCTION_MAP
	.Func,�����ɼ� �����ֹ�,CFOAT00100,SERVICE=CFOAT00100,ENCRYPT,SIGNATURE,headtype=B,CREATOR=������,CREDATE=2012/03/12 13:31:10;
	BEGIN_DATA_MAP
	CFOAT00100InBlock1,In(*EMPTY*),input;
	begin
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�����ɼ�ȣ�������ڵ�, FnoOrdprcPtnCode, FnoOrdprcPtnCode, char, 2;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		�ֹ�����, OrdQty, OrdQty, long, 16;
	end
	CFOAT00100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�ֹ������ڵ�, OrdMktCode, OrdMktCode, char, 2;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�����ɼ��ֹ������ڵ�, FnoOrdPtnCode, FnoOrdPtnCode, char, 2;
		�����ɼ�ȣ�������ڵ�, FnoOrdprcPtnCode, FnoOrdprcPtnCode, char, 2;
		�����ɼǰŷ������ڵ�, FnoTrdPtnCode, FnoTrdPtnCode, char, 2;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		�ֹ�����, OrdQty, OrdQty, long, 16;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
		���ǸŸſϷ�ð�, DscusBnsCmpltTime, DscusBnsCmpltTime, char, 9;
		�׷�ID, GrpId, GrpId, char, 20;
		�ֹ��Ϸù�ȣ, OrdSeqno, OrdSeqno, long, 10;
		��Ʈ��������ȣ, PtflNo, PtflNo, long, 10;
		�ٽ��Ϲ�ȣ, BskNo, BskNo, long, 10;
		Ʈ��ġ��ȣ, TrchNo, TrchNo, long, 10;
		�׸��ȣ, ItemNo, ItemNo, long, 16;
		������ù�ȣ, OpDrtnNo, OpDrtnNo, char, 12;
		���������ȣ, MgempNo, MgempNo, char, 9;
		�ݵ�ID, FundId, FundId, char, 12;
		�ݵ��ֹ���ȣ, FundOrdNo, FundOrdNo, long, 10;
	end
	CFOAT00100OutBlock2,Out(*EMPTY*),output;
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
CFOAT00100