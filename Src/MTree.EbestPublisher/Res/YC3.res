BEGIN_FUNCTION_MAP
.Feed, ��ǰ��������ü��(YC3), YC3, attr, key=8, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			shcode,			shcode,			char,	8;
    end
    OutBlock,���,output;
    begin
		����ü��ð�,		ychetime,		ychetime,		char,	6;
		����ü�ᰡ��,		yeprice,		yeprice,		float,	9.2;
		����ü�����,		yevolume,		yevolume,		long,	6;
����ü�ᰡ����������񱸺�,	jnilysign,      jnilysign,		char,	1;
����ü�ᰡ�����������,		jnilchange,		preychange,		float,	9.2;
����ü�ᰡ�������������,	jnilydrate,     jnilydrate,		float,	9.2;
		�����ڵ�,			shcode,			shcode,			char,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
