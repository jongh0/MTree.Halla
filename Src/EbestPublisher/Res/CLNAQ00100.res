BEGIN_FUNCTION_MAP
	.Func,��Ź�㺸���ڰ���������Ȳ��ȸ,CLNAQ00100,SERVICE=CLNAQ00100,headtype=B,CREATOR=�̼�ȣ,CREDATE=2012/01/10 15:45:49;
	BEGIN_DATA_MAP
	CLNAQ00100InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ȸ����, QryTp, QryTp, char, 1;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�������Ǳ���, SecTpCode, SecTpCode, char, 1;
		�������ڵ���ڵ�, LoanIntrstGrdCode, LoanIntrstGrdCode, char, 2;
		���ⱸ��, LoanTp, LoanTp, char, 1;
	end
	CLNAQ00100OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ȸ����, QryTp, QryTp, char, 1;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�������Ǳ���, SecTpCode, SecTpCode, char, 1;
		�������ڵ���ڵ�, LoanIntrstGrdCode, LoanIntrstGrdCode, char, 2;
		���ⱸ��, LoanTp, LoanTp, char, 1;
	end
	CLNAQ00100OutBlock2,Out(*EMPTY*),output,occurs;
	begin
		�����ȣ, IsuNo, IsuNo, char, 12;
		�����, IsuNm, IsuNm, char, 40;
		�׸鰡, Parprc, Parprc, double, 13.2;
		��������, PrdayCprc, PrdayCprc, double, 13.2;
		������, RatVal, RatVal, double, 19.8;
		��밡, SubstPrc, SubstPrc, double, 13.2;
		��ϱ���, RegTpNm, RegTpNm, char, 20;
		�������ű�¡���з���, SpotMgnLevyClssNm, SpotMgnLevyClssNm, char, 40;
		�ŷ���������, FnoTrdStopRsnCnts, FnoTrdStopRsnCnts, char, 40;
		������������, DgrsPtnNm, DgrsPtnNm, char, 40;
		�������, AcdPtnNm, AcdPtnNm, char, 40;
		���屸��, MktTpNm, MktTpNm, char, 20;
		�ѵ���, LmtVal, LmtVal, long, 18;
		�����ѵ���, AcntLmtVal, AcntLmtVal, long, 18;
		�������ڵ�, LoanGrdCode, LoanGrdCode, char, 2;
		����ݾ�, LoanAmt, LoanAmt, long, 16;
		���Ⱑ����, LoanAbleRat, LoanAbleRat, double, 26.9;
		��������1, LoanIntrat1, LoanIntrat1, double, 14.4;
		�����ID, RegPsnId, RegPsnId, char, 16;
		������, Rat01, Rat01, double, 19.8;
		������, Rat02, Rat02, double, 19.8;
	end
	CLNAQ00100OutBlock3,Sum(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		������հ�ݾ�, LrgMnyoutSumAmt, LrgMnyoutSumAmt, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
