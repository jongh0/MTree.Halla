BEGIN_FUNCTION_MAP
.Feed, KOSPI200�ɼǽǽð������Ѱ�(D0), OD0, attr, key=8, group=1 ;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			optcode,			optcode,		char,	8;
    end
    OutBlock,���,output;
    begin
		���ӸŸſ���		,	gubun,			gubun,			char,	1;
		�ǽð��������ѿ���	,	dy_gubun,		dy_gubun,		char,	1;
		�ǽð����Ѱ�		,	dy_uplmtprice,	dy_uplmtprice,	float,	8.2;
		�ǽð����Ѱ�		,	dy_dnlmtprice,	dy_dnlmtprice,	float,	8.2;
		�����ڵ�			,	opttcode,		opttcode,		char,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
