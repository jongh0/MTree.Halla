BEGIN_FUNCTION_MAP
	.Func,�����ɼ� CME �ŸŰŷ���Ȳ,CCEAQ01100,SERVICE=CCEAQ01100,headtype=B,CREATOR=������,CREDATE=2013/07/19 11:07:15;
	BEGIN_DATA_MAP
	CCEAQ01100InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
	end
	CCEAQ01100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
	end
	CCEAQ01100OutBlock2,Sum(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�Ÿż��ͱݾ�, BnsplAmt, BnsplAmt, long, 16;
		�����ɼǾ����ݾ�, FnoCtrctAmt, FnoCtrctAmt, long, 16;
		�������հ�ݾ�, CmsnAmtSumAmt, CmsnAmtSumAmt, long, 16;
	end
	CCEAQ01100OutBlock3,Out(*EMPTY*),output,occurs;
	begin
		�Ÿ���, BnsDt, BnsDt, char, 8;
		�ֹ���ȣ, OrdNo, OrdNo, long, 10;
		������ȣ, CtrctNo, CtrctNo, long, 10;
		ü���ȣ, ExecNo, ExecNo, long, 10;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
		���ʾ�������ü�ᰡ, BgnCtrctIdxExecPrc, BgnCtrctIdxExecPrc, double, 13.2;
		���ʱݾ�, BgnAmt, BgnAmt, long, 16;
		��������, CtrctQty, CtrctQty, long, 16;
		ü�ᰡ, ExecPrc, ExecPrc, double, 13.2;
		�����ݾ�, CtrctAmt, CtrctAmt, long, 16;
		������ݾ�, CmsnAmt, CmsnAmt, long, 16;
		�Ÿż��ͱݾ�, BnsplAmt, BnsplAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
