BEGIN_FUNCTION_MAP
	.Func,�Ļ���ǰ���ű�����ȸ,MMDAQ91200,SERVICE=MMDAQ91200,headtype=B,CREATOR=������,CREDATE=2014/09/03 15:16:28;
	BEGIN_DATA_MAP
	MMDAQ91200InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����з��ڵ�, IsuLgclssCode, IsuLgclssCode, char, 2;
		�����ߺз��ڵ�, IsuMdclssCode, IsuMdclssCode, char, 2;
	end
	MMDAQ91200OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		�����з��ڵ�, IsuLgclssCode, IsuLgclssCode, char, 2;
		�����ߺз��ڵ�, IsuMdclssCode, IsuMdclssCode, char, 2;
	end
	MMDAQ91200OutBlock2,Out(*EMPTY*),output,occurs;
	begin
		����Һз��ڵ�, IsuSmclssCode, IsuSmclssCode, char, 3;
		�����ߺз��ڵ�, IsuMdclssCode, IsuMdclssCode, char, 2;
		������ߺз���, IsuLrgMdclssNm, IsuLrgMdclssNm, char, 40;
		������߼Һз���, IsuLrgMidSmclssNm, IsuLrgMidSmclssNm, char, 40;
		�����ѱ������, ShtnHanglIsuNm, ShtnHanglIsuNm, char, 40;
		��Ź���ű���, CsgnMgnrt, CsgnMgnrt, double, 26.9;
		�������ű���, MaintMgnrt, MaintMgnrt, double, 26.9;
		�������ű���, MnyMgnrt, MnyMgnrt, double, 26.9;
		�ܿ��ϼ�, RmndDays, RmndDays, long, 6;
	end
	END_DATA_MAP
END_FUNCTION_MAP
