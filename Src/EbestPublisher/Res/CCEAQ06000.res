BEGIN_FUNCTION_MAP
	.Func,�����ɼ� CME �ֹ�ü�᳻����ȸ,CCEAQ06000,SERVICE=CCEAQ06000,headtype=B,CREATOR=��ȿ��,CREDATE=2012/04/17 17:48:18;
	BEGIN_DATA_MAP
	CCEAQ06000InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����Է±���, ChoicInptTpCode, ChoicInptTpCode, char, 1;
		������ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		�����ɼǺз��ڵ�, FnoClssCode, FnoClssCode, char, 2;
		��ǰ���ڵ�, PrdgrpCode, PrdgrpCode, char, 2;
		ü�ᱸ��, PrdtExecTpCode, PrdtExecTpCode, char, 1;
		�����ɼǰŷ������ڵ�, FnoTrdPtnCode, FnoTrdPtnCode, char, 2;
		�����ֹ���ȣ2, SrtOrdNo2, SrtOrdNo2, long, 10;
		�����ȣ, EndNo, EndNo, long, 10;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
	end
	CCEAQ06000OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����Է±���, ChoicInptTpCode, ChoicInptTpCode, char, 1;
		������ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		�����ɼǺз��ڵ�, FnoClssCode, FnoClssCode, char, 2;
		��ǰ���ڵ�, PrdgrpCode, PrdgrpCode, char, 2;
		ü�ᱸ��, PrdtExecTpCode, PrdtExecTpCode, char, 1;
		�����ɼǰŷ������ڵ�, FnoTrdPtnCode, FnoTrdPtnCode, char, 2;
		�����ֹ���ȣ2, SrtOrdNo2, SrtOrdNo2, long, 10;
		�����ȣ, EndNo, EndNo, long, 10;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
	end
	CCEAQ06000OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�����ֹ�����, FutsOrdQty, FutsOrdQty, long, 16;
		����ü�����, FutsExecQty, FutsExecQty, long, 16;
	end
	CCEAQ06000OutBlock3,Out1(*EMPTY*),output,occurs;
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
		�����ɼ�ȣ�������ڵ�, FnoOrdprcPtnCode, FnoOrdprcPtnCode, char, 2;
		�����ɼ�ȣ��������, FnoOrdprcPtnNm, FnoOrdprcPtnNm, char, 40;
		�ֹ���, OrdPrc, OrdPrc, double, 13.2;
		�ֹ�����, OrdQty, OrdQty, long, 16;
		�ֹ����и�, OrdTpNm, OrdTpNm, char, 10;
		ü�ᱸ�и�, ExecTpNm, ExecTpNm, char, 10;
		ü�ᰡ, ExecPrc, ExecPrc, double, 13.2;
		ü�����, ExecQty, ExecQty, long, 16;
		�����ð�, CtrctTime, CtrctTime, char, 9;
		������ȣ, CtrctNo, CtrctNo, long, 10;
		ü���ȣ, ExecNo, ExecNo, long, 10;
		�Ÿż��ͱݾ�, BnsplAmt, BnsplAmt, long, 16;
		��ü�����, UnercQty, UnercQty, long, 16;
		�����ID, UserId, UserId, char, 16;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
		��Ÿ�ü�ڵ��, CommdaCodeNm, CommdaCodeNm, char, 40;
		IP�ּ�, IpAddr, IpAddr, char, 16;
		�ŷ���������, TrdPtnTpNm, TrdPtnTpNm, char, 20;
		�׷�ID, GrpId, GrpId, char, 20;
	end
	END_DATA_MAP
END_FUNCTION_MAP
