BEGIN_FUNCTION_MAP
.Feed, �����ɼǿ���ü��(YOC), YOC, attr, key=8, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			optcode,			optcode,			char,	8;
    end
    OutBlock,���,output;
    begin
		����ü��ð�,		ychetime,		ychetime,		char,	6;
		����ü�ᰡ��,		yeprice,		yeprice,		float,	6.2;
����ü�ᰡ����������񱸺�,	jnilysign,      jnilysign,		char,	1;
����ü�ᰡ�����������,		jnilchange,		preychange,		float,	6.2;
����ü�ᰡ�������������,	jnilydrate,     jnilydrate,		float,	6.2;
		�����ڵ�,			optcode,		optcode,		char,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
YOC