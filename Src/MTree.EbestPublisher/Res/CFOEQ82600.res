BEGIN_FUNCTION_MAP
	.Func,�����ɼ� �Ϻ� ���¼��ͳ���,CFOEQ82600,SERVICE=CFOEQ82600,headtype=B,CREATOR=Ȳ�Լ�,CREDATE=2013/01/06 17:48:51;
	BEGIN_DATA_MAP
	CFOEQ82600InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		��ȸ����, QryTp, QryTp, char, 1;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
		�����ɼ��ܰ��򰡱����ڵ�, FnoBalEvalTpCode, FnoBalEvalTpCode, char, 1;
	end
	CFOEQ82600OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		��ȸ����, QryTp, QryTp, char, 1;
		���ļ�������, StnlnSeqTp, StnlnSeqTp, char, 1;
		�����ɼ��ܰ��򰡱����ڵ�, FnoBalEvalTpCode, FnoBalEvalTpCode, char, 1;
	end
	CFOEQ82600OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		������������, FutsAdjstDfamt, FutsAdjstDfamt, long, 16;
		�ɼǸŸż��ͱݾ�, OptBnsplAmt, OptBnsplAmt, long, 16;
		�����ɼǼ�����, FnoCmsnAmt, FnoCmsnAmt, long, 16;
		�����հ�ݾ�, PnlSumAmt, PnlSumAmt, long, 16;
		�Ա��հ�ݾ�, MnyinSumAmt, MnyinSumAmt, long, 16;
		����հ�ݾ�, MnyoutSumAmt, MnyoutSumAmt, long, 16;
		���¸�, AcntNm, AcntNm, char, 40;
	end
	CFOEQ82600OutBlock3,OutList(*EMPTY*),output,occurs;
	begin
		��ȸ��, QryDt, QryDt, char, 8;
		��Ź�Ѿ�, DpstgTotamt, DpstgTotamt, long, 16;
		��Ź����, DpstgMny, DpstgMny, long, 16;
		�����ɼ����űݾ�, FnoMgn, FnoMgn, long, 16;
		�������ͱݾ�, FutsPnlAmt, FutsPnlAmt, long, 16;
		�ɼǸŸż��ͱݾ�, OptBsnPnlAmt, OptBsnPnlAmt, long, 16;
		�ɼ��򰡼��ͱݾ�, OptEvalPnlAmt, OptEvalPnlAmt, long, 16;
		������, CmsnAmt, CmsnAmt, long, 16;
		�հ�ݾ�1, SumAmt1, SumAmt1, long, 16;
		�հ�ݾ�, SumAmt2, SumAmt2, long, 16;
		�����հ�ݾ�, PnlSumAmt, PnlSumAmt, long, 16;
		�����ż��ݾ�, FutsBuyAmt, FutsBuyAmt, long, 16;
		�����ŵ��ݾ�, FutsSellAmt, FutsSellAmt, long, 16;
		�ɼǸż��ݾ�, OptBuyAmt, OptBuyAmt, long, 16;
		�ɼǸŵ��ݾ�, OptSellAmt, OptSellAmt, long, 16;
		�Աݾ�, InAmt, InAmt, long, 16;
		��ݾ�, OutAmt, OutAmt, long, 16;
		�򰡱ݾ�, EvalAmt, EvalAmt, long, 16;
		�ջ��򰡱ݾ�, AddupEvalAmt, AddupEvalAmt, long, 16;
		�ݾ�2, Amt2, Amt2, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
