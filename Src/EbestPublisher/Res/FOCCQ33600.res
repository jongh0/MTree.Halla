BEGIN_FUNCTION_MAP
	.Func,�ֽİ��� �Ⱓ�����ͷ� ��,FOCCQ33600,SERVICE=FOCCQ33600,headtype=B,CREATOR=������,CREDATE=2012/03/15 11:06:33;
	BEGIN_DATA_MAP
	FOCCQ33600InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		�Ⱓ����, TermTp, TermTp, char, 1;
	end
	FOCCQ33600OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		��ȸ������, QrySrtDt, QrySrtDt, char, 8;
		��ȸ������, QryEndDt, QryEndDt, char, 8;
		�Ⱓ����, TermTp, TermTp, char, 1;
	end
	FOCCQ33600OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�Ÿž����ݾ�, BnsctrAmt, BnsctrAmt, long, 16;
		�Աݱݾ�, MnyinAmt, MnyinAmt, long, 16;
		��ݱݾ�, MnyoutAmt, MnyoutAmt, long, 16;
		���ڿ������ܱݾ�, InvstAvrbalPramt, InvstAvrbalPramt, long, 16;
		���ڼ��ͱݾ�, InvstPlAmt, InvstPlAmt, long, 16;
		���ڼ��ͷ�, InvstErnrat, InvstErnrat, double, 9.2;
	end
	FOCCQ33600OutBlock3,OutLst(*EMPTY*),output,occurs;
	begin
		������, BaseDt, BaseDt, char, 8;
		�����򰡱ݾ�, FdEvalAmt, FdEvalAmt, long, 19;
		�⸻�򰡱ݾ�, EotEvalAmt, EotEvalAmt, long, 19;
		���ڿ������ܱݾ�, InvstAvrbalPramt, InvstAvrbalPramt, long, 16;
		�Ÿž����ݾ�, BnsctrAmt, BnsctrAmt, long, 16;
		�Աݰ��, MnyinSecinAmt, MnyinSecinAmt, long, 16;
		��ݰ��, MnyoutSecoutAmt, MnyoutSecoutAmt, long, 16;
		�򰡼��ͱݾ�, EvalPnlAmt, EvalPnlAmt, long, 16;
		�Ⱓ���ͷ�, TermErnrat, TermErnrat, double, 11.3;
		����, Idx, Idx, double, 13.2;
	end
	END_DATA_MAP
END_FUNCTION_MAP
