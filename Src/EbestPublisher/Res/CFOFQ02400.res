BEGIN_FUNCTION_MAP
	.Func,���� �̰��� ������Ȳ(��հ�),CFOFQ02400,SERVICE=CFOFQ02400,headtype=B,CREATOR=������,CREDATE=2012/03/16 14:07:10;
	BEGIN_DATA_MAP
	CFOFQ02400InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��Ͻ����ڵ�, RegMktCode, RegMktCode, char, 2;
		�ż�����, BuyDt, BuyDt, char, 8;
	end
	CFOFQ02400OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��Ͻ����ڵ�, RegMktCode, RegMktCode, char, 2;
		�ż�����, BuyDt, BuyDt, char, 8;
	end
	CFOFQ02400OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		������������, FutsCtrctQty, FutsCtrctQty, long, 16;
		�ɼǾ�������, OptCtrctQty, OptCtrctQty, long, 16;
		��������, CtrctQty, CtrctQty, long, 16;
		���������ݾ�, FutsCtrctAmt, FutsCtrctAmt, long, 16;
		�����ż������ݾ�, FutsBuyctrAmt, FutsBuyctrAmt, long, 16;
		�����ŵ������ݾ�, FutsSlctrAmt, FutsSlctrAmt, long, 16;
		�ݿɼǾ����ݾ�, CalloptCtrctAmt, CalloptCtrctAmt, long, 16;
		�ݸż��ݾ�, CallBuyAmt, CallBuyAmt, long, 16;
		�ݸŵ��ݾ�, CallSellAmt, CallSellAmt, long, 16;
		ǲ�ɼǾ����ݾ�, PutoptCtrctAmt, PutoptCtrctAmt, long, 16;
		ǲ�ż��ݾ�, PutBuyAmt, PutBuyAmt, long, 16;
		ǲ�ŵ��ݾ�, PutSellAmt, PutSellAmt, long, 16;
		��ü�����ݾ�, AllCtrctAmt, AllCtrctAmt, long, 16;
		�ż���������ݾ�, BuyctrAsmAmt, BuyctrAsmAmt, long, 16;
		�ŵ���������ݾ�, SlctrAsmAmt, SlctrAsmAmt, long, 16;
		���������հ�, FutsPnlSum, FutsPnlSum, long, 16;
		�ɼǼ����հ�, OptPnlSum, OptPnlSum, long, 16;
		��ü�����հ�, AllPnlSum, AllPnlSum, long, 16;
	end
	CFOFQ02400OutBlock3,Out2(*EMPTY*),output,occurs;
	begin
		�����ɼ�ǰ�񱸺�, FnoClssCode, FnoClssCode, char, 1;
		�����ŵ�����, FutsSellQty, FutsSellQty, long, 16;
		�����ŵ�����, FutsSellPnl, FutsSellPnl, long, 16;
		�����ż�����, FutsBuyQty, FutsBuyQty, long, 16;
		�����ż�����, FutsBuyPnl, FutsBuyPnl, long, 16;
		�ݸŵ�����, CallSellQty, CallSellQty, long, 16;
		�ݸŵ�����, CallSellPnl, CallSellPnl, long, 16;
		�ݸż�����, CallBuyQty, CallBuyQty, long, 16;
		�ݸż�����, CallBuyPnl, CallBuyPnl, long, 16;
		ǲ�ŵ�����, PutSellQty, PutSellQty, long, 16;
		ǲ�ŵ�����, PutSellPnl, PutSellPnl, long, 16;
		ǲ�ż�����, PutBuyQty, PutBuyQty, long, 16;
		ǲ�ż�����, PutBuyPnl, PutBuyPnl, long, 16;
	end
	CFOFQ02400OutBlock4,Out3(*EMPTY*),output,occurs;
	begin
		�����ȣ, IsuNo, IsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
		�ܰ����, BalQty, BalQty, long, 16;
		��հ�, FnoAvrPrc, FnoAvrPrc, double, 19.8;
		���ʱݾ�, BgnAmt, BgnAmt, long, 16;
		����û�����, ThdayLqdtQty, ThdayLqdtQty, long, 16;
		���簡, Curprc, Curprc, double, 13.2;
		�򰡱ݾ�, EvalAmt, EvalAmt, long, 16;
		�򰡼��ͱݾ�, EvalPnlAmt, EvalPnlAmt, long, 16;
		�򰡼��ͷ�, EvalErnrat, EvalErnrat, double, 12.6;
	end
	END_DATA_MAP
END_FUNCTION_MAP
