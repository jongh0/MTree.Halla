BEGIN_FUNCTION_MAP
	.Func,�����������űݷ����ֹ����ɼ�����ȸ,CSPBQ00200,SERVICE=CSPBQ00200,headtype=B,CREATOR=�̻���,CREDATE=2011/12/12 09:22:25;
	BEGIN_DATA_MAP
	CSPBQ00200InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		��Ÿ�ü�ڵ�, RegCommdaCode, RegCommdaCode, char, 2;
	end
	CSPBQ00200OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�Ÿű���, BnsTpCode, BnsTpCode, char, 1;
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�ֹ�����, OrdPrc, OrdPrc, double, 15.2;
		��Ÿ�ü�ڵ�, RegCommdaCode, RegCommdaCode, char, 2;
	end
	CSPBQ00200OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�����, IsuNm, IsuNm, char, 40;
		������, Dps, Dps, long, 16;
		���ݾ�, SubstAmt, SubstAmt, long, 16;
		�ſ�㺸����ݾ�, CrdtPldgRuseAmt, CrdtPldgRuseAmt, long, 16;
		�����ֹ����ɱݾ�, MnyOrdAbleAmt, MnyOrdAbleAmt, long, 16;
		����ֹ����ɱݾ�, SubstOrdAbleAmt, SubstOrdAbleAmt, long, 16;
		�������űݾ�, MnyMgn, MnyMgn, long, 16;
		������űݾ�, SubstMgn, SubstMgn, long, 16;
		�ŷ��ұݾ�, SeOrdAbleAmt, SeOrdAbleAmt, long, 16;
		�ڽ��ڱݾ�, KdqOrdAbleAmt, KdqOrdAbleAmt, long, 16;
		����������(D+1), PrsmptDpsD1, PrsmptDpsD1, long, 16;
		����������(D+2), PrsmptDpsD2, PrsmptDpsD2, long, 16;
		��ݰ��ɱݾ�, MnyoutAbleAmt, MnyoutAbleAmt, long, 16;
		�̼��ݾ�, RcvblAmt, RcvblAmt, long, 16;
		��������, CmsnRat, CmsnRat, double, 15.5;
		�߰�¡���ݾ�, AddLevyAmt, AddLevyAmt, long, 16;
		������ݾ�, RuseObjAmt, RuseObjAmt, long, 16;
		����������ݾ�, MnyRuseObjAmt, MnyRuseObjAmt, long, 16;
		�̿�����űݷ�, FirmMgnRat, FirmMgnRat, double, 7.4;
		���������ݾ�, SubstRuseObjAmt, SubstRuseObjAmt, long, 16;
		�������űݷ�, IsuMgnRat, IsuMgnRat, double, 7.4;
		�������űݷ�, AcntMgnRat, AcntMgnRat, double, 7.4;
		�ŷ����űݷ�, TrdMgnrt, TrdMgnrt, double, 7.4;
		������, Cmsn, Cmsn, long, 16;
		���űݷ�20�ۼ�Ʈ�ֹ����ɱݾ�, MgnRat20pctOrdAbleAmt, MgnRat20pctOrdAbleAmt, long, 16;
		���űݷ�100�ۼ�Ʈ�����ֹ����ɼ���?, MgnRat20OrdAbleQty, MgnRat20OrdAbleQty, long, 16;
		���űݷ�30�ۼ�Ʈ�ֹ����ɱݾ�, MgnRat30pctOrdAbleAmt, MgnRat30pctOrdAbleAmt, long, 16;
		���űݷ�30�ۼ�Ʈ�ֹ����ɼ���??, MgnRat30OrdAbleQty, MgnRat30OrdAbleQty, long, 16;
		���űݷ�40�ۼ�Ʈ�ֹ����ɱݾ�, MgnRat40pctOrdAbleAmt, MgnRat40pctOrdAbleAmt, long, 16;
		���űݷ�40�ۼ�Ʈ�ֹ����ɼ���??, MgnRat40OrdAbleQty, MgnRat40OrdAbleQty, long, 16;
		���űݷ�100�ۼ�Ʈ�ֹ����ɱݾ�, MgnRat100pctOrdAbleAmt, MgnRat100pctOrdAbleAmt, long, 16;
		���űݷ�100�ۼ�Ʈ�ֹ����ɼ���??, MgnRat100OrdAbleQty, MgnRat100OrdAbleQty, long, 16;
		���űݷ�100�ۼ�Ʈ�����ֹ����ɱݾ�?, MgnRat100MnyOrdAbleAmt, MgnRat100MnyOrdAbleAmt, long, 16;
		���űݷ�100�ۼ�Ʈ�����ֹ����ɼ���, MgnRat100MnyOrdAbleQty, MgnRat100MnyOrdAbleQty, long, 16;
		���űݷ�20�ۼ�Ʈ���밡�ɱݾ�, MgnRat20pctRuseAbleAmt, MgnRat20pctRuseAbleAmt, long, 16;
		���űݷ�30�ۼ�Ʈ���밡�ɱݾ�, MgnRat30pctRuseAbleAmt, MgnRat30pctRuseAbleAmt, long, 16;
		���űݷ�40�ۼ�Ʈ���밡�ɱݾ�, MgnRat40pctRuseAbleAmt, MgnRat40pctRuseAbleAmt, long, 16;
		���űݷ�100�ۼ�Ʈ���밡�ɱݾ�, MgnRat100pctRuseAbleAmt, MgnRat100pctRuseAbleAmt, long, 16;
		�ֹ����ɼ���, OrdAbleQty, OrdAbleQty, long, 16;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
CSPBQ00200