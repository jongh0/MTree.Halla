BEGIN_FUNCTION_MAP
	.Func,�������¿����� �ֹ����ɱݾ� ���� ��ȸ,CSPAQ12200,SERVICE=CSPAQ12200,headtype=B,CREATOR=�̻���,CREDATE=2015/04/13 08:41:27;
	BEGIN_DATA_MAP
	CSPAQ12200InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		����������ȣ, MgmtBrnNo, MgmtBrnNo, char, 3;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�ܰ��������, BalCreTp, BalCreTp, char, 1;
	end
	CSPAQ12200OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		����������ȣ, MgmtBrnNo, MgmtBrnNo, char, 3;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		��й�ȣ, Pwd, Pwd, char, 8;
		�ܰ��������, BalCreTp, BalCreTp, char, 1;
	end
	CSPAQ12200OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		������, BrnNm, BrnNm, char, 40;
		���¸�, AcntNm, AcntNm, char, 40;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		��ݰ��ɱݾ�, MnyoutAbleAmt, MnyoutAbleAmt, long, 16;
		�ŷ��ұݾ�, SeOrdAbleAmt, SeOrdAbleAmt, long, 16;
		�ڽ��ڱݾ�, KdqOrdAbleAmt, KdqOrdAbleAmt, long, 16;
		�ܰ��򰡱ݾ�, BalEvalAmt, BalEvalAmt, long, 16;
		�̼��ݾ�, RcvblAmt, RcvblAmt, long, 16;
		��Ź�ڻ��Ѿ�, DpsastTotamt, DpsastTotamt, long, 16;
		������, PnlRat, PnlRat, double, 18.6;
		���ڿ���, InvstOrgAmt, InvstOrgAmt, long, 20;
		���ڼ��ͱݾ�, InvstPlAmt, InvstPlAmt, long, 16;
		�ſ�㺸�ֹ��ݾ�, CrdtPldgOrdAmt, CrdtPldgOrdAmt, long, 16;
		������, Dps, Dps, long, 16;
		���ݾ�, SubstAmt, SubstAmt, long, 16;
		D1������, D1Dps, D1Dps, long, 16;
		D2������, D2Dps, D2Dps, long, 16;
		���ݹ̼��ݾ�, MnyrclAmt, MnyrclAmt, long, 16;
		���ű�����, MgnMny, MgnMny, long, 16;
		���űݴ��, MgnSubst, MgnSubst, long, 16;
		��ǥ�ݾ�, ChckAmt, ChckAmt, long, 16;
		����ֹ����ɱݾ�, SubstOrdAbleAmt, SubstOrdAbleAmt, long, 16;
		���űݷ�100�ۼ�Ʈ�ֹ����ɱݾ�, MgnRat100pctOrdAbleAmt, MgnRat100pctOrdAbleAmt, long, 16;
		���űݷ�35%�ֹ����ɱݾ�, MgnRat35ordAbleAmt, MgnRat35ordAbleAmt, long, 16;
		���űݷ�50%�ֹ����ɱݾ�, MgnRat50ordAbleAmt, MgnRat50ordAbleAmt, long, 16;
		���ϸŵ�����ݾ�, PrdaySellAdjstAmt, PrdaySellAdjstAmt, long, 16;
		���ϸż�����ݾ�, PrdayBuyAdjstAmt, PrdayBuyAdjstAmt, long, 16;
		���ϸŵ�����ݾ�, CrdaySellAdjstAmt, CrdaySellAdjstAmt, long, 16;
		���ϸż�����ݾ�, CrdayBuyAdjstAmt, CrdayBuyAdjstAmt, long, 16;
		D1��ü�����ҿ�ݾ�, D1ovdRepayRqrdAmt, D1ovdRepayRqrdAmt, long, 16;
		D2��ü�����ҿ�ݾ�, D2ovdRepayRqrdAmt, D2ovdRepayRqrdAmt, long, 16;
		D1�������Ⱑ�ɱݾ�, D1PrsmptWthdwAbleAmt, D1PrsmptWthdwAbleAmt, long, 16;
		D2�������Ⱑ�ɱݾ�, D2PrsmptWthdwAbleAmt, D2PrsmptWthdwAbleAmt, long, 16;
		��Ź�㺸����ݾ�, DpspdgLoanAmt, DpspdgLoanAmt, long, 16;
		�ſ뼳��������, Imreq, Imreq, long, 16;
		���ڱݾ�, MloanAmt, MloanAmt, long, 16;
		�����Ĵ㺸����, ChgAfPldgRat, ChgAfPldgRat, double, 9.3;
		���㺸�ݾ�, OrgPldgAmt, OrgPldgAmt, long, 16;
		�δ㺸�ݾ�, SubPldgAmt, SubPldgAmt, long, 16;
		�ҿ�㺸�ݾ�, RqrdPldgAmt, RqrdPldgAmt, long, 16;
		���㺸�����ݾ�, OrgPdlckAmt, OrgPdlckAmt, long, 16;
		�㺸�����ݾ�, PdlckAmt, PdlckAmt, long, 16;
		�߰��㺸����, AddPldgMny, AddPldgMny, long, 16;
		D1�ֹ����ɱݾ�, D1OrdAbleAmt, D1OrdAbleAmt, long, 16;
		�ſ����ڹ̳��ݾ�, CrdtIntdltAmt, CrdtIntdltAmt, long, 16;
		��Ÿ�뿩�ݾ�, EtclndAmt, EtclndAmt, long, 16;
		���������ݴ�Ÿűݾ�, NtdayPrsmptCvrgAmt, NtdayPrsmptCvrgAmt, long, 16;
		���㺸�հ�ݾ�, OrgPldgSumAmt, OrgPldgSumAmt, long, 16;
		�ſ��ֹ����ɱݾ�, CrdtOrdAbleAmt, CrdtOrdAbleAmt, long, 16;
		�δ㺸�հ�ݾ�, SubPldgSumAmt, SubPldgSumAmt, long, 16;
		�ſ�㺸������, CrdtPldgAmtMny, CrdtPldgAmtMny, long, 16;
		�ſ�㺸���ݾ�, CrdtPldgSubstAmt, CrdtPldgSubstAmt, long, 16;
		�߰��ſ�㺸����, AddCrdtPldgMny, AddCrdtPldgMny, long, 16;
		�ſ�㺸����ݾ�, CrdtPldgRuseAmt, CrdtPldgRuseAmt, long, 16;
		�߰��ſ�㺸���, AddCrdtPldgSubst, AddCrdtPldgSubst, long, 16;
		�ŵ���ݴ㺸����ݾ�, CslLoanAmtdt1, CslLoanAmtdt1, long, 16;
		ó�����ѱݾ�, DpslRestrcAmt, DpslRestrcAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
