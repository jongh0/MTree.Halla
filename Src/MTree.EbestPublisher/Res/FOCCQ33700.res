BEGIN_FUNCTION_MAP
	.Func,�����ɼ� �Ⱓ�� ���� ���ͷ� ��Ȳ,FOCCQ33700,SERVICE=FOCCQ33700,headtype=B,CREATOR=�̼���,CREDATE=2013/01/08 10:45:28;
	BEGIN_DATA_MAP
	FOCCQ33700InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		��ȸ����, QryTp, QryTp, char, 1;
		���رݾױ���, BaseAmtTp, BaseAmtTp, char, 1;
		��ȸ�Ⱓ����, QryTermTp, QryTermTp, char, 1;
		���ͻ��ⱸ���ڵ�, PnlCalcTpCode, PnlCalcTpCode, char, 1;
	end
	FOCCQ33700OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		��ȸ����, QryTp, QryTp, char, 1;
		���رݾױ���, BaseAmtTp, BaseAmtTp, char, 1;
		��ȸ�Ⱓ����, QryTermTp, QryTermTp, char, 1;
		���ͻ��ⱸ���ڵ�, PnlCalcTpCode, PnlCalcTpCode, char, 1;
	end
	FOCCQ33700OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�Աݾ�, InAmt, InAmt, long, 16;
		��ݾ�, OutAmt, OutAmt, long, 16;
		�����ɼǾ����ݾ�, FnoCtrctAmt, FnoCtrctAmt, long, 16;
		���ڿ������ܱݾ�, InvstPramtAvrbalAmt, InvstPramtAvrbalAmt, long, 16;
		������������, FutsAdjstDfamt, FutsAdjstDfamt, long, 16;
		�ɼǸŸż��ͱݾ�, OptBsnPnlAmt, OptBsnPnlAmt, long, 16;
		�ɼ��򰡼��ͱݾ�, OptEvalPnlAmt, OptEvalPnlAmt, long, 16;
		���ڼ��ͱݾ�, InvstPlAmt, InvstPlAmt, long, 16;
		���ͷ�, ErnRat, ErnRat, double, 12.6;
	end
	FOCCQ33700OutBlock3,OutList(*EMPTY*),output,occurs;
	begin
		�ŷ���, TrdDt, TrdDt, char, 8;
		���ʿ�Ź�ڻ�ݾ�, FdDpsastAmt, FdDpsastAmt, long, 16;
		�⸻��Ź�ڻ�ݾ�, EotDpsastAmt, EotDpsastAmt, long, 16;
		�Աݾ�, InAmt, InAmt, long, 16;
		��ݾ�, OutAmt, OutAmt, long, 16;
		���ڿ������ܱݾ�, InvstAvrbalPramt, InvstAvrbalPramt, long, 16;
		���ڼ��ͱݾ�, InvstPlAmt, InvstPlAmt, long, 16;
		���ͷ�, Ernrat, Ernrat, double, 12.6;
		�����ɼǾ����ݾ�, FnoCtrctAmt, FnoCtrctAmt, long, 16;
		ȸ����, Trnrat, Trnrat, double, 12.6;
		������������, FutsAdjstDfamt, FutsAdjstDfamt, long, 16;
		�ɼǸŸż��ͱݾ�, OptBsnPnlAmt, OptBsnPnlAmt, long, 16;
		�ɼ��򰡼��ͱݾ�, OptEvalPnlAmt, OptEvalPnlAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
