BEGIN_FUNCTION_MAP
.Feed, ELW����ü��(Ys3), Ys3, attr, key=6, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			shcode,			shcode,			char,	6;
    end
    OutBlock,���,output;
    begin
		ȣ���ð�,			hotime,			hotime,			char,	6;
		����ü�ᰡ��,		yeprice,		yeprice,		long,	8;
		����ü�����,		yevolume,		yevolume,		long,	12;
����ü�ᰡ����������񱸺�,	jnilysign,      jnilysign,		char,	1;
����ü�ᰡ�����������,		jnilchange,		preychange,		long,	8;
����ü�ᰡ�������������,	jnilydrate,     jnilydrate,		float,	6.2;
		����ŵ�ȣ��,		yofferho0,		yofferho0,		long,	8;
		����ż�ȣ��,		ybidho0,		ybidho0,		long,	8;
		����ŵ�ȣ������,	yofferrem0,		yofferrem0,		long,	12;
		����ż�ȣ������,	ybidrem0,		ybidrem0,		long,	12;
		�����ڵ�,			shcode,			shcode,			char,	6;
    end
    END_DATA_MAP
END_FUNCTION_MAP
