BEGIN_FUNCTION_MAP
	.Func,�����ɼ� �����ֹ�ü�᳻����ȸ,CFOAQ00600,ENCRYPT,SERVICE=CFOAQ00600,headtype=B,CREATOR=�����,CREDATE=2012/03/12 16:35:20;
	BEGIN_DATA_MAP
	CFOAQ00600InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		�����ɼǺз��ڵ�, FnoClssCode, FnoClssCode, char, 2;
		��ǰ���ڵ�, PrdgrpCode, PrdgrpCode, char, 2;
		ü�ᱸ��, PrdtExecTpCode, PrdtExecTpCode, char, 1;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
	end
	CFOAQ00600OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		�����ɼǺз��ڵ�, FnoClssCode, FnoClssCode, char, 2;
		��ǰ���ڵ�, PrdgrpCode, PrdgrpCode, char, 2;
		ü�ᱸ��, PrdtExecTpCode, PrdtExecTpCode, char, 1;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
		��Ÿ�ü�ڵ�, CommdaCode, CommdaCode, char, 2;
	end
	CFOAQ00600OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�����ֹ�����, FutsOrdQty, FutsOrdQty, long, 16;
		����ü�����, FutsExecQty, FutsExecQty, long, 16;
		�ɼ��ֹ�����, OptOrdQty, OptOrdQty, long, 16;
		�ɼ�ü�����, OptExecQty, OptExecQty, long, 16;
	end
	CFOAQ00600OutBlock3,Out1(*EMPTY*),output,occurs;
	begin
		�ֹ���, OrdDt, OrdDt, char, 8;
		�ֹ���ȣ, OrdNo, OrdNo, long, 10;
		���ֹ���ȣ, OrgOrdNo, OrgOrdNo, long, 10;
		�ֹ��ð�, OrdTime, OrdTime, char, 9;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
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
	end
	END_DATA_MAP
END_FUNCTION_MAP
