BEGIN_FUNCTION_MAP
.Feed, KOSPI200�ɼǰ���������Ȯ��(X0), OX0, attr, key=8, group=1 ;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			optcode,			optcode,		char,	8;
    end
    OutBlock,���,output;
    begin
		���� ���Ѵܰ�	,	upstep		,	upstep		,	char,	2;
		���� ���Ѵܰ�	,	dnstep		,	dnstep		,	char,	2;
		���� ���Ѱ�		,	uplmtprice	,	uplmtprice	,	float,	6.2;
		���� ���Ѱ�		,	dnlmtprice	,	dnlmtprice	,	float,	6.2;
		�����ڵ�		,	opttcode	,	opttcode	,	char,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
