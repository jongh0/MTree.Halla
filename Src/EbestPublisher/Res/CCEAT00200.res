BEGIN_FUNCTION_MAP
	.Func,CME �����ֹ�,CCEAT00200,SERVICE=CCEAT00200,headtype=B,CREATOR=��ȣ��,CREDATE=2012/04/10 10:07:08;
	BEGIN_DATA_MAP
	CCEAT00200InBlock1,In(*EMPTY*),input;
	begin
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		�����ɼ�ȣ�������ڵ�, FnoOrdprcPtnCode, FnoOrdprcPtnCode, char, 2;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		��������, MdfyQty, MdfyQty, long, 16;
	end
	CCEAT00200OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�ֹ������ڵ�, OrdMktCode, OrdMktCode, char, 2;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�����ɼ��ֹ������ڵ�, FnoOrdPtnCode, FnoOrdPtnCode, char, 2;
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		�����ɼ�ȣ�������ڵ�, FnoOrdprcPtnCode, FnoOrdprcPtnCode, char, 2;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		��������, MdfyQty, MdfyQty, long, 16;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
		���ǸŸſϷ�ð�, DscusBnsCmpltTime, DscusBnsCmpltTime, char, 9;
		�׷�ID, GrpId, GrpId, char, 20;
		�ֹ��Ϸù�ȣ, OrdSeqno, OrdSeqno, long, 10;
		��Ʈ��������ȣ, PtflNo, PtflNo, long, 10;
		�ٽ��Ϲ�ȣ, BskNo, BskNo, long, 10;
		Ʈ��ġ��ȣ, TrchNo, TrchNo, long, 10;
		�����۹�ȣ, ItemNo, ItemNo, long, 10;
		���������ȣ, MgempNo, MgempNo, char, 9;
		�ݵ�ID, FundId, FundId, char, 12;
		�ݵ���ֹ���ȣ, FundOrgOrdNo, FundOrgOrdNo, long, 10;
		�ݵ��ֹ���ȣ, FundOrdNo, FundOrdNo, long, 10;
	end
	CCEAT00200OutBlock2,Out(*EMPTY*),output;
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
