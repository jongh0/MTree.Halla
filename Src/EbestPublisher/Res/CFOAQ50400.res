BEGIN_FUNCTION_MAP
	.Func,�����ɼ� �����ܰ� �� ����Ȳ2,CFOAQ50400,ENCRYPT,SERVICE=CFOAQ50400,headtype=B,CREATOR=�����,CREDATE=2012/03/12 15:01:36;
	BEGIN_DATA_MAP
	CFOAQ50400InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�ֹ���, OrdDt, OrdDt, char, 8;
		�ܰ��򰡱���, BalEvalTp, BalEvalTp, char, 1;
		���������򰡱���, FutsPrcEvalTp, FutsPrcEvalTp, char, 1;
		û�������ȸ����, LqdtQtyQryTp, LqdtQtyQryTp, char, 1;
	end
	CFOAQ50400OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�ֹ���, OrdDt, OrdDt, char, 8;
		�ܰ��򰡱���, BalEvalTp, BalEvalTp, char, 1;
		���������򰡱���, FutsPrcEvalTp, FutsPrcEvalTp, char, 1;
		û�������ȸ����, LqdtQtyQryTp, LqdtQtyQryTp, char, 1;
	end
	CFOAQ50400OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�򰡿�Ź���Ѿ�, EvalDpsamtTotamt, EvalDpsamtTotamt, long, 15;
		�����򰡿�Ź�ݾ�, MnyEvalDpstgAmt, MnyEvalDpstgAmt, long, 15;
		��Ź���Ѿ�, DpsamtTotamt, DpsamtTotamt, long, 16;
		��Ź����, DpstgMny, DpstgMny, long, 16;
		��Ź���, DpstgSubst, DpstgSubst, long, 16;
		��ȭ���ݾ�, FcurrSubstAmt, FcurrSubstAmt, long, 16;
		���Ⱑ���ѱݾ�, PsnOutAbleTotAmt, PsnOutAbleTotAmt, long, 15;
		���Ⱑ�����ݾ�, PsnOutAbleCurAmt, PsnOutAbleCurAmt, long, 16;
		���Ⱑ�ɴ��ݾ�, PsnOutAbleSubstAmt, PsnOutAbleSubstAmt, long, 16;
		�ֹ������ѱݾ�, OrdAbleTotAmt, OrdAbleTotAmt, long, 15;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		��Ź���ű��Ѿ�, CsgnMgnTotamt, CsgnMgnTotamt, long, 16;
		������Ź���űݾ�, MnyCsgnMgn, MnyCsgnMgn, long, 16;
		�������ű��Ѿ�, MtmgnTotamt, MtmgnTotamt, long, 15;
		�����������űݾ�, MnyMaintMgn, MnyMaintMgn, long, 16;
		�߰����ű��Ѿ�, AddMgnTotamt, AddMgnTotamt, long, 15;
		�����߰����űݾ�, MnyAddMgn, MnyAddMgn, long, 16;
		������, CmsnAmt, CmsnAmt, long, 16;
		�̼��ݾ�, RcvblAmt, RcvblAmt, long, 16;
		�̼���ü��, RcvblOdpnt, RcvblOdpnt, long, 16;
		�����򰡼��ͱݾ�, FutsEvalPnlAmt, FutsEvalPnlAmt, long, 16;
		�ɼ��򰡼��ͱݾ�, OptEvalPnlAmt, OptEvalPnlAmt, long, 16;
		�ɼ��򰡱ݾ�, OptEvalAmt, OptEvalAmt, long, 16;
		�ɼǸŸż��ͱݾ�, OptBnsplAmt, OptBnsplAmt, long, 16;
		������������, FutsAdjstDfamt, FutsAdjstDfamt, long, 16;
		�Ѽ��ͱݾ�, TotPnlAmt, TotPnlAmt, long, 16;
		�����ͱݾ�, NetPnlAmt, NetPnlAmt, long, 16;
	end
	CFOAQ50400OutBlock3,Out2(*EMPTY*),output,occurs;
	begin
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 50;
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		�Ÿű���, BnsTpNm, BnsTpNm, char, 10;
		�̰�������, UnsttQty, UnsttQty, long, 16;
		��հ�, FnoAvrPrc, FnoAvrPrc, double, 19.8;
		���簡, NowPrc, NowPrc, double, 13.2;
		���, CmpPrc, CmpPrc, double, 13.2;
		�򰡼���, EvalPnl, EvalPnl, long, 16;
		���ͷ�, PnlRat, PnlRat, double, 12.6;
		�򰡱ݾ�, EvalAmt, EvalAmt, long, 16;
		�򰡺���, EvalRat, EvalRat, double, 7.2;
		û�갡�ɼ���, LqdtAbleQty, LqdtAbleQty, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
