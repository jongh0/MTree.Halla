BEGIN_FUNCTION_MAP
.Feed, ��/���Ѱ�������Ż(SHD), SHD, attr, key=1, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		��/���ѱ���,		updnlmtgubun,	updnlmtgubun,	char,	1;
    end
    OutBlock,���,output;
    begin
		�ŷ���/�ڽ��ڱ���,	sijanggubun,	sijanggubun,	char,	1;
		�����,			    hname,			hname,			char,	20;
		���簡,				price,			price,			long,	8;
		���ϴ�񱸺�,		sign,			sign,			char,	1;
		���ϴ��,			change,			change,			long,	8;
		�����,				drate,			drate,			float,	6.2;
		�����ŷ���,			volume,			volume,			long,	12;
		�ŷ�������,			volincrate,		volincrate,		float,	12.2;
		��/���Ѱ�,			updnlmtprice,	updnlmtprice,	long,	8;
		��/���Ѱ������,	updnlmtdrate,	updnlmtdrate,	float,	6.2;
		���ϰŷ���,			jnilvolume,		jnilvolume,		long,	12;
		�����ڵ�,	    	shcode,			shcode,			char,	6;
		��������,			gwangubun,		gwangubun,		char,	1;
		�̻�޵��,		undergubun,		undergubun,		char,	1;
		�������Ǳ���,		tgubun,			tgubun,			char,	1;
		�켱�ֱ���,			wgubun,			wgubun,			char,	1;
		�Ҽ��Ǳ���,			dishonest,		dishonest,		char,	1;
		���űݷ�,			jkrate,			jkrate,			char,	1;
    end
    END_DATA_MAP
END_FUNCTION_MAP
