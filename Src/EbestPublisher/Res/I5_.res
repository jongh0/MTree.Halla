BEGIN_FUNCTION_MAP
.Feed, �ڽ���ETF����ǽð�NAV(I5), I5_, attr, key=6, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			shcode,			shcode,			char,	6;
    end
    OutBlock,���,output;
    begin
		�ð�,				time,			time,			char,	8;
		���簡,				price,			price,			long,	8;
		���ϴ�񱸺�,		sign,			sign,			char,	1;
		���ϴ��,			change,			change,			long,	8;
		�����ŷ���,			volume,			volume,			float,	12;
		NAV���,			navdiff,		navdiff,		float,	9.2;
		NAV,				nav,			nav,			float,	9.2;
		���ϴ��,			navchange,		navchange,		float,	9.2;
		��������,			crate,			crate,			float,	9.2;
		����,				grate,			grate,			float,	9.2;
		����,				jisu,			jisu,			float,	8.2;
		���ϴ��,			jichange,		jichange,		float,	8.2;
		���ϴ����,			jirate,			jirate,			float,	8.2;
		�����ڵ�,       	shcode,     	shcode,     	char,   6;
    end
    END_DATA_MAP
END_FUNCTION_MAP
