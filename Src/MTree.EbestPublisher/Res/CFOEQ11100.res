BEGIN_FUNCTION_MAP
	.Func,�����ɼǰ����꿹Ź�ݻ�,CFOEQ11100,SERVICE=CFOEQ11100,headtype=B,CREATOR=������,CREDATE=2012/03/16 14:04:41;
	BEGIN_DATA_MAP
	CFOEQ11100InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�Ÿ���, BnsDt, BnsDt, char, 8;
	end
	CFOEQ11100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�Ÿ���, BnsDt, BnsDt, char, 8;
	end
	CFOEQ11100OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		����ÿ�Ź���Ѿ�, OpnmkDpsamtTotamt, OpnmkDpsamtTotamt, long, 16;
		����ÿ�����, OpnmkDps, OpnmkDps, long, 16;
		��������ݹ̼���, OpnmkMnyrclAmt, OpnmkMnyrclAmt, long, 16;
		����ô��ݾ�, OpnmkSubstAmt, OpnmkSubstAmt, long, 16;
		�ѱݾ�, TotAmt, TotAmt, long, 16;
		������, Dps, Dps, long, 16;
		���ݹ̼��ݾ�, MnyrclAmt, MnyrclAmt, long, 16;
		��������ݾ�, SubstDsgnAmt, SubstDsgnAmt, long, 16;
		��Ź���űݾ�, CsgnMgn, CsgnMgn, long, 16;
		������Ź���űݾ�, MnyCsgnMgn, MnyCsgnMgn, long, 16;
		�������űݾ�, MaintMgn, MaintMgn, long, 16;
		�����������űݾ�, MnyMaintMgn, MnyMaintMgn, long, 16;
		��ݰ����Ѿ�, OutAbleAmt, OutAbleAmt, long, 16;
		��ݰ��ɱݾ�, MnyoutAbleAmt, MnyoutAbleAmt, long, 16;
		��ݰ��ɴ��, SubstOutAbleAmt, SubstOutAbleAmt, long, 16;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		�߰����űݱ���, AddMgnOcrTpCode, AddMgnOcrTpCode, char, 1;
		�߰����űݾ�, AddMgn, AddMgn, long, 16;
		�����߰����űݾ�, MnyAddMgn, MnyAddMgn, long, 16;
		���Ͽ�Ź�Ѿ�, NtdayTotAmt, NtdayTotAmt, long, 16;
		���Ͽ�Ź����, NtdayDps, NtdayDps, long, 16;
		���Ϲ̼���, NtdayMnyrclAmt, NtdayMnyrclAmt, long, 16;
		���Ͽ�Ź���, NtdaySubstAmt, NtdaySubstAmt, long, 16;
		������Ź���ű�, NtdayCsgnMgn, NtdayCsgnMgn, long, 16;
		������Ź���ű�����, NtdayMnyCsgnMgn, NtdayMnyCsgnMgn, long, 16;
		�����������ű�, NtdayMaintMgn, NtdayMaintMgn, long, 16;
		�����������ű�����, NtdayMnyMaintMgn, NtdayMnyMaintMgn, long, 16;
		�������Ⱑ�ɱݾ�, NtdayOutAbleAmt, NtdayOutAbleAmt, long, 16;
		�������Ⱑ�ɱݾ�, NtdayMnyoutAbleAmt, NtdayMnyoutAbleAmt, long, 16;
		�������Ⱑ�ɴ��, NtdaySubstOutAbleAmt, NtdaySubstOutAbleAmt, long, 16;
		�����ֹ����ɱݾ�, NtdayOrdAbleAmt, NtdayOrdAbleAmt, long, 16;
		�����ֹ���������, NtdayMnyOrdAbleAmt, NtdayMnyOrdAbleAmt, long, 16;
		�����߰����űݱ���, NtdayAddMgnTp, NtdayAddMgnTp, char, 1;
		�����߰����ű�, NtdayAddMgn, NtdayAddMgn, long, 16;
		�����߰����ű�����, NtdayMnyAddMgn, NtdayMnyAddMgn, long, 16;
		���ϰ����ݾ�, NtdaySettAmt, NtdaySettAmt, long, 16;
		�򰡿�Ź���Ѿ�, EvalDpsamtTotamt, EvalDpsamtTotamt, long, 15;
		�����򰡿�Ź�ݾ�, MnyEvalDpstgAmt, MnyEvalDpstgAmt, long, 15;
		��Ź���̿�����޿����ݾ�, DpsamtUtlfeeGivPrergAmt, DpsamtUtlfeeGivPrergAmt, long, 16;
		����, TaxAmt, TaxAmt, long, 16;
		��Ź���ű� ����, CsgnMgnrat, CsgnMgnrat, double, 7.2;
		��Ź���ű����ݺ���, CsgnMnyMgnrat, CsgnMnyMgnrat, double, 7.2;
		��Ź�Ѿ׺����ݾ�(��Ź���űݱ���), DpstgTotamtLackAmt, DpstgTotamtLackAmt, long, 16;
		��Ź���ݺ����ݾ�(��Ź���űݱ���), DpstgMnyLackAmt, DpstgMnyLackAmt, long, 16;
		���Աݾ�, RealInAmt, RealInAmt, long, 16;
		�Աݾ�, InAmt, InAmt, long, 16;
		��ݾ�, OutAmt, OutAmt, long, 16;
		������������, FutsAdjstDfamt, FutsAdjstDfamt, long, 16;
		������������, FutsThdayDfamt, FutsThdayDfamt, long, 16;
		������������, FutsUpdtDfamt, FutsUpdtDfamt, long, 16;
		����������������, FutsLastSettDfamt, FutsLastSettDfamt, long, 16;
		�ɼǰ�������, OptSettDfamt, OptSettDfamt, long, 16;
		�ɼǸż��ݾ�, OptBuyAmt, OptBuyAmt, long, 16;
		�ɼǸŵ��ݾ�, OptSellAmt, OptSellAmt, long, 16;
		�ɼ��������, OptXrcDfamt, OptXrcDfamt, long, 16;
		�ɼǹ�������, OptAsgnDfamt, OptAsgnDfamt, long, 16;
		�ǹ��μ����ݾ�, RealGdsUndAmt, RealGdsUndAmt, long, 16;
		�ǹ��μ����������, RealGdsUndAsgnAmt, RealGdsUndAsgnAmt, long, 16;
		�ǹ��μ��������, RealGdsUndXrcAmt, RealGdsUndXrcAmt, long, 16;
		������, CmsnAmt, CmsnAmt, long, 16;
		����������, FutsCmsn, FutsCmsn, long, 16;
		�ɼǼ�����, OptCmsn, OptCmsn, long, 16;
		������������, FutsCtrctQty, FutsCtrctQty, long, 16;
		���������ݾ�, FutsCtrctAmt, FutsCtrctAmt, long, 16;
		�ɼǾ�������, OptCtrctQty, OptCtrctQty, long, 16;
		�ɼǾ����ݾ�, OptCtrctAmt, OptCtrctAmt, long, 16;
		�����̰�������, FutsUnsttQty, FutsUnsttQty, long, 16;
		�����̰����ݾ�, FutsUnsttAmt, FutsUnsttAmt, long, 16;
		�ɼǹ̰�������, OptUnsttQty, OptUnsttQty, long, 16;
		�ɼǹ̰����ݾ�, OptUnsttAmt, OptUnsttAmt, long, 16;
		�����ż��̰�������, FutsBuyUnsttQty, FutsBuyUnsttQty, long, 16;
		�����ż��̰����ݾ�, FutsBuyUnsttAmt, FutsBuyUnsttAmt, long, 16;
		�����ŵ��̰�������, FutsSellUnsttQty, FutsSellUnsttQty, long, 16;
		�����ŵ��̰����ݾ�, FutsSellUnsttAmt, FutsSellUnsttAmt, long, 16;
		�ɼǸż��̰�������, OptBuyUnsttQty, OptBuyUnsttQty, long, 16;
		�ɼǸż��̰����ݾ�, OptBuyUnsttAmt, OptBuyUnsttAmt, long, 16;
		�ɼǸŵ��̰�������, OptSellUnsttQty, OptSellUnsttQty, long, 16;
		�ɼǸŵ��̰����ݾ�, OptSellUnsttAmt, OptSellUnsttAmt, long, 16;
		�����ż���������, FutsBuyctrQty, FutsBuyctrQty, long, 16;
		�����ż������ݾ�, FutsBuyctrAmt, FutsBuyctrAmt, long, 16;
		�����ŵ���������, FutsSlctrQty, FutsSlctrQty, long, 16;
		�����ŵ������ݾ�, FutsSlctrAmt, FutsSlctrAmt, long, 16;
		�ɼǸż���������, OptBuyctrQty, OptBuyctrQty, long, 16;
		�ɼǸż������ݾ�, OptBuyctrAmt, OptBuyctrAmt, long, 16;
		�ɼǸŵ���������, OptSlctrQty, OptSlctrQty, long, 16;
		�ɼǸŵ������ݾ�, OptSlctrAmt, OptSlctrAmt, long, 16;
		�����Ÿż��ͱݾ�, FutsBnsplAmt, FutsBnsplAmt, long, 16;
		�ɼǸŸż��ͱݾ�, OptBnsplAmt, OptBnsplAmt, long, 16;
		�����򰡼��ͱݾ�, FutsEvalPnlAmt, FutsEvalPnlAmt, long, 16;
		�ɼ��򰡼��ͱݾ�, OptEvalPnlAmt, OptEvalPnlAmt, long, 16;
		�����򰡱ݾ�, FutsEvalAmt, FutsEvalAmt, long, 16;
		�ɼ��򰡱ݾ�, OptEvalAmt, OptEvalAmt, long, 16;
		�������������Աݱݾ�, MktEndAfMnyInAmt, MktEndAfMnyInAmt, long, 16;
		��������������ݱݾ�, MktEndAfMnyOutAmt, MktEndAfMnyOutAmt, long, 16;
		�������Ĵ�������ݾ�, MktEndAfSubstDsgnAmt, MktEndAfSubstDsgnAmt, long, 16;
		�������Ĵ�������ݾ�, MktEndAfSubstAbndAmt, MktEndAfSubstAbndAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
