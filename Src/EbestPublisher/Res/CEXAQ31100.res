BEGIN_FUNCTION_MAP
	.Func,������ �߰����ܰ�� ����Ȳ,CEXAQ31100,SERVICE=CEXAQ31100,headtype=B,CREATOR=�̽���,CREDATE=2012/12/22 14:39:24;
	BEGIN_DATA_MAP
	CEXAQ31100InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		�����ڵ�, IsuCode, IsuCode, char, 12;
		�ܰ��򰡱���, BalEvalTp, BalEvalTp, char, 1;
		���������򰡱���, FutsPrcEvalTp, FutsPrcEvalTp, char, 1;
	end
	CEXAQ31100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		�����ڵ�, IsuCode, IsuCode, char, 12;
		�ܰ��򰡱���, BalEvalTp, BalEvalTp, char, 1;
		���������򰡱���, FutsPrcEvalTp, FutsPrcEvalTp, char, 1;
	end
	CEXAQ31100OutBlock2,Out1(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		���¸�, AcntNm, AcntNm, char, 40;
		�Ÿż��ͱݾ�, BnsplAmt, BnsplAmt, long, 16;
		��������, AdjstDfamt, AdjstDfamt, long, 16;
		���򰡱ݾ�, TotEvalAmt, TotEvalAmt, long, 16;
		�Ѽ��ͱݾ�, TotPnlAmt, TotPnlAmt, long, 16;
	end
	CEXAQ31100OutBlock3,Out2(*EMPTY*),output,occurs;
	begin
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
		�̰�������, UnsttQty, UnsttQty, long, 16;
		û�갡�ɼ���, LqdtAbleQty, LqdtAbleQty, long, 16;
		��հ�, FnoAvrPrc, FnoAvrPrc, double, 19.8;
		���ذ�, BasePrc, BasePrc, double, 30.10;
		���簡, NowPrc, NowPrc, double, 13.2;
		���, CmpPrc, CmpPrc, double, 13.2;
		�򰡱ݾ�, EvalAmt, EvalAmt, long, 16;
		�򰡼���, EvalPnl, EvalPnl, long, 16;
		���ͷ�, PnlRat, PnlRat, double, 12.6;
		�̰����ݾ�, UnsttAmt, UnsttAmt, long, 16;
		�Ÿż��ͱݾ�, BnsplAmt, BnsplAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
