BEGIN_FUNCTION_MAP
	.Func,�����ɼ� �ɼǸŵ��� �ֹ����ű���ȸ,CFOBQ10800,SERVICE=CFOBQ10800,headtype=B,CREATOR=�����,CREDATE=2011/12/05 09:48:00;
	BEGIN_DATA_MAP
	CFOBQ10800InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ǰ���ڵ�, PrdgrpClssCode, PrdgrpClssCode, char, 2;
		�����ڻ��ڵ�, ClssGrpCode, ClssGrpCode, char, 3;
		�ֱٿ�������, FstmmTpCode, FstmmTpCode, char, 1;
	end
	CFOBQ10800OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ǰ���ڵ�, PrdgrpClssCode, PrdgrpClssCode, char, 2;
		�����ڻ��ڵ�, ClssGrpCode, ClssGrpCode, char, 3;
		�ֱٿ�������, FstmmTpCode, FstmmTpCode, char, 1;
	end
	CFOBQ10800OutBlock2,Out(*EMPTY*),output,occurs;
	begin
		��簡, ElwXrcPrc, ElwXrcPrc, double, 13.2;
		�����ɼ������ȣ, FnoIsuNo, FnoIsuNo, char, 12;
		�ѱ������1, HanglIsuNm1, HanglIsuNm1, char, 40;
		���и�1, TpNm1, TpNm1, char, 40;
		�̷а�1, Thrprc1, Thrprc1, double, 19.8;
		���ذ�1, BasePrc1, BasePrc1, double, 13.2;
		�ֹ����űݾ�1, OrdMgn1, OrdMgn1, long, 16;
		�����ɼ������ȣ0, FnoIsuNo0, FnoIsuNo0, char, 12;
		�ѱ������2, HanglIsuNm2, HanglIsuNm2, char, 40;
		���и�2, TpNm2, TpNm2, char, 40;
		�̷а�2, Thrprc2, Thrprc2, double, 19.8;
		���ذ�2, BasePrc2, BasePrc2, double, 13.2;
		�ֹ����űݾ�2, OrdMgn2, OrdMgn2, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
