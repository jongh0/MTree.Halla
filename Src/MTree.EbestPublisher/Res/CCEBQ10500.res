BEGIN_FUNCTION_MAP
	.Func,�����ɼ� CME ��Ź�����ű���ȸ,CCEBQ10500,SERVICE=CCEBQ10500,headtype=B,CREATOR=���ȯ,CREDATE=2012/04/16 18:02:19;
	BEGIN_DATA_MAP
	CCEBQ10500InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
	end
	CCEBQ10500OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
	end
	CCEBQ10500OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		��Ź���Ѿ�, DpsamtTotamt, DpsamtTotamt, long, 16;
		������, Dps, Dps, long, 16;
		���ݾ�, SubstAmt, SubstAmt, long, 16;
		��翹Ź���Ѿ�, FilupDpsamtTotamt, FilupDpsamtTotamt, long, 16;
		��翹����, FilupDps, FilupDps, long, 16;
		�������ͱݾ�, FutsPnlAmt, FutsPnlAmt, long, 16;
		���Ⱑ�ɱݾ�, WthdwAbleAmt, WthdwAbleAmt, long, 16;
		���Ⱑ�����ݾ�, PsnOutAbleCurAmt, PsnOutAbleCurAmt, long, 16;
		���Ⱑ�ɴ��ݾ�, PsnOutAbleSubstAmt, PsnOutAbleSubstAmt, long, 16;
		���űݾ�, Mgn, Mgn, long, 16;
		�������űݾ�, MnyMgn, MnyMgn, long, 16;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		�߰����űݾ�, AddMgn, AddMgn, long, 16;
		�����߰����űݾ�, MnyAddMgn, MnyAddMgn, long, 16;
		�����ϼ�ǥ�Աݾ�, AmtPrdayChckInAmt, AmtPrdayChckInAmt, long, 16;
		�����ɼ����ϴ��ŵ��ݾ�, FnoPrdaySubstSellAmt, FnoPrdaySubstSellAmt, long, 16;
		�����ɼǱ��ϴ��ŵ��ݾ�, FnoCrdaySubstSellAmt, FnoCrdaySubstSellAmt, long, 16;
		�����ɼ����ϰ��Աݾ�, FnoPrdayFdamt, FnoPrdayFdamt, long, 16;
		�����ɼǱ��ϰ��Աݾ�, FnoCrdayFdamt, FnoCrdayFdamt, long, 16;
		��ȭ���ݾ�, FcurrSubstAmt, FcurrSubstAmt, long, 16;
		�����ɼǰ��»������űݸ�, FnoAcntAfmgnNm, FnoAcntAfmgnNm, char, 20;
	end
	CCEBQ10500OutBlock3,Out2(*EMPTY*),output,occurs;
	begin
		��ǰ���ڵ��, PdGrpCodeNm, PdGrpCodeNm, char, 20;
		���������űݾ�, NetRiskMgn, NetRiskMgn, long, 16;
		�������űݾ�, PrcMgn, PrcMgn, long, 16;
		�����������űݾ�, SprdMgn, SprdMgn, long, 16;
		���ݺ������űݾ�, PrcFlctMgn, PrcFlctMgn, long, 16;
		�ּ����űݾ�, MinMgn, MinMgn, long, 16;
		�ֹ����űݾ�, OrdMgn, OrdMgn, long, 16;
		�ɼǼ��ż��ݾ�, OptNetBuyAmt, OptNetBuyAmt, long, 16;
		��Ź���űݾ�, CsgnMgn, CsgnMgn, long, 16;
		�������űݾ�, MaintMgn, MaintMgn, long, 16;
		�����ż�ü��ݾ�, FutsBuyExecAmt, FutsBuyExecAmt, long, 16;
		�����ŵ�ü��ݾ�, FutsSellExecAmt, FutsSellExecAmt, long, 16;
		�ɼǸż�ü��ݾ�, OptBuyExecAmt, OptBuyExecAmt, long, 16;
		�ɼǸŵ�ü��ݾ�, OptSellExecAmt, OptSellExecAmt, long, 16;
		�������ͱݾ�, FutsPnlAmt, FutsPnlAmt, long, 16;
		��������Ź���ű�, TotRiskCsgnMgn, TotRiskCsgnMgn, long, 16;
		�μ�����Ź���ű�, UndCsgnMgn, UndCsgnMgn, long, 16;
		���űݰ���ݾ�, MgnRdctAmt, MgnRdctAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
